using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FROperationTester : LugusSingletonRuntime<FROperationTester> 
{
	//public FR.WorldType worldType = FR.WorldType.DESERT;
	public FR.Target cameraMode = FR.Target.BOTH;
	
	public bool showGUI = true;

	public float interactionSpeed = 1.0f;
	public bool immediateMode = false; // if true, goes directly to MathManager, skipping MathInputManager and all HUD stuff. true == faster testing of individual animations etc.

	protected Vector2 GetScreenPosition( FR.OperationType operationIconType )
	{
		List<OperationIcon> operationIcons = new List<OperationIcon>();
		operationIcons.AddRange ( GameObject.FindObjectsOfType<OperationIcon>() );
		
		foreach( OperationIcon icon in operationIcons )
		{
			if( icon.type == operationIconType )
			{
				return GetScreenPosition( LugusCamera.ui, icon.transform.position );
			}
		}

		return Vector2.zero;
	}

	protected Vector2 GetScreenPosition( Fraction fr )
	{
		if( FRTargetExtensions.TargetFromFraction(fr).HasNumerator() )
			return GetScreenPosition( LugusCamera.numerator, fr.Numerator.Renderer.interactionCharacter.Head.position );
		else
			return GetScreenPosition( LugusCamera.denominator, fr.Denominator.Renderer.interactionCharacter.Head.position );
	}

	protected Vector2 GetScreenPosition(Camera cam, Vector3 worldPoint)
	{
		return cam.WorldToScreenPoint( worldPoint );
	}

	protected Fraction FindFraction(bool left)
	{
		List<Fraction> fractions = new List<Fraction>();

		NumberRenderer[] numbers = GameObject.FindObjectsOfType<NumberRenderer>();
		
		Fraction leftFraction = null;
		float smallestX = float.MaxValue;

		foreach( NumberRenderer number in numbers )
		{
			if( !fractions.Contains( number.Number.Fraction ) )
				fractions.Add( number.Number.Fraction );

			if( number.transform.position.x < smallestX )
				leftFraction = number.Number.Fraction;
		}

		if( left )
		{
			return leftFraction;
		}
		else
		{
			// for now, assume just 2 fractions
			if( fractions.Count == 1 )
				return fractions[0];
			else
			{
				if( fractions[0] == leftFraction )
					return fractions[1];
				else
					return fractions[0];
			}
		}
	}

	public Coroutine TriggerClick( Vector2 screenPosition )
	{
		return gameObject.StartLugusRoutine( TriggerClickRoutine( screenPosition ) ).Coroutine;
	}

	public IEnumerator TriggerClickRoutine( Vector2 screenPosition )
	{
		//Debug.LogError("TriggerClickRoutine : " + screenPosition );

		LugusInput.use.inputPoints.Clear();
		LugusInput.use.lastPoint = screenPosition;
		LugusInput.use.inputPoints.Add( LugusInput.use.lastPoint );
		LugusInput.use.down = true;

		yield return null;
		
		LugusInput.use.down = false;
		LugusInput.use.dragging = true;

		yield return new WaitForSeconds(0.05f);
		
		LugusInput.use.dragging = false;
		LugusInput.use.up = true;

		yield return null;
		
		LugusInput.use.up = false;
	}

	public bool busy = false;

	public void TestOperation( FR.OperationType operationType, Fraction fr1 = null, Fraction fr2 = null )
	{
		this.gameObject.StartLugusRoutine( TestOperationRoutine(operationType, fr1, fr2) );
	}

	public Fraction[] GetRandomFractions( FR.OperationType operationType )
	{
		Fraction[] fractions = new Fraction[2];
		
		if( operationType == FR.OperationType.ADD )
		{
			//fractions[0] = new Fraction( 6, 4 );
			//fractions[1] = new Fraction( 3, 4 );
			
			fractions[0] = new Fraction( 3, 4 );
			fractions[1] = new Fraction( 8, 4 );
		}
		if( operationType == FR.OperationType.SUBTRACT )
		{
			fractions[0] = new Fraction( 9, 2 );
			fractions[1] = new Fraction( 5, 2 );
		}
		if(  operationType == FR.OperationType.MULTIPLY )
		{
			//fractions[0] = new Fraction( 3, 4 );
			//fractions[1] = new Fraction( 3, 2 );
			fractions[0] = new Fraction( 2, 2 );
			fractions[1] = new Fraction( 5, 6 );
		}
		if( operationType == FR.OperationType.DIVIDE )
		{
			//fractions[0] = new Fraction( 2, 5 );
			//fractions[1] = new Fraction( 2, 4 );
			fractions[0] = new Fraction( 1, 1 );
			fractions[1] = new Fraction( 7, 7 );
		}
		if( operationType == FR.OperationType.SIMPLIFY )
		{
			fractions[0] = new Fraction( 8, 4 );
			fractions[1] = new Fraction( 5, 3 );
		}
		if( operationType == FR.OperationType.DOUBLE )
		{
			fractions[0] = new Fraction( 5, 2 );
			fractions[1] = new Fraction( 8, 4 );
		}

		return fractions;
	}

	// NOTE: FR.Target is used a little differently here
	// normally .BOTH means num + denom
	// here, it means ONLY the cameraview of both worlds is shown
	// use .EVERYTHING to do BOTH + NUMERATOR + DENOMINATOR
	public IEnumerator TestOperationRoutine(FR.OperationType operationType, Fraction fr1 = null, Fraction fr2 = null, FR.Target cameras = FR.Target.EVERYTHING)
	{
		if( fr1 == null || fr2 == null )
		{
			Fraction[] frs = GetRandomFractions(operationType); 
			fr1 = frs[0];
			fr2 = frs[1];
		}

		busy = true;
	
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


		busy = false;
	}

	/*
	public void TestAdd( Fraction fr1, Fraction fr2 )
	{
		Fraction[] fractions = new Fraction[2];
		fractions[0] = fr1;
		fractions[1] = fr2;

		gameObject.StartLugusRoutine( DefaultTestRoutine(FR.OperationType.ADD, fractions) );
	}
	
	public void TestSubtract( Fraction fr1, Fraction fr2 )
	{
		Fraction[] fractions = new Fraction[2];
		fractions[0] = fr1;
		fractions[1] = fr2;
		
		gameObject.StartLugusRoutine( DefaultTestRoutine(FR.OperationType.SUBTRACT, fractions) );
	}
	*/

	protected IEnumerator ImmediateModeTestRoutine(FR.OperationType operationType, Fraction[] fractions)
	{
		WorldFactory.use.CreateDebugWorld( WorldFactory.use.defaultWorldType, fractions, cameraMode, false );
		MathInputManager.use.InitializeOperationIcons(1);
		LugusInput.use.acceptInput = false;
		
		yield return new WaitForSeconds(0.5f * interactionSpeed);

		MathManager.use.SelectOperation( operationType );
		MathManager.use.OnTarget1Selected( FindFraction(true) );

		if( operationType != FR.OperationType.SIMPLIFY && operationType != FR.OperationType.DOUBLE )
		{
			MathManager.use.OnTarget2Selected( FindFraction(false) );
		}

		
		MathManager.use.ProcessCurrentOperation();


		while( MathManager.use.operationInfo != null )
		{
			yield return null;
		}
		
		LugusInput.use.acceptInput = true;
		
		yield break;
	}

	protected IEnumerator DefaultTestRoutine(FR.OperationType operationType, Fraction[] fractions)
	{
		WorldFactory.use.CreateDebugWorld(  WorldFactory.use.defaultWorldType, fractions, cameraMode, false );

		/*
		WorldFactory.use.CreateWorld( worldType, fractions );
		HUDManager.use.SetMode( cameraMode );
		FRCamera.use.MoveToDefaultPositions();
		*/
		MathInputManager.use.InitializeOperationIcons(1);
		
		LugusInput.use.acceptInput = false;
		
		yield return new WaitForSeconds(1.0f * interactionSpeed);
		
		yield return TriggerClick( GetScreenPosition( operationType ) );
		
		yield return new WaitForSeconds(2.0f + (1.0f * interactionSpeed) ); // wait for icon to fly to the top and arrows to show
		
		yield return TriggerClick( GetScreenPosition( FindFraction(true) ) );

		if( operationType != FR.OperationType.SIMPLIFY && operationType != FR.OperationType.DOUBLE )
		{
			yield return new WaitForSeconds( 1.0f * interactionSpeed );
		
			yield return TriggerClick( GetScreenPosition( FindFraction(false) ) );
		}
		
		while( MathManager.use.operationInfo != null )
		{
			yield return null;
		}
		
		LugusInput.use.acceptInput = true;
		
		yield break;
	}
	/*
	protected IEnumerator TestAddRoutine(Fraction[] fractions)
	{
		WorldFactory.use.CreateWorld( worldType, fractions );
		HUDManager.use.SetMode( cameraMode );
		MathInputManager.use.InitializeOperationIcons(1);

		LugusInput.use.acceptInput = false;
		
		yield return new WaitForSeconds(1.0f * interactionSpeed);

		yield return TriggerClick( GetScreenPosition( LugusCamera.ui, FR.OperationType.ADD ) );

		yield return new WaitForSeconds(2.0f + (1.0f * interactionSpeed) ); // wait for icon to fly to the top and arrows to show

		yield return TriggerClick( GetScreenPosition( FindFraction(true) ) );

		yield return new WaitForSeconds( 1.0f * interactionSpeed );

		yield return TriggerClick( GetScreenPosition( FindFraction(false) ) );

		while( MathManager.use.currentState != null )
		{
			yield return null;
		}

		LugusInput.use.acceptInput = true;

		yield break;
	}
	*/


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
	
	protected void Update () 
	{
	
	}

	public void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		if( !showGUI )
			return;

		if( !busy )
			GUILayout.BeginArea( new Rect(0, 0, 200, 400), GUI.skin.box );
		else
			GUILayout.BeginArea( new Rect(0, 0, 200, 30), GUI.skin.box );

		GUILayout.BeginVertical();

		DrawGUI(null, null);
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public void DrawGUI(Fraction fr1, Fraction fr2)
	{
		if( FROperationTester.use.busy )
		{
			GUILayout.Label("Operation is in progress...");
		}
		else
		{
			FR.OperationType operationType = FR.OperationType.NONE;
			Fraction[] fractions = null;
			if( GUILayout.Button("Test ADD") )
			{
				operationType = FR.OperationType.ADD;
			}
			if( GUILayout.Button("Test SUBTRACT") )
			{
				operationType = FR.OperationType.SUBTRACT;
			}
			if( GUILayout.Button("Test MULTIPLY") )
			{
				operationType = FR.OperationType.MULTIPLY;
			}
			if( GUILayout.Button("Test DIVIDE") )
			{
				operationType = FR.OperationType.DIVIDE;
			}
			if( GUILayout.Button("Test SIMPLIFY") )
			{
				operationType = FR.OperationType.SIMPLIFY;
			}
			if( GUILayout.Button("Test DOUBLE") )
			{
				operationType = FR.OperationType.DOUBLE;
			}
			
			if( operationType != FR.OperationType.NONE )
			{
				fractions = new Fraction[2];

				if( fr1 != null )
				{
					fractions[0] = fr1;
					fractions[1] = fr2;
				}
				else
				{
					fractions[0] = null;
					fractions[1] = null;
				}
				
				FROperationTester.use.TestOperation( operationType, fractions[0], fractions[1] );
				
			}

			GUILayout.Space(20);

			FR.WorldType[] worldTypes = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
			foreach( FR.WorldType worldType in worldTypes )
			{
				if( worldType == FR.WorldType.NONE )
					continue;

				if( GUILayout.Button("Create " + worldType + "\n") )
				{
					fractions = new Fraction[2];
					fractions[0] = new Fraction( 6, 3 );
					fractions[1] = new Fraction( 2, 3 );

					WorldFactory.use.CreateDebugWorld( worldType, fractions );
				}
			}

			/*
			if( GUILayout.Button("RESET WORLD\n") )
			{
				WorldFactory.use.CreateWorld(FR.WorldType.DESERT, FR.Target.BOTH, true);
				HUDManager.use.SetMode( FR.Target.BOTH );
			}
			*/
		}
	}
}
