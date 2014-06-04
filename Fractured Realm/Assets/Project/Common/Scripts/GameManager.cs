using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	public enum GameState
	{
		NONE = -1,

		Start = 1,
		StartSequence = 2,
		WaitingForInput = 3,
		ProcessingOperation = 4,
		EndSequence = 5,
		End = 6
	}
}

public class GameManager : LugusSingletonRuntime<GameManager>
{
	public delegate void OnStateChanged(FR.GameState oldState, FR.GameState newState);
	public OnStateChanged onStateChanged = null;

	public FR.GameState currentState = FR.GameState.NONE;

	public World currentWorld = null;

	public void StartGame( FR.WorldType type, Fraction[] fractions, FR.Target composition = FR.Target.BOTH, FR.GameState startFromState = FR.GameState.Start )
	{
		currentWorld = WorldFactory.use.CreateDebugWorld(  type, fractions, composition, false );
		
		ChangeState( startFromState );
	}

	public void ChangeState(FR.GameState newState)
	{
		FR.GameState oldState = currentState;
		currentState = newState;

		Debug.Log ("GameManager:ChangeState : from " + oldState + " to " + newState );

		if( newState == FR.GameState.Start )
		{
			if( oldState != FR.GameState.NONE )
			{
				Debug.LogError("GameManager:ChangeState : changed to Start from " + oldState + ". Not allowed! Oldstate should be NONE");
			}
			else
			{
				LugusCoroutines.use.StartRoutine( StartRoutine() );
			}
		}

		if( newState == FR.GameState.StartSequence )
		{
			LugusCoroutines.use.StartRoutine( StartSequenceRoutine() );
		}

		if( newState == FR.GameState.EndSequence )
		{			
			if( oldState != FR.GameState.ProcessingOperation )
			{
				Debug.LogError("GameManager:ChangeState : changed to EndSequence from " + oldState + ". Not allowed! Oldstate should be ProcessingOperation");
			}
			else
			{
				LugusCoroutines.use.StartRoutine( EndSequenceRoutine() );
			}
		}

		if( newState == FR.GameState.End )
		{
			LugusCoroutines.use.StartRoutine( EndRoutine() );
		}

		if( onStateChanged != null ) 
			onStateChanged( oldState, newState );
	}

	public IEnumerator StartRoutine()
	{
		// black overlay (if not yet present)
		// fade from black

		ScreenFader.use.FadeIn(3.0f);
		
		ChangeState( FR.GameState.StartSequence );

		yield break;
	}

	public int currentInteractionGroupIndex = 0;

	public IEnumerator StartSequenceRoutine()
	{
		
		Fraction right = FindFraction(false);
		Fraction left = FindFraction(true);
		
		Vector3 scale1 = left.Numerator.Renderer.transform.localScale;
		left.Numerator.Renderer.transform.localScale = Vector3.zero;
		
		Vector3 scale2 = left.Denominator.Renderer.transform.localScale;
		left.Denominator.Renderer.transform.localScale = Vector3.zero;

		// move camera to camera entry for the current interaction group
		if( currentInteractionGroupIndex == 0 )
		{
			FRCamera.use.MoveToInteractionGroup( currentInteractionGroupIndex + 1, 0, false );

			yield return new WaitForSeconds(3.0f); // fade in : see StartRoutine()
		}
		else
		{
			FRCamera.use.MoveToInteractionGroup( currentInteractionGroupIndex + 1, 0, true );
			yield return new WaitForSeconds(1.0f);
		}

		left.Numerator.Renderer.transform.position = currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].PortalEntry.position;
		left.Denominator.Renderer.transform.position = currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].PortalEntry.position;

		left.Numerator.Renderer.gameObject.ScaleTo(scale1).Time (2.0f).Execute();
		left.Denominator.Renderer.gameObject.ScaleTo(scale2).Time (2.0f).Execute();

		yield return new WaitForSeconds(2.0f);

		yield return left.Renderer.Animator.RotateTowards( FR.Target.BOTH, right.Renderer ).Coroutine;

		FRCamera.use.MoveToInteractionGroup( currentInteractionGroupIndex + 1, 1, true );
		left.Numerator.Renderer.Animator.RunTo( currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].Spawn1.position );
		left.Denominator.Renderer.Animator.RunTo( currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].Spawn1.position );

		yield return new WaitForSeconds(2.0f);
		
		yield return left.Renderer.Animator.RotateTowardsCamera().Coroutine;

		ChangeState( FR.GameState.WaitingForInput );

		yield break;
	}

	protected IEnumerator EndSequenceRoutine()
	{
		Fraction left = FindFraction(true);

		Debug.Log (Time.frameCount + " before turn to exit ");

		yield return LugusCoroutineUtil.WaitForFinish(
			left.Renderer.Numerator.Animator.RotateTowards( currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].PortalExit.position ),
			left.Renderer.Denominator.Animator.RotateTowards( currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].PortalExit.position )
			).Coroutine;
		
		Debug.Log (Time.frameCount + " after turn to exit ");

		//yield return new WaitForSeconds(1.0f);

		FRCamera.use.MoveToInteractionGroup( currentInteractionGroupIndex + 1, 2, true );
		left.Numerator.Renderer.Animator.RunTo( currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].PortalExit.position );
		left.Denominator.Renderer.Animator.RunTo( currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].PortalExit.position );

		yield return new WaitForSeconds(2.0f);

		yield return left.Renderer.Animator.RotateTowardsCamera().Coroutine;

		
		left.Numerator.Renderer.gameObject.ScaleTo(Vector3.zero).Time (2.0f).Execute();
		left.Denominator.Renderer.gameObject.ScaleTo(Vector3.zero).Time (2.0f).Execute();
		
		yield return new WaitForSeconds(2.0f);

		ChangeState( FR.GameState.End );
		yield break;
	}

	protected IEnumerator EndRoutine()
	{
		currentInteractionGroupIndex++;

		ChangeState( FR.GameState.StartSequence );
		yield break;
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

	// Use this for initialization
	void Start () 
	{
		MathInputManager mim = MathInputManager.use; // make sure it's initialized
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
