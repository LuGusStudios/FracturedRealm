using UnityEngine;
using System.Collections;

public class OperationDouble : IOperation 
{
	public OperationDouble() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.DOUBLE;  
	}
	
	public override bool AcceptState(OperationState state)
	{
		if( state.StopFraction == null )
		{
			return true;
		}
		else
		{
			Debug.LogError("OperationDouble:AcceptState : stopFraction was filled in... weird!");
			return false;
		}  
	}
	
	public override OperationState Process(OperationState state)
	{
		Debug.Log("OperationDouble:Process : " + state.StartFraction ); 
		
		
		OperationState outcome = new OperationState( this.type, state.StartFraction.CopyData(), null ); 
		
		outcome.StartFraction.Numerator.Value *= 2;
		outcome.StartFraction.Denominator.Value *= 2;
		
		Reset();
		return outcome;
	}
	
	
	public override bool RequiresTwoFractions()
	{ 
		return false; 
	}
}
