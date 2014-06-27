using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class Fraction 
{
	// when using the class purely as data structure
	public Fraction(int numeratorValue, int denominatorValue)
	{
		Fill ( numeratorValue, denominatorValue );
	}


	public Fraction( Number numerator, Number demoninator )
	{
		this.Numerator = numerator;
		this.Denominator = demoninator;
		
		this.Numerator.Fraction = this;
		this.Denominator.Fraction = this;
		
		this.Numerator.IsNumerator = true;
		this.Numerator.IsDenominator = false;
		
		this.Denominator.IsNumerator = false;
		this.Denominator.IsDenominator = true;
	}

	public Fraction( string data, FR.Target composition = FR.Target.BOTH )
	{
		bool error = true;
		if( data.Contains("/") )
		{
			string[] split = data.Split('/');

			if( split.Length == 2 )
			{
				error = false;
				Fill ( split[0], split[1] );
			}
		}
		else if( data.Contains("\\") )
		{
			string[] split = data.Split('\\');
			
			if( split.Length == 2 )
			{
				error = false;
				Fill ( split[0], split[1] );
			}
		}
		else
		{
			// no / found : if composition is just 1 of both, this is possible
			if( composition == FR.Target.BOTH )
			{
				Debug.LogError("Fraction : string " + data + " doesn't contain / but is supposed to be for composition BOTH..");
				Fill ( -1, -1 );
			}
			else
			{
				if( composition == FR.Target.NUMERATOR )
				{
					error = false;
					Fill ( data, "" + 0 );
				}
				else if( composition == FR.Target.DENOMINATOR )
				{
					error = false;
					Fill ( "" + 0, data );
				}
				else
				{
					Debug.LogError("Fraction: invalid composition! " + composition);
					Fill ( -1, -1 );
				}
			}
		}

		// allow users to switch composition without switching fraction values
		if( composition == FR.Target.NUMERATOR )
			this.Denominator.Value = 0;
		else if( composition == FR.Target.DENOMINATOR )
			this.Numerator.Value = 0;


		if( error )
		{
			Debug.LogError("Fraction: string " + data + " is not wellformed. Has to be of format x / y or x \\ y. Or composition needs to be 1 of both (" + composition + ")");
		}
	}

	protected void Fill( string numeratorValue, string denominatorValue )
	{
		int num = -1;
		int denom = -1;
		try
		{
			num = int.Parse(numeratorValue.Trim ());
		}
		catch(Exception e)
		{
			Debug.LogError("Fraction: numeratorValue " + numeratorValue + " is not an int!");
			num = -1;
		}
		
		try
		{
			denom = int.Parse(denominatorValue.Trim ());
		}
		catch(Exception e)
		{
			Debug.LogError("Fraction: denominatorValue " + denominatorValue + " is not an int!");
			denom = -1;
		}

		Fill ( num, denom );
	}

	protected void Fill( int numeratorValue, int denominatorValue )
	{
		this.Numerator 		= new Number(numeratorValue, this, true);
		this.Denominator 	= new Number(denominatorValue, this, false);
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

	public void Destroy()
	{
		if( this.HasNumerator() )
		{
			GameObject.Destroy( this.Numerator.Renderer );
		}

		if( this.HasDenominator() )
		{
			GameObject.Destroy( this.Denominator.Renderer );
		}
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

	public bool HasNumerator()
	{
		if( _numerator == null )
			return false;

		return _numerator.Value != 0;
	}
	
	public bool HasDenominator()
	{
		if( _denominator == null )
			return false;
		
		return _denominator.Value != 0;
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
