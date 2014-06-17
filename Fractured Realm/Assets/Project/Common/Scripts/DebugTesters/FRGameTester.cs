using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FRGameTester : LugusSingletonRuntime<FRGameTester> 
{
	//public FR.WorldType worldType = FR.WorldType.DESERT;
	public FR.Target cameraMode = FR.Target.BOTH;
	
	public bool showGUI = true;
	
	public float interactionSpeed = 1.0f;
	public bool immediateMode = false; // if true, goes directly to MathManager, skipping MathInputManager and all HUD stuff. true == faster testing of individual animations etc.
	public bool skipStartSequence = true;

	public bool busy = false;
	
	public void TestExerciseGroup( ExerciseGroup exercises )
	{
		this.gameObject.StartLugusRoutine( TestExerciseGroupRoutine(exercises) );
	}

	public IEnumerator TestExerciseGroupRoutine( ExerciseGroup exercises )
	{
		foreach( Exercise exercise in exercises.exercises )
		{
			yield return this.gameObject.StartLugusRoutine( TestExerciseRoutine(exercise) ).Coroutine;
		}

		yield break;
	}

	public void TestExercise( Exercise exercise )
	{
		this.gameObject.StartLugusRoutine( TestExerciseRoutine(exercise) );
	}

	public IEnumerator TestExerciseRoutine( Exercise exercise )
	{
		Debug.Log ("Starting exercise routine " + (exercise) );

		busy = true;
		
		
		if( skipStartSequence || immediateMode )
		{
			GameManager.use.StartGame(exercise, FR.GameState.WaitingForInput );
		}
		else
		{
			GameManager.use.StartGame(exercise, FR.GameState.Start );
		}

		foreach( ExercisePart part in exercise.parts )
		{
			/*
			Fraction[] fractions = new Fraction[2];
			fractions[0] = part.First.CopyData();
			fractions[1] = part.Last.CopyData();

			if( exercise.composition == FR.Target.NUMERATOR )
			{
				fractions[0].Denominator.Value = 0;
				fractions[1].Denominator.Value = 0;
			}
			if( exercise.composition == FR.Target.DENOMINATOR )
			{
				fractions[0].Numerator.Value = 0;
				fractions[1].Numerator.Value = 0;
			}
			*/
			
			
			if( immediateMode )
			{
				yield return gameObject.StartLugusRoutine( FROperationTester.use.ImmediateModeTestRoutine(part.operations[0]) ).Coroutine;
			}
			else
			{
				yield return gameObject.StartLugusRoutine( FROperationTester.use.DefaultTestRoutine(part.operations[0]) ).Coroutine;
			}
			
			yield return new WaitForSeconds(1.5f);

		}
		
		Debug.Log ("Stopped exercise routine " + (exercise) );

		busy = false;
	}
	
	// NOTE: FR.Target is used a little differently here
	// normally .BOTH means num + denom
	// here, it means ONLY the cameraview of both worlds is shown
	// use .EVERYTHING to do BOTH + NUMERATOR + DENOMINATOR
	public IEnumerator TestOperationRoutine(FR.OperationType operationType, Fraction fr1 = null, Fraction fr2 = null, FR.Target cameras = FR.Target.EVERYTHING)
	{
		
		busy = true;

		/*
		Fraction[] fractions = new Fraction[2];
		
		if( cameras == FR.Target.BOTH || cameras == FR.Target.EVERYTHING )
		{
			fractions[0] = fr1.CopyData();
			fractions[1] = fr2.CopyData();
			
			cameraMode = FR.Target.BOTH;
			
			if( immediateMode )
				yield return gameObject.StartLugusRoutine( ImmediateModeTestRoutine(operationType, fractions) ).Coroutine;
			else
				yield return gameObject.StartLugusRoutine( DefaultTestRoutine(operationType, fractions) ).Coroutine;
			
			yield return new WaitForSeconds(1.5f);
		}
		
		if( cameras != FR.Target.BOTH )
		{
			if( cameras.HasNumerator() && operationType != FR.OperationType.DIVIDE )
			{
				// Numerator only
				cameraMode = FR.Target.NUMERATOR;
				fractions[0] = fr1.CopyData();
				fractions[1] = fr2.CopyData();
				fractions[0].Denominator.Value = 0;
				fractions[1].Denominator.Value = 0;
				
				if( immediateMode )
					yield return gameObject.StartLugusRoutine( ImmediateModeTestRoutine(operationType, fractions) ).Coroutine;
				else
					yield return gameObject.StartLugusRoutine( DefaultTestRoutine(operationType, fractions) ).Coroutine;
				
				yield return new WaitForSeconds(1.5f);
			}
			
			if( cameras.HasDenominator() && operationType != FR.OperationType.DIVIDE )
			{
				// Denominator only
				cameraMode = FR.Target.DENOMINATOR;
				fractions[0] = fr1.CopyData();
				fractions[1] = fr2.CopyData();
				fractions[0].Numerator.Value = 0;
				fractions[1].Numerator.Value = 0;
				
				if( immediateMode )
					yield return gameObject.StartLugusRoutine( ImmediateModeTestRoutine(operationType, fractions) ).Coroutine;
				else
					yield return gameObject.StartLugusRoutine( DefaultTestRoutine(operationType, fractions) ).Coroutine;
				
				yield return new WaitForSeconds(1.5f);
			}
		}

		*/
		
		busy = false;

		yield break;
	}
	

	
	
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void ShowGroupSelectGUI()
	{
		GUILayout.BeginArea( new Rect(0, 250 , 200, 200 ), GUI.skin.box);
		GUILayout.BeginVertical();
		
		foreach( string group in ExerciseManager.use.allExerciseGroups )
		{
			GUILayout.Label("" + group );
			GUILayout.BeginHorizontal();
			if( GUILayout.Button("auto\n") )
			{
				ExerciseManager.use.currentExerciseGroup = ExerciseManager.use.LoadExerciseGroup( group );
				FRGameTester.use.TestExerciseGroup( ExerciseManager.use.currentExerciseGroup );
			}
			if( GUILayout.Button("manual\n") )
			{
				ExerciseManager.use.currentExerciseGroup = ExerciseManager.use.LoadExerciseGroup( group );
				GameManager.use.StartGame( ExerciseManager.use.currentExerciseGroup );
			}
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	public void OnGUI()
	{
		if( !LugusDebug.debug )
			return;
		
		if( !showGUI )
			return;
		
		if( !busy )
		{
			ShowGroupSelectGUI();
		}
		else
		{
			GUILayout.BeginArea( new Rect(0, 0, 200, 30), GUI.skin.box );
			GUILayout.BeginVertical();
			GUILayout.Label("Operation is in progress...");
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
}
