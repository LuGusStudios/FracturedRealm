﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FRAnimationTester : LugusSingletonRuntime<FRAnimationTester>  
{
	public bool busy = false;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only

		if( !this.gameObject.activeSelf )
			return;

		FROperationTester.use.showGUI = false;
		FROperationTester.use.immediateMode = true;
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
	
	protected void Update () 
	{
	
	}

	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		if( !busy )
			GUILayout.BeginArea( new Rect(0, 0, 400, 400), GUI.skin.box );
		else
			GUILayout.BeginArea( new Rect(0, 0, 200, 30), GUI.skin.box );
		GUILayout.BeginVertical();
		
		DrawGUI(null, null);
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	protected FR.OperationType currentOperation = FR.OperationType.ADD;

	protected Vector2 scrollPosition = Vector2.zero;

	public void DrawGUI(Fraction fr1, Fraction fr2)
	{
		//FRAnimations.use.OperationAnimations[ currentOperation ];

		if( FRAnimationTester.use.busy )
		{
			GUILayout.Label("Animation is in progress...");
		}
		else
		{
			GUILayout.BeginHorizontal();
			int count = 0;
			FR.OperationType[] operationTypes = (FR.OperationType[]) Enum.GetValues(typeof(FR.OperationType));
			foreach( FR.OperationType operationType in operationTypes )
			{
				if( !FRAnimations.use.OperationAnimations.ContainsKey( operationType ) )
					continue;
				
				if( GUILayout.Button("" + operationType) )
				{
					currentOperation = operationType;
				}

				count++;

				//if( count >= 4 )
				//{
				//	GUILayout.EndHorizontal();
				//	GUILayout.BeginHorizontal();
				//}
			}
			GUILayout.EndHorizontal();

			if( currentOperation == FR.OperationType.NONE )
			{
				return;
			}

			GUILayout.Space(30);
			GUILayout.BeginScrollView( scrollPosition );

			List<FR.Animation> animations = FRAnimations.use.OperationAnimations[ currentOperation ];

			foreach( FR.Animation animation in animations )
			{
				FRAnimationData animationData = FRAnimations.use.GetAnimationData( animation );
				if( animationData.visualizer.GetImplementationStatus() == FR.VisualizerImplementationStatus.NONE )
				{
					continue;
				}

				if( GUILayout.Button("\n" + animationData.name + "\n") )
				{
					Debug.Log ("Start animation " + animationData.name);
					this.gameObject.StartLugusRoutine( TestAnimationRoutine(animationData) );
				}
			}

			GUILayout.EndScrollView();
		}
	}

	protected IEnumerator TestAnimationRoutine(FRAnimationData animation)
	{
		busy = true;
		LugusDebug.debug = false;

		// swap the OperationAnimations list with a list containing just the one we want to test here, so it's always selected
		List<FR.Animation> starterAnimations = FRAnimations.use.OperationAnimations[ animation.operation ];
		FRAnimations.use.OperationAnimations[ animation.operation ] = new List<FR.Animation>();
		FRAnimations.use.OperationAnimations[ animation.operation ].Add( animation.type );



		yield return this.gameObject.StartLugusRoutine( FROperationTester.use.TestOperationRoutine( animation.operation, null, null, FR.Target.BOTH ) ).Coroutine;

		FRAnimations.use.OperationAnimations[ animation.operation ] = starterAnimations;
		
		LugusDebug.debug = true;
		busy = false;
	}
}
