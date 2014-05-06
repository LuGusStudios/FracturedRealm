using UnityEngine;
using System.Collections;

public class OperationDivide : IOperation 
{
	public OperationDivide() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.DIVIDE; 
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
			result =  AcceptOperation(state); 
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
		Debug.Log ("OperationDivide:AcceptOperation : " + state);
		
		// state needs a StopFraction!
		if( state.StopFraction == null )
		{
			this.lastMessage = FR.OperationMessage.Error_Requires2Fractions;
			return false;
		}
			
		// don't divide the same fraction by itself
		if( state.StartFraction == state.StopFraction )
		{
			this.lastMessage = FR.OperationMessage.Error_IdenticalTargets;
			return false;
		}

		// maximum resulting value is 12
		if( state.StopFraction.Denominator.Value * state.StartFraction.Numerator.Value > 12 ||
		    state.StopFraction.Numerator.Value * state.StartFraction.Denominator.Value > 12 )
		{
			this.lastMessage = FR.OperationMessage.Error_ResultTooLarge;
			return false;
		}

		
		if( !Fraction.AreAlike( state.StartFraction, state.StopFraction ) )
		{
			this.lastMessage = FR.OperationMessage.Error_UnsimilarTargets;
			return false;
		}

		// can only divide "full" fractions : no 0 values
		if( state.StartFraction.Numerator.Value == 0 || state.StartFraction.Denominator.Value == 0 ||
		    state.StopFraction.Numerator.Value == 0 || state.StopFraction.Denominator.Value == 0 )
		{
			this.lastMessage = FR.OperationMessage.Error_RequiresFullFractions;
			return false;
		}

		return true;
	} 
	
	public override OperationState Process(OperationState state)
	{
		Debug.Log("OperationDivide:Process : " + state.StartFraction + " + " + state.StopFraction); 
		
		
		OperationState outcome = new OperationState( this.type, state.StartFraction.CopyData(), state.StopFraction.CopyData() ); 
		
		// invert numerator and denominator of StopFraction
		int temp = outcome.StopFraction.Numerator.Value; 
		outcome.StopFraction.Numerator.Value = outcome.StopFraction.Denominator.Value;
		outcome.StopFraction.Denominator.Value = temp;
		
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
