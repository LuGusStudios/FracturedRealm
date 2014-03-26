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
		Debug.Log ("OperationMultiply:AcceptOperation : " + state);
		
		// state needs a StopFraction!
		if( state.StopFraction == null )
		{
			Debug.LogError("OperationMultiply : StopFraction was null");
			return false;
		}
			
		// don't multiply the same fraction by itself
		if( state.StartFraction == state.StopFraction )
		{
			Debug.LogError("OperationMultiply : StopFraction was same as StartFraction");
			return false;
		}
		
		// TODO: no more than 12 as uitkomst! 
		
		if( !Fraction.AreAlike( state.StartFraction, state.StopFraction ) )
			return false;
		 
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
