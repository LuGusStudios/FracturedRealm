using UnityEngine;
using System.Collections;

public class OperationSimplify : IOperation 
{
	public OperationSimplify() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.SIMPLIFY; 
	}
	
	public override bool AcceptState(OperationState state)
	{
		if( state.StopFraction == null )
		{
			// both have to be even numbers!
			if( state.StartFraction.Numerator.Value % 2 == 0 &&
				state.StartFraction.Denominator.Value % 2 == 0 )
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			Debug.LogError("OperationSimplify:AcceptState : stopFraction was filled in... weird!");
			return false;
		}  
	}
	
	public override OperationState Process(OperationState state)
	{
		Debug.Log("OperationSimplify:Process : " + state.StartFraction ); 
		
		
		OperationState outcome = new OperationState( this.type, state.StartFraction.CopyData(), null ); 
		
		outcome.StartFraction.Numerator.Value /= 2;
		outcome.StartFraction.Denominator.Value /= 2;
		
		Reset();
		return outcome;
	}
	
	
	public override bool RequiresTwoFractions()
	{ 
		return false; 
	}
}
