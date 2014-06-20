using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	public enum GameState
	{
		NONE = -1,

		ExerciseStart = 1,
		PartStart = 2,
		PartStartSequence = 3,
		WaitingForInput = 4,
		ProcessingOperation = 5,
		PartEndSequence = 6,
		PartEnd = 7,
		ExerciseEnd = 8
	}
}

public class GameManager : LugusSingletonExisting<GameManager>
{
	public delegate void OnStateChanged(FR.GameState oldState, FR.GameState newState);
	public OnStateChanged onStateChanged = null;

	public FR.GameState currentState = FR.GameState.NONE;
	public ExercisePart currentExercisePart = null;

	public World currentWorld = null;

	public void StartGame( ExerciseGroup exercises, FR.GameState startFromState = FR.GameState.ExerciseStart )
	{
		// TODO: change me! 
		StartGame( exercises.exercises[0], startFromState );
	}

	public void StartGame( Exercise exercise, FR.GameState startFromState = FR.GameState.ExerciseStart )
	{
		//Debug.LogError("Starting the exercise !");

		currentInteractionGroupIndex = 0;
		exerciseEnd = false;

		currentWorld = WorldFactory.use.CreateWorld(exercise.worldType, exercise.composition);
		
		FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
		HUDManager.use.SetMode( exercise.composition );
		//HUDManager.use.UpdateOperationIcons(1);
		HUDManager.use.UpdateOperationIcons(0); // make sure there are no icons visible here
		
		FRCamera.use.MoveToInteractionGroup( 1, 1, false );

		// TODO: Change this!!!
		//StartGameDebug( exercise.worldType, exercise.parts[0].fractions.ToArray(), exercise.composition, startFromState );
		
		LugusCoroutines.use.StartRoutine ( ExerciseRunRoutine(exercise) );

		ChangeState( startFromState );
	}

	protected bool exerciseEnd = false;
	protected IEnumerator ExerciseRunRoutine(Exercise exercise)
	{
		foreach( ExercisePart part in exercise.parts )
		{
			PreparePart( part );

			while( this.currentState != FR.GameState.WaitingForInput )
			{
				yield return null;
			}

			while( this.currentState != FR.GameState.PartEnd )
			{
				yield return null;
			}
		}
		Debug.Log ("GameManager:ExerciseRunRoutine : all parts processed, " + exercise.parts.Count + " in total");

		// exercise parts are all done, we should now continue on to the next Exercise
		// however, PartEndRoutine() automatically initiates a ChangeState to PartStart 
		// we need to intercept this and re-route it to ExerciseEnd instead
		exerciseEnd = true;
	}

	protected void PreparePart( ExercisePart part )
	{
		//Debug.LogError("Starting the part!");

		List<FractionRenderer> rends = RendererFactory.use.CreateRenderers( currentWorld, part.fractions, currentInteractionGroupIndex );

		currentExercisePart = part;
	}

	public void StartGameDebug( FR.WorldType type, Fraction[] fractions, FR.Target composition = FR.Target.BOTH, FR.GameState startFromState = FR.GameState.ExerciseStart )
	{
		currentWorld = WorldFactory.use.CreateDebugWorld(  type, fractions, composition, false );


		
		ChangeState( startFromState );
	}

	public void ChangeState(FR.GameState newState)
	{
		//Debug.Log (Time.frameCount + " GameManager:ChangeState : from " + currentState + " to " + newState );

		// use separate routine here, to make sure there's at least 1 frame between states
		LugusCoroutines.use.StartRoutine( ChangeStateRoutine(newState) );
	}

	protected IEnumerator ChangeStateRoutine(FR.GameState newState)
	{
		// make sure there's at least 1 frame between states
		// otherwhise, we can't wait for currentState to be a certain value in other coroutines / other parts of the setup
		yield return null;

		FR.GameState oldState = currentState;
		currentState = newState;
		
		Debug.Log (/*Time.frameCount + */" GameManager:ChangeStateRoutine : from " + oldState + " to " + newState );
		
		if( newState == FR.GameState.ExerciseStart )
		{
			if( oldState != FR.GameState.NONE && oldState != FR.GameState.ExerciseEnd )
			{
				Debug.LogError("GameManager:ChangeState : changed to Start from " + oldState + ". Not allowed! Oldstate should be NONE or ExerciseEnd");
			}
			else
			{
				LugusCoroutines.use.StartRoutine( ExerciseStartRoutine() );
			}
		}

		if( newState == FR.GameState.PartStart )
		{
			if( !exerciseEnd )
			{
				LugusCoroutines.use.StartRoutine( PartStartRoutine() );
			}
			else
			{
				Debug.Log (Time.frameCount + " GameManager:ChangeStateRoutine : REROUTE at exerciseEnd : from " + FR.GameState.PartStart + " to " + FR.GameState.ExerciseEnd );

				newState = FR.GameState.ExerciseEnd;
				currentState = FR.GameState.ExerciseEnd;
				exerciseEnd = false; // reset for next time
			}
		}
		
		if( newState == FR.GameState.PartStartSequence )
		{
			LugusCoroutines.use.StartRoutine( PartStartSequenceRoutine() );
		} 

		if( newState == FR.GameState.ProcessingOperation )
		{
			HUDManager.use.UpdateOperationIcons(0);
		}


		if( newState == FR.GameState.PartEndSequence )
		{			
			if( oldState != FR.GameState.ProcessingOperation )
			{
				Debug.LogError("GameManager:ChangeState : changed to EndSequence from " + oldState + ". Not allowed! Oldstate should be ProcessingOperation");
			}
			else
			{
				LugusCoroutines.use.StartRoutine( PartEndSequenceRoutine() );
			}
		}
		
		if( newState == FR.GameState.PartEnd )
		{
			LugusCoroutines.use.StartRoutine( PartEndRoutine() );
		}
		
		if( newState == FR.GameState.ExerciseEnd )
		{
			LugusCoroutines.use.StartRoutine( ExerciseEndRoutine() );
		}
		
		if( onStateChanged != null ) 
			onStateChanged( oldState, newState );
	}

