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
		this.lastMessage = FR.OperationMessage.None;

		bool result = false;

		if( state.StopFraction == null )
		{
			// double value cannot surpass 12
			if( state.StartFraction.Numerator.Value * 2 <= 12 &&
			   state.StartFraction.Denominator.Value * 2 <= 12 )
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
			Debug.LogError("OperationDouble:AcceptState : stopFraction was filled in... weird!");

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
