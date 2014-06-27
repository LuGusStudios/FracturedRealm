using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	// left some room in between items for if we ever need to add it can be done logically
	public enum ExerciseDifficulty
	{
		NONE = -1,

		EASIEST = 1,
		EASY = 3,
		MEDIUM = 5,
		HARD = 7,
		HARDEST = 9
	}
}

public class Exercise
{
	public string name = "";
	public FR.ExerciseDifficulty difficulty = FR.ExerciseDifficulty.NONE; // if none: not taken into account with randomized sets of exercises
	public FR.Target composition = FR.Target.NONE; // if none: defaults to both
	public FR.WorldType worldType = FR.WorldType.NONE; // if none: chosen at random

	public List<ExercisePart> parts = new List<ExercisePart>();
	protected int currentPartIndex = -1;

	public ExercisePart currentPart = null;

	public ExercisePart Next()
	{
		if( parts == null || parts.Count == 0 )
		{
			return null;
		}
		
		++currentPartIndex;
		
		if( currentPartIndex >= parts.Count )
		{
			currentPart = null;
			return null;
		}
		else
		{
			currentPart = parts[ currentPartIndex ];
			return currentPart;
		}
	}
}
