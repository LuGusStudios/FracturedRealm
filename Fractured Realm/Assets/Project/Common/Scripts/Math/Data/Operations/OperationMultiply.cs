using UnityEngine;
using System.Collections;

public class OperationMultiply : IOperation 
{
	public OperationMultiply() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.MULTIPLY; 
	}
	
	public override bool AcceptState(OperationState state)
	{
		this.lastMessage = FR.OperationMessage.None;

		bool result = false;

		if( state.StartFraction == null )
			result = true;
		else if( state.StopFraction == null )
			result = true;
		else
		{
			result = AcceptOperation(state); 
		}


		if( lastMessage != FR.OperationMessage.None )
		{
			if( lastMessage != FR.OperationMessage.Error_Requires2Fractions )
				Debug.LogError("OperationAdd:AcceptState : raised error : " + lastMessage);
		}
		
		return result;
	}
	
	protected bool AcceptOperation(OperationState state)
	{
		Debug.Log ("OperationMultiply:AcceptOperation : " + state);
		
		// state needs a StopFraction!
		if( state.StopFraction == null )
		{
			Debug.LogError("OperationMultiply : StopFraction was null");
			this.lastMessage = FR.OperationMessage.Error_Requires2Fractions;
			return false;
		}
			
		// don't multiply the same fraction by itself
		if( state.StartFraction == state.StopFraction )
		{
			Debug.LogError("OperationMultiply : StopFraction was same as StartFraction");
			this.lastMessage = FR.OperationMessage.Error_IdenticalTargets;
			return false;
		}
		
		// maximum resulting value is 12
		if( state.StopFraction.Numerator.Value * state.StartFraction.Numerator.Value > 12 ||
		    state.StopFraction.Denominator.Value * state.StartFraction.Denominator.Value > 12 )
		{
			this.lastMessage = FR.OperationMessage.Error_ResultTooLarge;
			return false;
		}
		
		if( !Fraction.AreAlike( state.StartFraction, state.StopFraction ) )
		{
			this.lastMessage = FR.OperationMessage.Error_UnsimilarTargets;
			return false;
		}

		return true;
	} 
	
	public override OperationState Process(OperationState state)
	{
		Debug.Log("OperationMultiply:Process : " + state.StartFraction + " + " + state.StopFraction); 
		
		
		OperationState outcome = new OperationState( this.type, state.StartFraction.CopyData(), state.StopFraction.CopyData() ); 
		
		
		// multiply numerators and denominators
		outcome.StartFraction.Numerator.Value *= outcome.StopFraction.Numerator.Value;
		outcome.StartFraction.Denominator.Value *= outcome.StopFraction.Denominator.Value;
		outcome.StopFraction = null;
		
		Reset();
		return outcome;
	}
	
	
	public override bool RequiresTwoFractions()
	{ 
		return true; 
	}
}
