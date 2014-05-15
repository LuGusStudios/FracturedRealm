using UnityEngine;
using System.Collections;

public class OperationVisualizerSimplify : IOperationVisualizer 
{
	public OperationVisualizerSimplify() 
	{
		Reset();
	}
	
	public override void Reset()
	{
		this.type = FR.OperationType.SIMPLIFY; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerSimplify : Visualize : " + current + " TO " + target);
		
		// TODO: deselect characters if need be
		
		FractionRenderer fraction = current.StartFraction.Renderer;

		fraction.Animator.SpawnEffect( FR.Target.BOTH, FR.EffectType.FIRE_HIT );
		
		yield return new WaitForSeconds(0.2f);
		
		if( fraction.Fraction.Numerator.Value != 0 )
		{
			fraction.Fraction.Numerator.Value = target.StartFraction.Numerator.Value;
			fraction.Numerator.NumberValueChanged();
		}
		
		if( fraction.Fraction.Denominator.Value != 0 )
		{
			fraction.Fraction.Denominator.Value = target.StartFraction.Denominator.Value;
			fraction.Denominator.NumberValueChanged();
		}

		
		//CharacterFactory.use.ReplaceRenderer( fraction.Numerator, fraction.Numerator.Number);
		//CharacterFactory.use.ReplaceRenderer( fraction.Denominator, fraction.Denominator.Number);
		
		yield return new WaitForSeconds(0.3f);
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSimplify : finished");
		yield break; 
	}
}
