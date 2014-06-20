using UnityEngine;
using System;
using System.Collections;

// this class represents a Number inside of a Fraction
[System.Serializable]
public class Number 
{
	public delegate void ValueChangedHandler(int oldValue, int newValue);
	public ValueChangedHandler OnValueChanged;

	
	public Number(int val)
	{
		this.Value = val;	 
	}
	
	public Number( int val, Fraction parent, bool isNumerator )
	{
		this.Value = val;
		this.Fraction = parent;
		this.IsNumerator = isNumerator; 
	}
	
	public Number CopyData()
	{
		Number output = new Number(_value, _fraction, _isNumerator);
		return output;
	}
	
	[SerializeField]
	protected int _value = -666;
	public int Value
	{
		get{ return _value; } 
		set
		{ 
			int oldValue = _value;
			_value = value;
			
			if( OnValueChanged != null )
				OnValueChanged( oldValue, _value ); 
		} 
	}
	
	public int ValueTo6
	{
		get
		{
			if( _value < 7 )
				return _value;
			else
				return 6;
		}
	}
	
	public int ValuePast6
	{
		get
		{
			if( _value < 7 )
				return 0;
			else
				return _value - 6;
		}
	}

	public FR.Target ToTarget()
	{
		if( this.IsNumerator )
			return FR.Target.NUMERATOR;
		else
			return FR.Target.DENOMINATOR;
	}
	
	// the parent fraction
	[SerializeField]
	protected Fraction _fraction = null;
	public Fraction Fraction
	{
		get{ return _fraction; }
		set{ _fraction = value; }
	}
	
	[SerializeField]
	protected bool _isNumerator = true;
	
	public bool IsNumerator
	{
		get{ return _isNumerator; } 
		set{ _isNumerator = value; }
	}
	
	public bool IsDenominator
	{
		get{ return !_isNumerator; }
		set{ _isNumerator = !value; }
	}
	
	public Number OtherNumber
	{
		get
		{
			if( Fraction == null )
			{
				return null;
			}
			else
			{
				if( IsNumerator )
					return Fraction.Denominator;
				else
					return Fraction.Numerator;
			}
		}
	}
	
	
	
	
	/*
	// A Number can be rendered with 2 characters at this moment
	// this is because we only have characters up to the number 6, while we want to display numbers up to 12
	protected Character[] _renderers = null;
	public Character[] Renderers
	{
		get{ return _renderers; } // TODO: test Renderers[0] = x; and see if it sticks! (pass by value vs reference!)
		set{ _renderers = value; }
	}
	
	private void InitiateRenderers()
	{
		if( _renderers != null )
			return;
		
		_renderers = new Character[2];
		_renderers[0] = null;
		_renderers[1] = null;
	}
	
	//returns the current character for interaction (smallest number)
	public Character Renderer
	{
		get
		{ 
			if( _renderers[1] == null )
				return _renderers[0];
			else
				return _renderers[1];
			
			//return _renderer; 
		}
		set
		{ 
			_renderers[0] = value;
			//_renderer = value; 
		}
	}
	*/
	
	public NumberRenderer _renderer = null;
	public NumberRenderer Renderer
	{
		get{ return _renderer; }
		set{ _renderer = value; }
	}
}
