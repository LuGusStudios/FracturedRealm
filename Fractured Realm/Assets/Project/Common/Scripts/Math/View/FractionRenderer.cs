using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace FR
{
	// http://stackoverflow.com/questions/8447/enum-flags-attribute
	// http://www.dotnetperls.com/enum-flags
	// ex (FR.Target.NUMERATOR | FR.Target.ALL_CHARACTERS) means a function applies to both characters of the numerator
	// DO NOT CHANGE INT VALUES IF YOU DO NOT KNOW WHAT YOU'RE DOING
	[Flags]
	public enum Target
	{
		NONE = 0,
		NUMERATOR = 1,
		DENOMINATOR = 2, 
		BOTH = 3, // convenience value, same as NUMERATOR | DENOMINATOR. Intended that this is 3 and not a POW. Used to indicate the numbers up "above" 6 (the "main interaction characters" in the numbers)
		
		// for use when we have a Number with value > 6 
		// then the NumberRenderer contains 2 characters, of which only one is the "interactor" and the other is the bystander
		// usually, we want to address only interactor, so that is default mode
		// if we want to address both characters, we need to use the ALL_CHARACTERS flag
		ALL_CHARACTERS = 4,
		NUMERATOR_ALL = 5, // convenience value, same as NUMERATOR | ALL_CHARACTERS
		DENOMINATOR_ALL = 6, // convenience value, same as DENOMINATOR | ALL_CHARACTERS
		EVERYTHING =  7// convenience value, same as BOTH | ALL_CHARACTERS. Intended that this is 7 and not a POW
	}
}

public static class FRTargetExtensions
{

	public static bool Has( this FR.Target t, FR.Target testFor )
	{
		// ex. ((t & FR.Target.NUMERATOR) == FR.Target.NUMERATOR)
		return (t & testFor) == testFor;
	}

	public static bool HasNumerator( this FR.Target t )
	{
		return t.Has ( FR.Target.NUMERATOR );
	}
	
	public static bool HasDenominator( this FR.Target t )
	{
		return t.Has ( FR.Target.DENOMINATOR );
	}
	
	public static FR.Target TargetFromFraction( Fraction fr )
	{
		FR.Target output = FR.Target.NONE;

		if( fr.Numerator.Value != 0 )
			output |= FR.Target.NUMERATOR;

		if( fr.Denominator.Value != 0 )
			output |= FR.Target.DENOMINATOR;

		return output;
	}
}

public class FractionRenderer
{
	protected Fraction _fraction = null;
	public Fraction Fraction
	{
		get{ return _fraction; }
		set{ _fraction = value; }
	}
	
	public NumberRenderer Numerator
	{
		// ATTN: important that this be fetched through fraction every time!!!
		// DO NOT BUFFER, will break much functionality / flexibility
		get
		{ 
			//if( _fraction.Numerator.Value != 0 )
			if( _fraction != null && _fraction.Numerator != null )
				return _fraction.Numerator.Renderer; 
			else
				return null;
		} 
	}
	
	public NumberRenderer Denominator
	{
		// ATTN: important that this be fetched through fraction every time!!!
		// DO NOT BUFFER, will break much functionality / flexibility
		get
		{ 
			//if( _fraction.Denominator.Value != 0 )
			if( _fraction != null && _fraction.Denominator != null )
				return _fraction.Denominator.Renderer; 
			else
				return null;
		} 
	}

	protected FractionAnimator _animator = null;
	public FractionAnimator Animator
	{
		get{ return _animator; }
		set{ _animator = value; }
	}
	

	/*
	// OLD: fraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("TurnRight");
	// NEW: fractionRenderer.AnimationFireBool(FR.Target.NUMERATOR, "TurnRight");
	public void AnimationFireBool(FR.Target target, string boolName)
	{
		List<Character> targets = GetCharacters(target);
		
		foreach( Character c in targets )
		{
			//Debug.LogError ("AnimationFireBool : " + c.name);
			c.GetComponent<Animator>().FireBool(c, boolName);
		}
	}
	
	public void AnimationSetBool(FR.Target target, string boolName, bool v)
	{
		List<Character> targets = GetCharacters(target);
		
		foreach( Character c in targets )
		{
			//Debug.LogError ("AnimationFireBool : " + c.name);
			c.GetComponent<Animator>().SetBool(boolName, v);
		}
	}
	*/
	
	/*
	public IEnumerator AnimationWaitForState(Character subject, string stateName)
	{
		yield return new WaitForSeconds(0.3f);
		
		bool waiting = true;
		while( waiting )
		{
			if( subject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Idle") )
				waiting = false;
			
			yield return null;
		}
		
		yield break;
	}
	*/
			
	public List<Character> GetCharacters(FR.Target target)
	{
		List<Character> output = new List<Character>();
		
		if( (target & FR.Target.NUMERATOR) == FR.Target.NUMERATOR )
		{
			if( this.Numerator != null )
			{
				if( (target & FR.Target.ALL_CHARACTERS) == FR.Target.ALL_CHARACTERS )
				{
					output.AddRange( this.Numerator.Characters );
				}
				else // default is only the interaction character
				{
					output.Add( this.Numerator.interactionCharacter );
				}
			}
		}
		
		if( (target & FR.Target.DENOMINATOR) == FR.Target.DENOMINATOR )
		{
			if( this.Denominator != null )
			{
				if( (target & FR.Target.ALL_CHARACTERS) == FR.Target.ALL_CHARACTERS )
				{
					output.AddRange( this.Denominator.Characters );
				}
				else // default is only the interaction character
				{
					output.Add( this.Denominator.interactionCharacter );
				}
			}
		}
		
		return output;
	}
	
}
