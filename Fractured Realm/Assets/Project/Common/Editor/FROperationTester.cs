using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FROperationTester : LugusSingletonRuntime<FROperationTester> 
{
	public FR.WorldType worldType = FR.WorldType.DESERT;
	public FR.Target cameraMode = FR.Target.BOTH;

	public float interactionSpeed = 1.0f;

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
		Debug.LogError("TriggerClickRoutine : " + screenPosition );

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

	public void TestAdd( Fraction fr1, Fraction fr2 )
	{
		Fraction[] fractions = new Fraction[2];
		fractions[0] = fr1;
		fractions[1] = fr2;

		gameObject.StartLugusRoutine( DefaultTestRoutine(FR.OperationType.ADD, fractions) );
	}

	protected IEnumerator DefaultTestRoutine(FR.OperationType operationType, Fraction[] fractions)
	{
		WorldFactory.use.CreateWorld( worldType, fractions );
		HUDManager.use.SetMode( cameraMode );
		MathInputManager.use.InitializeOperationIcons(1);
		
		LugusInput.use.acceptInput = false;
		
		yield return new WaitForSeconds(1.0f * interactionSpeed);
		
		yield return TriggerClick( GetScreenPosition( operationType ) );
		
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
}
