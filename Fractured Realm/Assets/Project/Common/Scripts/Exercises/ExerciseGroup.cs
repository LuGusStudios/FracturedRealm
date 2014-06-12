using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExerciseGroup
{
	public string name = "";

	public List<Exercise> exercises = new List<Exercise>();
	protected int currentExerciseIndex = -1;

	public Exercise NextExercise()
	{
		if( exercises == null || exercises.Count == 0 )
		{
			return null;
		}

		++currentExerciseIndex;

		if( currentExerciseIndex >= exercises.Count )
		{
			return null;
		}
		else
		{
			return exercises[ currentExerciseIndex ];
		}
	}
}
