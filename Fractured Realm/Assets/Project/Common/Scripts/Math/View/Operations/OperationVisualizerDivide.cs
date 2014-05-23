using UnityEngine;
using System.Collections;

public class OperationVisualizerDivide : IOperationVisualizer 
{
	public OperationVisualizerDivide() 
	{
		Reset();
	}
	
	public override void Reset()
	{
		this.type = FR.OperationType.DIVIDE; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerDivide : Visualize : " + current + " TO " + target);

		// Denominator of StopFraction needs to become the Numerator
		// Numerator of StopFraction needs to become the Denominator
		// after that... we need to multiply as usual

		
		yield return LugusCoroutines.use.StartRoutine( VisualizeAnimation(current.StartFraction.Renderer, current.StopFraction.Renderer) ).Coroutine;

		

		yield return new WaitForSeconds(1.0f);

		// Now, we need to multiply the two fractions with eachother
		
		// Note: no need to go through the multiplay IOperation itself: the correct result was calculated by the divide Operation
		// we only need to visualize it the same way we do a multiplication
		IOperationVisualizer mulViz = MathManager.use.GetVisualizer( FR.OperationType.MULTIPLY );
		
		yield return LugusCoroutines.use.StartRoutine( mulViz.Visualize(current, target) ).Coroutine;
		
		// 4. ...
		// 5. Profit!
		
		yield return new WaitForSeconds(0.3f);
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerDivide : finished");
		yield break; 
	}

	public override IEnumerator VisualizeAnimation(FractionRenderer Starter, FractionRenderer Receiver)
	{
		// to switch the two, we need to pull a trick
		// this is because of the two-camera setup
		// we cannot actually re-use the NumberRenderer: we need to make a copy and fake the movements so that it LOOKS like it's moving correctly


		FractionRenderer Switcher = Receiver;

		// create new renderers (don't couple them to the correct Numbers yet)
		// TODO: check if these get the correct parent (i.e. WORLD, etc.)
		NumberRenderer newNumerator = RendererFactory.use.CreateRenderer( new Number(Switcher.Denominator.Number.Value, null, false) );
		NumberRenderer newDenominator = RendererFactory.use.CreateRenderer( new Number(Switcher.Numerator.Number.Value, null, true) );

		yield return LugusCoroutines.use.StartRoutine( VisualizeNumeratorPart(Switcher, newNumerator, newDenominator) ).Coroutine;


		// now, there are nicely switched visually
		// but in the back-end, the renderers are not coupled to the correct data-structures!!!
		
		// 1. actually switch the values in the datastructure
		Number temp = Switcher.Fraction.Numerator;
		Switcher.Fraction.Numerator = Switcher.Fraction.Denominator;
		Switcher.Fraction.Denominator = temp;
		Switcher.Fraction.Numerator.IsNumerator = true; // auto sets IsDenom to false
		Switcher.Fraction.Denominator.IsDenominator = true; // auto sets IsNum to false

		// 2. remove the old renderers
		RendererFactory.use.FreeRenderer( Switcher.Numerator );
		RendererFactory.use.FreeRenderer( Switcher.Denominator );

		// 3. make sure the new renderers now point to the correct numbers and vice versa
		newNumerator.Number = Switcher.Fraction.Numerator;
		newDenominator.Number = Switcher.Fraction.Denominator;
		
		Switcher.Fraction.Numerator.Renderer = newNumerator;
		Switcher.Fraction.Denominator.Renderer = newDenominator;
	}

	public virtual float TimeToTransition()
	{
		return 1.0f;
	}

