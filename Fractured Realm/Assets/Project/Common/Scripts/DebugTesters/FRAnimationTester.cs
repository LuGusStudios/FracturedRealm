using UnityEngine;
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

			FRAnimationData randomDescendAnimation = null;
			List<FR.Animation> ascendAnimations = new List<FR.Animation>();

			foreach( FR.Animation animation in animations )
			{
				FRAnimationData animationData = FRAnimations.use.GetAnimationData( animation );
				if( animationData.visualizer.GetImplementationStatus() == FR.VisualizerImplementationStatus.NONE )
				{
					continue;
				}

				if( randomDescendAnimation == null || (UnityEngine.Random.value > 0.5f) )
					randomDescendAnimation = animationData;

				if( GUILayout.Button("\n" + animationData.name + " " + ( (int) animationData.visualizer.GetImplementationStatus() ) + "\n") )
				{
					Debug.Log ("Start animation " + animationData.name);
					this.gameObject.StartLugusRoutine( TestAnimationRoutine(animationData) );
				}

				foreach( FR.Animation ascender in animationData.visualizer.NextAnimations )
				{
					if( !ascendAnimations.Contains( ascender ) )
					{
						ascendAnimations.Add( ascender );
					}
				}
			}

			if( currentOperation == FR.OperationType.DIVIDE && randomDescendAnimation != null )
			{
				// we also want to test the ASCEND operations

				// there is no list of ascend animations as there is of descend (normally, we get it from NextAnimations on the descends)
				// so: we compiled it while looping over the descends above

				GUILayout.Space(50);

				foreach( FR.Animation animation in ascendAnimations )
				{
					FRAnimationData animationData = FRAnimations.use.GetAnimationData( animation );
					if( animationData.visualizer.GetImplementationStatus() == FR.VisualizerImplementationStatus.NONE )
					{
						continue;
					}
					
					if( GUILayout.Button("\n" + animationData.name + " " + ( (int) animationData.visualizer.GetImplementationStatus() ) + "\n") )
					{
						Debug.Log ("Start animation " + animationData.name);
						this.gameObject.StartLugusRoutine( TestAnimationAscendRoutine(randomDescendAnimation, animationData) );
					}
				}
			}

			GUILayout.EndScrollView();
		}
	}

	protected IEnumerator TestAnimationAscendRoutine(FRAnimationData descender, FRAnimationData ascender)
	{
		busy = true;
		LugusDebug.debug = false;
		
		// swap the OperationAnimations list with a list containing just the one we want to test here, so it's always selected
		List<FR.Animation> starterAnimations = FRAnimations.use.OperationAnimations[ descender.operation ];
		FRAnimations.use.OperationAnimations[ descender.operation ] = new List<FR.Animation>();
		FRAnimations.use.OperationAnimations[ descender.operation ].Add( descender.type );

		List<FR.Animation> ascenderList = new List<FR.Animation>();
		ascenderList.Add( ascender.type );
		descender.visualizer.NextAnimations = ascenderList;
		
		yield return this.gameObject.StartLugusRoutine( FROperationTester.use.TestOperationRoutine( descender.operation, null, null, FR.Target.BOTH ) ).Coroutine;
		
		descender.visualizer.NextAnimations = null;
		FRAnimations.use.OperationAnimations[ descender.operation ] = starterAnimations;
		
		LugusDebug.debug = true;
		busy = false;
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
