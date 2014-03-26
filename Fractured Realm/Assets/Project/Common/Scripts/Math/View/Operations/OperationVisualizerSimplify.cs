using UnityEngine;
using System.Collections;

public class OperationVisualizerSimplify : IOperationVisualizer 
{
	public OperationVisualizerSimplify() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.SIMPLIFY; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerSimplify : Visualize : " + current + " TO " + target);
		
		// TODO: deselect characters if need be
		
		FractionRenderer fraction = current.StartFraction.Renderer;
		
		Effect[] hits = EffectFactory.use.CreateEffects( FR.EffectType.FIRE_HIT );
		hits[0].transform.position = fraction.Numerator.transform.position + new Vector3(0,50,-100);
		hits[1].transform.position = fraction.Denominator.transform.position + new Vector3(0,50,-100);
		
		yield return new WaitForSeconds(0.2f);
		
		fraction.Numerator.Number.Value = target.StartFraction.Numerator.Value;
		fraction.Denominator.Number.Value = target.StartFraction.Denominator.Value;
		
		fraction.Numerator.NumberValueChanged();
		fraction.Denominator.NumberValueChanged();
		
		//CharacterFactory.use.ReplaceRenderer( fraction.Numerator, fraction.Numerator.Number);
		//CharacterFactory.use.ReplaceRenderer( fraction.Denominator, fraction.Denominator.Number);
		
		yield return new WaitForSeconds(0.3f);
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSimplify : finished");
		yield break; 
	}
}
