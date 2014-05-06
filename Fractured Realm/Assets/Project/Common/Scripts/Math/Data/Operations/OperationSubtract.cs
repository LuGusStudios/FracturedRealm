using UnityEngine;
using System.Collections;

public class OperationSubtract : IOperation 
{
	public OperationSubtract() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.SUBTRACT; 
	}
	
	public override bool AcceptState(OperationState state)
	{
		this.lastMessage = FR.OperationMessage.None;

		bool result = false;

		if( state.StartFraction == null )
			result = AcceptStart(state);
		else if( state.StopFraction == null )
			result = true;
		else
			result = AcceptStop(state);

		
		if( lastMessage != FR.OperationMessage.None )
		{
			if( lastMessage != FR.OperationMessage.Error_Requires2Fractions )
				Debug.LogError("OperationAdd:AcceptState : raised error : " + lastMessage);
		}
		
		return result;
	}
	
	protected bool AcceptStart(OperationState state)
	{
		Debug.Log ("OperationSubtract:AcceptStart : " + state);
		
		// Add can only start at the nominator
		//if( !state.StartNumber.IsNumerator )
		//	return false;
		
		return true;
	}
	
	protected bool AcceptStop(OperationState state)
	{
		Debug.Log ("OperationSubtract:AcceptStop : " + state);
		
		// state needs a StopFraction!
		if( state.StopFraction == null )
		{
			this.lastMessage = FR.OperationMessage.Error_Requires2Fractions;
			return false;
		}
			
		// don't add the same fraction to itself
		if( state.StartFraction == state.StopFraction )
		{
			this.lastMessage = FR.OperationMessage.Error_IdenticalTargets;
			return false;
		}
		
		if( !Fraction.AreAlike( state.StartFraction, state.StopFraction ) )
		{
			this.lastMessage = FR.OperationMessage.Error_UnsimilarTargets;
			return false;
		}

		if( state.StartFraction.Denominator.Value != state.StopFraction.Denominator.Value )
		{
			this.lastMessage = FR.OperationMessage.Error_DenominatorsNotEqual;
			return false;
		}
		 
		return true;
	} 
	
	public override OperationState Process(OperationState state)
	{
		Debug.Log("OperationSubtract:Process : " + state.StartFraction + " + " + state.StopFraction); 
		
		
		OperationState outcome = new OperationState( this.type, state.StartFraction.CopyData(), state.StopFraction.CopyData() ); 
		
		// subtract the lowest Numerator from the largest (denominators stay the same)
		// the second fraction effectively disappears this way
		if( outcome.StartFraction.Numerator.Value > outcome.StopFraction.Numerator.Value )
		{
			outcome.StartFraction.Numerator.Value -= outcome.StopFraction.Numerator.Value;
		}
		else 
		{
			outcome.StartFraction.Numerator.Value = outcome.StopFraction.Numerator.Value - outcome.StartFraction.Numerator.Value;
		}
			
		outcome.StopFraction = null;
		
		Reset();
		return outcome;
	}
	
	
	public override bool RequiresTwoFractions()
	{ 
		return true; 
	}
}
