using UnityEngine;
using System.Collections;

public class OperationAdd : IOperation 
{
	public OperationAdd() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.ADD; 
	}
	
	public override bool AcceptState(OperationState state)
	{
		if( state.StartFraction == null )
			return AcceptStart(state);
		else if( state.StopFraction == null )
			return true;
		else
			return AcceptStop(state);  
	}
	
	protected bool AcceptStart(OperationState state)
	{
		Debug.Log ("OperationAdd:AcceptStart : " + state);
		
		// Add can only start at the nominator
		//if( !state.StartNumber.IsNumerator )
		//	return false;
		
		return true;
	}
	
	protected bool AcceptStop(OperationState state)
	{
		Debug.Log ("OperationAdd:AcceptStop : " + state);
		
		// state needs a StopFraction!
		if( state.StopFraction == null )
			return false;
			
		// don't add the same fraction to itself
		if( state.StartFraction == state.StopFraction )
			return false;
		
		// only add numerators
		//if( !state.StopNumber.IsNumerator )
		//	return false;
		
		// only if they are actually both fractions
		/*
		if( state.StartFraction.Denominator == null && state.StopFraction.Denominator != null )
			return false;
		if( state.StartFraction.Denominator != null && state.StopFraction.Denominator == null )
			return false;
		*/
		
		if( !Fraction.AreAlike( state.StartFraction, state.StopFraction ) )
			return false;
		
		// if we do have both fractions 
		//if( state.StartFraction.Denominator != null && state.StopFraction.Denominator != null )
		//{
			// only allow of the Denominators are the same
			if( state.StartFraction.Denominator.Value != state.StopFraction.Denominator.Value )
				return false;
		//}
		 
		return true;
	} 
	
	public override OperationState Process(OperationState state)
	{
		Debug.Log("OperationAdd:Process : " + state.StartFraction + " + " + state.StopFraction); 
		
		
		OperationState outcome = new OperationState( this.type, state.StartFraction.CopyData(), state.StopFraction.CopyData() ); 
		
		// add the numerators (denominators stay the same)
		// the second fraction effectively disappears this way
		outcome.StartFraction.Numerator.Value += outcome.StopFraction.Numerator.Value;
		outcome.StopFraction = null;
		
		Reset();
		return outcome;
	}
	
	
	public override bool RequiresTwoFractions()
	{ 
		return true; 
	}
}
