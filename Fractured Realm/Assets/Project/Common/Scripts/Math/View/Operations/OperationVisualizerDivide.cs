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
		NumberRenderer newNumerator = RendererFactory.use.CreateRenderer( new Number(Switcher.Denominator.Number.Value, null, false) );
		NumberRenderer newDenominator = RendererFactory.use.CreateRenderer( new Number(Switcher.Numerator.Number.Value, null, true) );

		newNumerator.transform.position = Switcher.Numerator.transform.position - new Vector3(0, 6.0f, 0);// TODO: update these "magic numbers"
		newDenominator.transform.position = Switcher.Denominator.transform.position + new Vector3(0, 8, 0);// TODO: update these "magic numbers"


		// need to start both animations at the same time
		FR.Animation denominatorAnimation = this.GetRandomNextAnimation();
		OperationVisualizerDivide denominatorVisualizer = null;
		if( denominatorAnimation == FR.Animation.NONE )
		{
			Debug.LogError("VisualizeAnimation:Divide : no denominator animation found for descender " + this.AnimationType() );
			denominatorVisualizer = new OperationVisualizerDivide();
		}
		else
		{
			denominatorVisualizer = (OperationVisualizerDivide) FRAnimations.use.animations[ (int) denominatorAnimation ].visualizer;
		}

		denominatorVisualizer.animationPartTarget = Switcher.Numerator.transform.position;
		
		this.blackHoleController = new BlackHoleController();
		denominatorVisualizer.blackHoleController = this.blackHoleController;
		
		float myTime = TimeToTransition();
		float denominatorTime = denominatorVisualizer.TimeToTransition();
		
		ILugusCoroutineHandle denominatorRoutine = null;
		
		if( myTime - denominatorTime < 0.0f )
		{
			// denominator takes longer to reach transition. Start first, wait appropriate time, and continue ours
			
			//denominatorRoutine = LugusCoroutines.use.StartRoutine( denominatorVisualizer.VisualizeDenominatorPart( Switcher, newNumerator, newDenominator, 0.0f, myTime, Vector3.zero ) );
			denominatorRoutine = LugusCoroutines.use.StartRoutine( denominatorVisualizer.VisualizeAnimationPart( FR.Target.DENOMINATOR, Switcher.Denominator, newNumerator) );
			
			denominatorRoutine.Claim();
			
			yield return new WaitForSeconds( denominatorTime - myTime );
		}
		else
		{
			// we take longer than denominator. 
			//denominatorRoutine = LugusCoroutines.use.StartRoutine( denominatorVisualizer.VisualizeDenominatorPart( Switcher, newNumerator, newDenominator, myTime - denominatorTime, myTime, Vector3.zero ) );
			denominatorVisualizer.animationPartDelay = myTime - denominatorTime;
			denominatorRoutine = LugusCoroutines.use.StartRoutine( denominatorVisualizer.VisualizeAnimationPart( FR.Target.DENOMINATOR, Switcher.Denominator, newNumerator) );
			denominatorRoutine.Claim();
		}


		this.animationPartTarget = Switcher.Denominator.transform.position;
		yield return LugusCoroutines.use.StartRoutine( VisualizeAnimationPart(FR.Target.NUMERATOR, Switcher.Numerator, newDenominator) ).Coroutine;



		while( denominatorRoutine.Running ) 
			yield return null;
		
		denominatorRoutine.Release();


		this.blackHoleController.FreeAll();
		this.blackHoleController = null;
		denominatorVisualizer.blackHoleController = null;


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
	 
	public float animationPartDelay = 0.0f;
	public Vector3 animationPartTarget;
	public BlackHoleController blackHoleController = null;

	public override IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer original, NumberRenderer second)
	{
		// part indicates if we're moving Numerator or Denominator
		// original is the original renderer that needs to move off-screen and later is destroyed
		// second is the copy of the original that will arrive below (or above)  ("newNumerator" or "newDenominator")
		
		// NOTE: normally, this switch isn't needed, as descend and ascend are separate
		// this is just for demonstration purposes
		if( part.HasNumerator() )
		{
			if( animationPartDelay > 0.0f )
				yield return new WaitForSeconds( animationPartDelay );
			
			Vector3 numeratorPos = original.transform.position;
			original.transform.position = numeratorPos;
			
			Vector3 numeratorOffScreen = numeratorPos - new Vector3(0, 6, 0); // TODO: update these "magic numbers"
			
			original.gameObject.MoveTo( numeratorOffScreen ).Time( 2.0f ).Execute();
			second.gameObject.MoveTo( animationPartTarget ).Time(2.0f).Execute();
			
			yield return new WaitForSeconds(2.0f);
		}
		else // denominator
		{
			if( animationPartDelay > 0.0f )
				yield return new WaitForSeconds( animationPartDelay );
			
			Vector3 denominatorPos = original.transform.position;
			original.transform.position = denominatorPos;

			Vector3 denominatorOffScreen = denominatorPos + new Vector3(0, 8, 0); // TODO: update these "magic numbers"

			original.gameObject.MoveTo( denominatorOffScreen ).Time( 2.0f ).Execute();
			second.gameObject.MoveTo( animationPartTarget ).Time(2.0f).Execute();
			
			yield return new WaitForSeconds(2.0f);
		}
		
		yield break; 
	}

	/*
	public override IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer original, NumberRenderer copy)
	{
		// part indicates if we're moving Numerator or Denominator
		// original is the original renderer that needs to move off-screen and later is destroyed
		// copy is the copy of the original that will arrive below  ("newNumerator" or "newDenominator")

		// NOTE: normally, this switch isn't needed, as descend and ascend are separate
		// this is just for demonstration purposes
		if( part.HasNumerator() )
		{
			if( animationPartDelay > 0.0f )
				yield return new WaitForSeconds( animationPartDelay );

			Vector3 numeratorPos = original.transform.position;
			Vector3 numeratorOffScreen = numeratorPos - new Vector3(0, 6, 0); // TODO: update these "magic numbers"
			
			original.transform.position = numeratorPos;
			copy.transform.position = numeratorOffScreen;
			
			original.gameObject.MoveTo( numeratorOffScreen ).Time( 2.0f ).Execute();
			copy.gameObject.MoveTo( numeratorPos ).Time(2.0f).Execute();
			
			yield return new WaitForSeconds(2.0f);
		}
		else // denominator
		{
			if( animationPartDelay > 0.0f )
				yield return new WaitForSeconds( animationPartDelay );
			
			Vector3 denominatorPos = original.transform.position;
			Vector3 denominatorOffScreen = denominatorPos + new Vector3(0, 8, 0); // TODO: update these "magic numbers"
			
			original.transform.position = denominatorPos;
			copy.transform.position = denominatorOffScreen;
			
			original.gameObject.MoveTo( denominatorOffScreen ).Time( 2.0f ).Execute();
			copy.gameObject.MoveTo( denominatorPos ).Time(2.0f).Execute();
			
			yield return new WaitForSeconds(2.0f);
		}

		yield break; 
	}
	*/

	/*
	public virtual IEnumerator VisualizeNumeratorPart( FractionRenderer Switcher, NumberRenderer newNumerator, NumberRenderer newDenominator )
	{
		//Debug.LogError("OperationVisualizerDivide_Descend:VisualizeAnimationPart : this should be overwritten in the correct Descend animation!");



		Vector3 numeratorPos = Switcher.Numerator.transform.position;
		Vector3 numeratorOffScreen = numeratorPos - new Vector3(0, 6, 0); // TODO: update these "magic numbers"

		Switcher.Numerator.transform.position = numeratorPos;
		newNumerator.transform.position = numeratorOffScreen;

		Switcher.Numerator.gameObject.MoveTo( numeratorOffScreen ).Time( 2.0f ).Execute();
		newNumerator.gameObject.MoveTo( numeratorPos ).Time(2.0f).Execute();

		yield return new WaitForSeconds(2.0f);


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
	*/
	
}
