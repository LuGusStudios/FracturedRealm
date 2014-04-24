/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

ENUM
	
public class FRAnimations
{

	public static FRAnimation TypeFromString( string animationName )
	{
		try
		{
			return (FRAnimation) Enum.Parse( typeof(FRAnimation), animationName ); 

		}
		catch( ArgumentException e )
		{
			return FRAnimation.NONE;
		}
	}
	
	protected static Dictionary<int, FRAnimationData> _animations = null;
	public static Dictionary<int, FRAnimationData> animations
	{
		get
		{
			if( _animations == null )
			{
				_animations = new Dictionary<int, FRAnimationData>();
				FillDictionary();
			}

			return _animations;
		}
	}

	public static void FillDictionary()
	{
		INIT
	}

	public FRAnimations()
	{
		FillDictionary();
	}
}
*/
