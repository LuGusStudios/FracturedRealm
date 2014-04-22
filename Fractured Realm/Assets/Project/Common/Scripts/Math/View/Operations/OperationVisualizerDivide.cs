using UnityEngine;
using System.Collections;

public class OperationVisualizerDivide : IOperationVisualizer 
{
	public OperationVisualizerDivide() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.DIVIDE; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
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
		NumberRenderer newNumerator = CharacterFactory.use.CreateRenderer( new Number(Switcher.Denominator.Number.Value, null, false) );
		NumberRenderer newDenominator = CharacterFactory.use.CreateRenderer( new Number(Switcher.Numerator.Number.Value, null, true) );
		
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
		CharacterFactory.use.FreeRenderer( Switcher.Numerator );
		CharacterFactory.use.FreeRenderer( Switcher.Denominator );
		
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
}
