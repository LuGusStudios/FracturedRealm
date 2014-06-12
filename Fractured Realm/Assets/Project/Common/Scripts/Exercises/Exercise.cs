using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	// left some room in between items for if we ever need to add it can be done logically
	public enum ExerciseDifficulty
	{
		NONE = -1,

		Easiest = 1,
		Easy = 3,
		Medium = 5,
		Hard = 7,
		Hardest = 9
	}
}

public class Exercise
{
	public string name = "";
	public FR.ExerciseDifficulty difficulty = FR.ExerciseDifficulty.NONE;

	public List<ExercisePart> parts = new List<ExercisePart>();
	protected int currentPartIndex = -1;

	public ExercisePart Next()
	{
		if( parts == null || parts.Count == 0 )
		{
			return null;
		}
		
		++currentPartIndex;
		
		if( currentPartIndex >= parts.Count )
		{
			return null;
		}
		else
		{
			return parts[ currentPartIndex ];
		}
	}
}
