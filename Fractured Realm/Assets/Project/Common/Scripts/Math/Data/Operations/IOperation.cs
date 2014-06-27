using UnityEngine;
using System.Collections;
using System;

public class OperationState
{
	public OperationState( FR.OperationType type, Fraction start, Fraction stop )
	{
		StartFraction = start;
		StopFraction = stop;
		
		Type = type;
	}
	
	public OperationState CopyData()
	{
		OperationState output = new OperationState(this.Type, null, null);
		
		if( this.StartFraction != null )
			output.StartFraction = StartFraction.CopyData();
		if( this.StopFraction != null )
			output.StopFraction = StopFraction.CopyData();
		
		if( this.StartNumber != null )
			output.StartNumber = StartNumber.CopyData();
		if( this.StopNumber != null )
			output.StopNumber = StopNumber.CopyData();
		
		return output;
	}
	
	protected FR.OperationType _type = FR.OperationType.NONE;
	public FR.OperationType Type
	{
		get{ return _type; }
		set{ _type = value; } 
	}
	
	protected Fraction _startFraction = null;
	public Fraction StartFraction
	{
		get{ return _startFraction; }
		set{ _startFraction = value; }
	}
	
	protected Number _startNumber = null;
	public Number StartNumber
	{
		get{ return _startNumber; }
		set
		{ 
			_startNumber = value; 
			if( _startNumber != null )
				_startFraction = _startNumber.Fraction;
		}
	}
	
	protected Fraction _stopFraction = null;
	public Fraction StopFraction
	{
		get{ return _stopFraction; }
		set{ _stopFraction = value; }
	}
	
	public Number _stopNumber = null;
	public Number StopNumber
	{
		get{ return _stopNumber; }
		set
		{ 
			_stopNumber = value; 
			if( _stopNumber != null )
				_stopFraction = _stopNumber.Fraction;
		}
	}
	
	public override string ToString ()
	{ 
		return string.Format ("[OperationState: Type={0}, StartFraction={1}, StartNumber={2}, StopFraction={3}, StopNumber={4}]", Type, StartFraction, ((StartNumber != null) ? "" + StartNumber.Value : "?"), StopFraction, ((StopNumber != null) ? "" + StopNumber.Value : "?"));
	}
}

namespace FR
{

	public enum OperationType
	{
		NONE = -1,
		ADD = 1,
		SUBTRACT = 2,
		MULTIPLY = 3,
		DIVIDE = 4,
		DOUBLE = 5,
		SIMPLIFY = 6
	}

	[Flags]
	public enum OperationMessage
	{
		None = 0,

		Error_Requires2Fractions = 1, // just 1 target set and tried to execute
		Error_Requires1Fraction = 128, // 2 targets set but just 1 needed (shouldnt happen, but can never be too carefull :))
		Error_IdenticalTargets = 2, // startFraction == stopFraction
		Error_UnsimilarTargets = 4, // ! AreAlike() -> not both have numerator/denominator or same combination of both

		Error_DenominatorsNotEqual = 8, // both denominators aren't equal. NEcessary for + and -
		Error_RequiresFullFractions = 16, // divide requires both targets to be full fractions (numerator + denominator of both are non zero)

		Error_ResultTooLarge = 32, // results should never be > 12 in this setup
		Error_SimplificationImpossible = 64, // ex. 5/3 cannot be simplified
		Error_RequiresEvenNumbers = 256, // ex. 5/3 cannot be simplified

		Errors = 511, // convenience value to check if message is an error. if( OperationMessage.Errors & myMessage == myMessage ) { error } NOTE: check for .None separately!!!



	}
}

public class IOperation
{
	public IOperation()
	{
		
	}

	public static string OperationTypeToString(FR.OperationType type)
	{
		if( type == FR.OperationType.ADD )
		{
			return "+";
		}
		else if( type == FR.OperationType.SUBTRACT )
		{
			return "-";
		}
		else if( type == FR.OperationType.MULTIPLY )
		{
			return "x";
		}
		else if( type == FR.OperationType.DIVIDE )
		{
			return "/";
		}
		else if( type == FR.OperationType.SIMPLIFY )
		{
			return "/2";
		}
		else if( type == FR.OperationType.DOUBLE )
		{
			return "x2";
		}

		return "" + type;
	}

	public FR.OperationMessage lastMessage = FR.OperationMessage.None;


	public FR.OperationType type = FR.OperationType.NONE;
	
	public virtual bool AcceptState( OperationState state ){ return false; }
	public virtual OperationState Process( OperationState state ){ return state.CopyData(); }
	
	public virtual bool RequiresTwoFractions(){ return true; }
}
