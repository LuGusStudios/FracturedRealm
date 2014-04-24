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
		if( state.StartFraction == null )
			return true;
		else if( state.StopFraction == null )
			return true;
		else
		{
			return AcceptOperation(state); 
		}
	}
	
	protected bool AcceptOperation(OperationState state)
	{
		Debug.Log ("OperationDivide:AcceptOperation : " + state);
		
		// state needs a StopFraction!
		if( state.StopFraction == null )
			return false;
			
		// don't divide the same fraction by itself
		if( state.StartFraction == state.StopFraction )
			return false;
		
		// TODO: no more than 12 as uitkomst! 
		
		if( !Fraction.AreAlike( state.StartFraction, state.StopFraction ) )
			return false;

		// can only divide "full" fractions : no 0 values
		if( state.StartFraction.Numerator.Value == 0 || state.StartFraction.Denominator.Value == 0 ||
		    state.StopFraction.Numerator.Value == 0 || state.StopFraction.Denominator.Value == 0 )
		{
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
