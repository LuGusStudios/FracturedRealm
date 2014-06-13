using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExerciseGroup
{
	public string name = "";

	public List<Exercise> exercises = new List<Exercise>();
	protected int currentExerciseIndex = -1;

	public Exercise currentExercise = null;

	public Exercise NextExercise()
	{
		if( exercises == null || exercises.Count == 0 )
		{
			return null;
		}

		++currentExerciseIndex;

		if( currentExerciseIndex >= exercises.Count )
		{
			currentExercise = null;
			return null;
		}
		else
		{
			currentExercise = exercises[ currentExerciseIndex ];
			return currentExercise;
		}
	}
}