	public virtual IEnumerator VisualizeNumeratorPart( FractionRenderer Switcher, NumberRenderer newNumerator, NumberRenderer newDenominator )
	{
		//Debug.LogError("OperationVisualizerDivide_Descend:VisualizeAnimationPart : this should be overwritten in the correct Descend animation!");

		FR.Animation denominatorAnimation = this.GetRandomNextAnimation();
		OperationVisualizerDivide denominatorVisualizer = null;
		if( denominatorAnimation == FR.Animation.NONE )
		{
			denominatorVisualizer = this;
		}
		else
		{
			denominatorVisualizer = (OperationVisualizerDivide) FRAnimations.use.animations[ (int) denominatorAnimation ].visualizer;
		}

		float myTime = TimeToTransition();
		float denominatorTime = denominatorVisualizer.TimeToTransition();

		ILugusCoroutineHandle denominatorRoutine = null;

		if( myTime - denominatorTime < 0.0f )
		{
			// denominator takes longer to reach transition. Start first, wait appropriate time, and continue ours

			denominatorRoutine = LugusCoroutines.use.StartRoutine( denominatorVisualizer.VisualizeDenominatorPart( Switcher, newNumerator, newDenominator, 0.0f, myTime, Vector3.zero ) );
			denominatorRoutine.Claim();

			yield return new WaitForSeconds( denominatorTime - myTime );
		}
		else
		{
			// we take longer than denominator.
			denominatorRoutine = LugusCoroutines.use.StartRoutine( denominatorVisualizer.VisualizeDenominatorPart( Switcher, newNumerator, newDenominator, myTime - denominatorTime, myTime, Vector3.zero ) );
			denominatorRoutine.Claim();
		}

		Vector3 numeratorPos = Switcher.Numerator.transform.position;
		Vector3 numeratorOffScreen = numeratorPos - new Vector3(0, 6, 0); // TODO: update these "magic numbers"

		Switcher.Numerator.transform.position = numeratorPos;
		newNumerator.transform.position = numeratorOffScreen;

		Switcher.Numerator.gameObject.MoveTo( numeratorOffScreen ).Time( 2.0f ).Execute();
		newNumerator.gameObject.MoveTo( numeratorPos ).Time(2.0f).Execute();

		yield return new WaitForSeconds(2.0f);


		while( denominatorRoutine.Running )
			yield return null;

		denominatorRoutine.Release();

		/*
		// move original renderers down and up
		// moving up is a longer way than moving down!
		Vector3 numeratorUp = Switcher.Numerator.transform.position;
		Vector3 numeratorDown = numeratorUp
		
		Vector3 denominatorDown = Switcher.Denominator.transform.position;
		Vector3 denominatorUp = denominatorDown + new Vector3(0, 8, 0);// TODO: update these "magic numbers"
		
		Switcher.Numerator.gameObject.MoveTo( numeratorDown ).Time (2.0f).Execute();
		Switcher.Denominator.gameObject.MoveTo( denominatorUp ).Time (2.0f).Execute(); 
		
		
		//Debug.LogError("Divide : newNum is denom vis? " + newNumerator.VisualizedAsDenominator );
		//Debug.LogError("Divide : newDenom is num vis? " + newDenominator.VisualizedAsNumerator );
		
		newNumerator.transform.position = numeratorDown;
		newNumerator.gameObject.MoveTo( numeratorUp ).Time (2.0f).Execute();
		
		newDenominator.transform.position = denominatorUp;
		newDenominator.gameObject.MoveTo( denominatorDown ).Time (2.0f).Execute();
		
		yield return new WaitForSeconds(2.0f);
		*/


		yield break;
	}

	public virtual IEnumerator VisualizeDenominatorPart( FractionRenderer Switcher, NumberRenderer newNumerator, NumberRenderer newDenominator, float delay, float numeratorTransitionTime, Vector3 numeratorTransitionOffset )
	{
		if( delay > 0.0f )
			yield return new WaitForSeconds( delay );
		
		Vector3 denominatorPos = Switcher.Denominator.transform.position;
		Vector3 denominatorOffScreen = denominatorPos + new Vector3(0, 8, 0); // TODO: update these "magic numbers"
		
		Switcher.Denominator.transform.position = denominatorPos;
		newDenominator.transform.position = denominatorOffScreen;
		
		Switcher.Denominator.gameObject.MoveTo( denominatorOffScreen ).Time( 2.0f ).Execute();
		newDenominator.gameObject.MoveTo( denominatorPos ).Time(2.0f).Execute();
		
		yield return new WaitForSeconds(2.0f);

		yield break;
	}


