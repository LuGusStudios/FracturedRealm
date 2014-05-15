/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FR
{
	public enum AnimationStage
	{
		NONE = -1,
		Stage1 = 1,
		Stage2 = 2
	}
	
	ENUM
}

public partial class FRAnimations : LugusSingletonRuntime<FRAnimations>
{

	public FR.Animation TypeFromString( string animationName )
	{
		try
		{
			return (FR.Animation) Enum.Parse( typeof(FR.Animation), animationName ); 

		}
		catch( ArgumentException e )
		{
			return FR.Animation.NONE;
		}
	}
	
	protected Dictionary<int, FRAnimationData> _animations = null;
	public Dictionary<int, FRAnimationData> animations
	{
		get
		{
			if( _animations == null )
			{
				FillDictionary();
			}

			return _animations;
		}
	}

	public FRAnimationData GetAnimationData( FR.Animation animation )
	{
		return _animations[ (int) animation];
	}

	public void FillDictionary()
	{
		_animations = new Dictionary<int, FRAnimationData>();
				
		INIT
	}

	public FRAnimations()
	{
		FillDictionary();
	}
}
*/
