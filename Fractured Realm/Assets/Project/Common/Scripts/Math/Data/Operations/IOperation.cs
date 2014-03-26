using UnityEngine;
using System.Collections;

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
		NONE,
		ADD,
		SUBTRACT,
		MULTIPLY,
		DIVIDE,
		DOUBLE,
		SIMPLIFY
	}
}

public class IOperation
{
	public IOperation()
	{
		
	}
	
	public FR.OperationType type = FR.OperationType.NONE;
	
	public virtual bool AcceptState( OperationState state ){ return false; }
	public virtual OperationState Process( OperationState state ){ return state.CopyData(); }
	
	public virtual bool RequiresTwoFractions(){ return true; }
}
