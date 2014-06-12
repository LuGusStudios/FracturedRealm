using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExerciseManager : LugusSingletonRuntime<ExerciseManager> 
{
	public ExerciseGroup currentExerciseGroup = null;

	public List<string> allExerciseGroups = new List<string>();
	public int currentExerciseGroupIndex = -1; 

	public ExerciseGroup NextExerciseGroup()
	{
		if( allExerciseGroups == null || allExerciseGroups.Count == 0 )
		{
			return null;
		}
		
		++currentExerciseGroupIndex;
		
		if( currentExerciseGroupIndex >= allExerciseGroups.Count )
		{
			return null;
		}
		else
		{
			return LoadExerciseGroup( allExerciseGroups[ currentExerciseGroupIndex ] );
		}
	}

	public void Start()
	{
		LoadDebugData();
	}

	public void LoadDebugData()
	{
		allExerciseGroups = ExerciseLoader.LoadAllExerciseGroupNames();

		if( allExerciseGroups == null || allExerciseGroups.Count == 0 )
		{
			Debug.LogError("ExerciseManager:Load : no AllExerciseGroups found!");
			return;
		}

		NextExerciseGroup();
		NextExerciseGroup();

		/*
		ExerciseGroup group = null;
		Exercise exercise = null;
		ExercisePart part = null;
		Fraction[] fractions = new Fraction[2];


		group = new ExerciseGroup();

		exercise = new Exercise();
		exercise.name = "1";
		exercise.difficulty = FR.ExerciseDifficulty.Easiest;

		part = new ExercisePart();
		fractions[0] = new Fraction( 1, 2 ); 
		fractions[1] = new Fraction( 3, 2 );
		part.operations.Add ( FR.OperationType.ADD );
		part.fractions = fractions;
		part.outcome = new Fraction( 4, 2 );
		exercise.parts.Add(part);
		
		part = new ExercisePart();
		fractions[0] = new Fraction( 4, 2 ); 
		fractions[1] = new Fraction( 3, 2 );
		part.operations.Add ( FR.OperationType.SUBTRACT );
		part.fractions = fractions;
		part.outcome = new Fraction( 1, 2 );
		exercise.parts.Add(part);

		group.exercises.Add( exercise );

		
		exercise = new Exercise();
		exercise.name = "2";
		exercise.difficulty = FR.ExerciseDifficulty.Medium;
		
		part = new ExercisePart();
		fractions[0] = new Fraction( 2, 3 ); 
		fractions[1] = new Fraction( 4, 2 );
		part.operations.Add ( FR.OperationType.MULTIPLY );
		part.fractions = fractions;
		part.outcome = new Fraction( 8, 6 );
		exercise.parts.Add(part);
		
		part = new ExercisePart();
		fractions[0] = new Fraction( 2, 3 ); 
		fractions[1] = new Fraction( 4, 2 );
		part.operations.Add ( FR.OperationType.DIVIDE );
		part.fractions = fractions;
		part.outcome = new Fraction( 12, 4 );
		exercise.parts.Add(part);
		
		group.exercises.Add( exercise );


		//allExerciseGroups.Add( group );


		LoadExerciseGroup( "1" );
		*/
	}

	/*
	public void LoadExerciseGroup(ExerciseGroup group)
	{
		currentExerciseGroup = group;
	}
	*/

	public ExerciseGroup LoadExerciseGroup(string name)
	{
		ExerciseGroup group = ExerciseLoader.LoadExerciseGroup( name );

		if( group == null )
		{
			Debug.LogError("ExerciseManager:LoadExerciseGroup : group " + name + " not found!");
			return null;
		}
		else
		{
			return group;
		}
	}
}
