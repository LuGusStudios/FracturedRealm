using UnityEngine;
using System.Collections;


public class IOperationVisualizer
{
	public IOperationVisualizer()
	{
		
	}
	
	public FR.OperationType type = FR.OperationType.NONE;
	
	public virtual IEnumerator Visualize(OperationState current, OperationState target){ yield break; }
	
	protected virtual void CheckOutcome(OperationState outcome, OperationState target)
	{
		if( outcome.StartFraction.Numerator.Value == target.StartFraction.Numerator.Value &&
			outcome.StartFraction.Denominator.Value == target.StartFraction.Denominator.Value )
		{
			Debug.Log ("IOPerationVisualiser:CheckOutcome : " + this.type + " : outcome validated :)");
		}
		else
		{
			Debug.LogError("IOperationVisualizer:CheckOutcome : " + this.type + " : incorrect outcome! " + outcome + " != " + target);
		}
	}
}