	/*
	public IEnumerator VisualizeOLD(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerDivide : Visualize : " + current + " TO " + target);
		
		// TODO: deselect characters if need be
		
		// Denominator of StopFraction needs to become the Numerator
		// Numerator of StopFraction needs to become the Denominator
		// after that... we need to multiply as usual -> Re-use VisualizerMultiply for now
		
		// to switch the two, we need to pull a trick
		// this is because of the two-camera setup
		// we cannot actually re-use the NumberRenderer: we need to make a copy and fake the movements so that it LOOKS like it's moving correctly
		
		
		
		FractionRenderer Switcher = current.StopFraction.Renderer; 
		
		// move original renderers down and up
		// moving up is a longer way than moving down!
		Vector3 numeratorUp = Switcher.Numerator.transform.position;
		Vector3 numeratorDown = numeratorUp - new Vector3(0, 6, 0); // TODO: update these "magic numbers"
		
		Vector3 denominatorDown = Switcher.Denominator.transform.position;
		Vector3 denominatorUp = denominatorDown + new Vector3(0, 8, 0);// TODO: update these "magic numbers"
		
		Switcher.Numerator.gameObject.MoveTo( numeratorDown ).Time (2.0f).Execute();
		Switcher.Denominator.gameObject.MoveTo( denominatorUp ).Time (2.0f).Execute(); 
		
		// create new renderers (don't couple them to the correct Numbers yet)
		// TODO: check if these get the correct parent (i.e. WORLD, etc.)
		NumberRenderer newNumerator = RendererFactory.use.CreateRenderer( new Number(Switcher.Denominator.Number.Value, null, false) );
		NumberRenderer newDenominator = RendererFactory.use.CreateRenderer( new Number(Switcher.Numerator.Number.Value, null, true) );
		
		newNumerator.transform.position = numeratorDown;
		newNumerator.gameObject.MoveTo( numeratorUp ).Time (2.0f).Execute();
		
		newDenominator.transform.position = denominatorUp;
		newDenominator.gameObject.MoveTo( denominatorDown ).Time (2.0f).Execute();
		
		yield return new WaitForSeconds(2.0f);
		
		// now, there are nicely switched visually
		// but in the back-end, the renderers are not coupled to the correct data-structures!!!
		
		// 1. actually switch the values in the datastructure
		Number temp = current.StopFraction.Numerator;
		current.StopFraction.Numerator = current.StopFraction.Denominator;
		current.StopFraction.Denominator = temp;
		
		// 2. remove the old renderers
		RendererFactory.use.FreeRenderer( Switcher.Numerator );
		RendererFactory.use.FreeRenderer( Switcher.Denominator );
		
		// 3. make sure the new renderers now point to the correct numbers and vice versa
		newNumerator.Number = current.StopFraction.Numerator;
		newDenominator.Number = current.StopFraction.Denominator;
		
		current.StopFraction.Numerator.Renderer = newNumerator;
		current.StopFraction.Denominator.Renderer = newDenominator;
		
		
		
		yield return new WaitForSeconds(1.0f);
		
		// Now, we need to multiply the two fractions with eachother
		
		// Note: no need to go through the multiplay IOperation itself: the correct result was calculated by the divide Operation
		// we only need to visualize it the same way we do a multiplication
		IOperationVisualizer mulViz = MathManager.use.GetVisualizer( FR.OperationType.MULTIPLY );
		
		yield return LugusCoroutines.use.StartRoutine( mulViz.Visualize(current, target) ).Coroutine;
		
		// 4. ...
		// 5. Profit!
		
		yield return new WaitForSeconds(0.3f);
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerDivide : finished");
		yield break; 
	}
	*/
}
