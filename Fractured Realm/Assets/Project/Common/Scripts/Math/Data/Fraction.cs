using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class Fraction 
{
	// when using the class purely as data structure
	public Fraction(int numeratorValue, int denominatorValue)
	{
		this.Numerator 		= new Number(numeratorValue, this, true);
		this.Denominator 	= new Number(denominatorValue, this, false);
	}

	public Fraction( Number numerator, Number demoninator )
	{
		this.Numerator = numerator;
		this.Denominator = demoninator;
		
		this.Numerator.Fraction = this;
		this.Denominator.Fraction = this;
		
		this.Numerator.IsNumerator = true;
		this.Denominator.IsDenominator = true;
	}
	
	public Fraction CopyData()
	{
		Fraction output = new Fraction( _numerator.CopyData(), _denominator.CopyData() );
		return output;
	}
	
	
	// if both are the same
	// ex. both have numerators and denominators
	// ex. both have only numerators
	// ex. both have only denominators
	public static bool AreAlike(Fraction f1, Fraction f2)
	{
		if( f1.Numerator == null || f1.Denominator == null )
		{
			Debug.LogError("Fraction:AreAlike : f1 one of the fields is null! Shouldn't happen! Use .Value == 0 instead! " + f1);
			return false;
		}
		
		if( f2.Numerator == null || f2.Denominator == null )
		{
			Debug.LogError("Fraction:AreAlike : f2 one of the fields is null! Shouldn't happen! Use .Value == 0 instead! " + f2);
			return false;
		}
		
		// only denominators
		if( f1.Numerator.Value == 0 && f2.Numerator.Value == 0 &&
			f1.Denominator.Value != 0 && f2.Denominator.Value != 0 )
		{
			return true;
		}
		
		// only numerators
		if( f1.Denominator.Value == 0 && f2.Denominator.Value == 0 &&
			f1.Numerator.Value != 0 && f2.Numerator.Value != 0 )
		{
			return true;
		}
		
		// both have numerators and denominators
		if( f1.Denominator.Value != 0 && f2.Denominator.Value != 0 &&
			f1.Numerator.Value != 0 && f2.Numerator.Value != 0 )
		{
			return true;
		}
		
		Debug.LogError("Fraction:AreAlike : f1 and f2 are not alike... should not happen " + f1 + " != " + f2); 
		
		return false;
	}
	
	[SerializeField]
	protected Number _numerator = null;
	[SerializeField]
	protected Number _denominator = null;
	
	public Number Numerator
	{
		get{ return _numerator; }
		set{ _numerator = value; }
	}
	
	public Number Denominator
	{
		get{ return _denominator; }
		set{ _denominator = value; }
	}
	
	protected FractionRenderer _renderer = null;
	public FractionRenderer Renderer
	{
		get{ return _renderer; }
		set{ _renderer = value; }
	}
	
	public override string ToString ()
	{
		 return "" + Numerator.Value + "/" + Denominator.Value;
	}
}
