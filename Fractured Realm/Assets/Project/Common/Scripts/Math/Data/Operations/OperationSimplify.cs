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
		this.lastMessage = FR.OperationMessage.None;

		bool result = false;

		if( state.StopFraction == null )
		{
			// both have to be even numbers!
			if( state.StartFraction.Numerator.Value % 2 == 0 &&
				state.StartFraction.Denominator.Value % 2 == 0 )
			{
				this.lastMessage = FR.OperationMessage.None;
				result = true;
			}
			else
			{
				this.lastMessage = FR.OperationMessage.Error_ResultTooLarge;
				result = false;
			}
		}
		else
		{
			Debug.LogError("OperationSimplify:AcceptState : stopFraction was filled in... weird!");

			this.lastMessage = FR.OperationMessage.Error_Requires1Fraction;
			result = false;
		}  

		
		if( lastMessage != FR.OperationMessage.None )
		{
			if( lastMessage != FR.OperationMessage.Error_Requires2Fractions )
				Debug.LogError("OperationAdd:AcceptState : raised error : " + lastMessage);
		}
		
		return result;
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