	public IEnumerator ExerciseStartRoutine()
	{
		// black overlay (if not yet present)
		// fade from black

		ScreenFader.use.FadeIn(3.0f);
		
		ChangeState( FR.GameState.PartStart );

		yield break;
	}

	public IEnumerator PartStartRoutine()
	{
		if( exerciseEnd )
		{
			Debug.LogError("GameManager:PartStartRoutine : exerciseEnd is true! THIS SHOULDN'T HAPPEN, but might due to racing...");
			ChangeState( FR.GameState.ExerciseEnd );
			yield break;
		}

		ChangeState( FR.GameState.PartStartSequence );
		
		yield break;
	}

	public int currentInteractionGroupIndex = 0;

	public IEnumerator PartStartSequenceRoutine()
	{
		
		Fraction right = FindFraction(false);
		Fraction left = FindFraction(true);

		// Scaling to Vector3.zero doesn't keep the invidiual Characters' localPosition for some reason
		// so the (value > 6)-character doesn't float, it stays at the feet of the interactionCharacter after scaling back up... very weird
		// scaling to something very small but non-zero seems to fix it though...
		Vector3 scale1 = left.Numerator.Renderer.transform.localScale;
		left.Numerator.Renderer.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);//Vector3.zero;
		
		Vector3 scale2 = left.Denominator.Renderer.transform.localScale;
		left.Denominator.Renderer.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);//Vector3.zero;

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
		
		// TODO: set actual starting values of the portal here!
		Portal portal1 = RendererFactory.use.CreatePortal( currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].PortalEntry, 1, FR.Target.NUMERATOR );
		Portal portal2 = RendererFactory.use.CreatePortal( currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].PortalEntry, 1, FR.Target.DENOMINATOR );

		left.Numerator.Renderer.transform.position = currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].PortalEntry.position;
		left.Denominator.Renderer.transform.position = currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].PortalEntry.position;

		left.Numerator.Renderer.gameObject.ScaleTo(scale1).Time (2.0f).Execute();
		left.Denominator.Renderer.gameObject.ScaleTo(scale2).Time (2.0f).Execute();

		yield return new WaitForSeconds(2.0f);

		GameObject.Destroy( portal1.gameObject );
		GameObject.Destroy( portal2.gameObject );

		yield return left.Renderer.Animator.RotateTowards( FR.Target.BOTH, right.Renderer ).Coroutine;

		FRCamera.use.MoveToInteractionGroup( currentInteractionGroupIndex + 1, 1, true );
		left.Numerator.Renderer.Animator.RunTo( currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].Spawn1.position );
		left.Denominator.Renderer.Animator.RunTo( currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].Spawn1.position );

		yield return new WaitForSeconds(2.0f);
		
		yield return left.Renderer.Animator.RotateTowardsCamera().Coroutine;


		// TODO: should be shown here, and hidden after input accepted
		// MathInputManager.use.InitializeOperationIcons(1);

		HUDManager.use.UpdateOperationIcons( currentExercisePart.availableOperations );

		ChangeState( FR.GameState.WaitingForInput );

		yield break;
	}

	protected IEnumerator PartEndSequenceRoutine()
	{
		HUDManager.use.UpdateOperationIcons(0);


		Fraction left = FindFraction(true);

		Debug.Log (Time.frameCount + " before turn to exit ");

		// TODO: set actual expected values of the portal here!
		Portal portal1 = RendererFactory.use.CreatePortal( currentWorld.numerator.InteractionGroups[currentInteractionGroupIndex].PortalExit, 1, FR.Target.NUMERATOR );
		Portal portal2 = RendererFactory.use.CreatePortal( currentWorld.denominator.InteractionGroups[currentInteractionGroupIndex].PortalExit, 1, FR.Target.DENOMINATOR );


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

		GameObject.Destroy( portal1.gameObject );
		GameObject.Destroy( portal2.gameObject );

		left.Destroy();

		ChangeState( FR.GameState.PartEnd );
		yield break;
	}

	protected IEnumerator PartEndRoutine()
	{
		currentInteractionGroupIndex++;

		ChangeState( FR.GameState.PartStart );
		yield break;
	}
	
	public IEnumerator ExerciseEndRoutine()
	{
		// black overlay (if not yet present)
		// fade from black
		
		ScreenFader.use.FadeOut(3.0f);

		yield return new WaitForSeconds( 3.0f );
		
		yield break;
	}

	protected Fraction FindFraction(bool left)
	{
		List<Fraction> fractions = new List<Fraction>();
		
		NumberRenderer[] numbers = GameObject.FindObjectsOfType<NumberRenderer>();

		if( numbers.Length != 2 && numbers.Length != 4 )
		{
			Debug.LogError("FindFraction: expected # NumberRenderers is 2 or 4 (1 or 2 fractions), instead was " + numbers.Length);
		}
		
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
		ExerciseManager em = ExerciseManager.use;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
