using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MathInputManager : LugusSingletonExisting<MathInputManager> 
{	
	public enum InputState
	{
		NONE = -1,

		IDLE = 1,
		TARGET1 = 2,
		TARGET2 = 3,
		DISABLED = 4
	}

	public bool acceptInput = true;

	public InputState state = InputState.NONE;

	protected List<OperationIcon> operationIcons;

	protected Transform currentOperationRendererLocation = null;
	protected ArrowDrawer arrow1 = null;
	protected ArrowDrawer arrow2 = null;

	void Awake () 
	{
		MathManager m = MathManager.use; // make sure it exists

		if( currentOperationRendererLocation == null )
		{
			currentOperationRendererLocation = GameObject.Find ("CurrentOperationLocation").transform;
		}

		if( arrow1 == null )
		{
			arrow1 = GameObject.Find ("Arrow1").GetComponent<ArrowDrawer>();
		}

		if( arrow2 == null )
		{
			arrow2 = GameObject.Find ("Arrow2").GetComponent<ArrowDrawer>();
		}

	}

	public void Start()
	{
		InitializeOperationIcons();

		ChangeState( InputState.IDLE );
	}

	protected void InitializeOperationIcons()
	{
		operationIcons = new List<OperationIcon>();
		operationIcons.AddRange ( GameObject.FindObjectsOfType<OperationIcon>() );
		
		foreach( OperationIcon icon in operationIcons )
		{
			icon.OperationAmount = Random.Range(-1, 6);
		}
	}


	public void ChangeState(InputState newState)
	{
		InputState oldState = state;
		state = newState;

		if( newState == InputState.IDLE )
		{
			// TODO: hide arrows
		}
		else if( newState == InputState.TARGET1 || newState == InputState.TARGET2 )
		{
			// TODO: show arrows
		}
		else if( newState == InputState.DISABLED )
		{
			// TODO: hide arrows
		}

		Debug.Log("MathInputManager:ChangeState : changed from " + oldState + " to " + newState );
	}

	
	void Update () 
	{
		if( !acceptInput) 
			return;
		
		if( LugusInput.use.KeyDown(KeyCode.S) ) // "spawn"
		{
			InitializeOperationIcons();
		}
		
		// we need to have selected an operation to be able to continue
		//if( MathManager.use.currentOperation == null )
		//	return;
		
		if( LugusInput.use.down || LugusInput.use.dragging || LugusInput.use.up )
		{
			//if( state == InputState.IDLE )
			//{
				ProcessOperationSelect();
			//}
			if( state == InputState.TARGET1 || state == InputState.TARGET2 )
			{
				ProcessTargetSelect();
			}
			else if( state == InputState.DISABLED )
			{
				// do nothing here
			}
		}
	}

	protected OperationIcon currentOperationIcon = null;
	protected Vector2 mouseDownPosition;

	public void ProcessOperationSelect()
	{
		if( LugusInput.use.down )
		{
			Transform hit = LugusInput.use.RayCastFromMouse( LugusCamera.ui );
			if( hit == null )
				return;

			OperationIcon clickedIcon = null;

			// check if we've clicked one of the operation icons
			foreach( OperationIcon icon in operationIcons )
			{
				if( hit == icon.transform )
				{
					clickedIcon = icon;
					break;
				}
			}

			// if no icon selected, nothing to do here
			if( clickedIcon == null )
				return; 

			mouseDownPosition = LugusInput.use.lastPoint;

			if( currentOperationIcon != null )
			{
				if( currentOperationIcon == clickedIcon ) // clicked the same icon
				{
					iTween.PunchScale( currentOperationIcon.GetTopRenderer().gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.5f );

					// TODO: auditive feedback
					Debug.LogError("Selected same icon. Giving visual and auditive feedback");
				}
				else
				{
					currentOperationIcon.GetTopRenderer().gameObject.MoveTo( currentOperationIcon.GetTopRendererPosition() ).Time (0.5f).Execute();
					currentOperationIcon = null;

					ChangeState( InputState.IDLE );

					Debug.LogError("Selected another input icon : deselecting previous one and starting over");
				}
			} 

			// TODO: add and keep clicked-offset so texture doesn;t "snap" to the mouse position
			currentOperationIcon = clickedIcon;

			Debug.LogError("Begun selecting operationIcon : " + currentOperationIcon.name);
		}
		else if( LugusInput.use.dragging && state == InputState.IDLE && currentOperationIcon != null )
		{
			// TODO: possibly cache TopRenderer so we don't have to request it every frame
			Transform renderer = currentOperationIcon.GetTopRenderer();
			renderer.transform.position = LugusInput.use.ScreenTo3DPoint( renderer );
		}
		else if( LugusInput.use.up && state == InputState.IDLE && currentOperationIcon != null )
		{
			//if( Vector2.Distance( LugusInput.use.lastPoint, mouseDownPosition ) < 10.0f )
			//{
				// user probably misclicked and wasn't really planning on selecting this item... ignore it
			//	currentOperationIcon = null;
			//	return;
			//}

			
			Debug.LogError("Selected operationIcon : " + currentOperationIcon.name);
			//currentOperationIcon.OperationAmount -= 1;

			gameObject.StartLugusRoutine( MoveTopRendererRoutine() );
		}
	}

	protected IEnumerator MoveTopRendererRoutine()
	{
		// move with constant speed. Calculate time needed
		float time = Vector3.Distance( currentOperationIcon.GetTopRenderer().transform.position, currentOperationRendererLocation.position ) / 15.0f; // 1500px / s

		currentOperationIcon.GetTopRenderer().gameObject.MoveTo( currentOperationRendererLocation.position ).Time ( time ).Execute();

		yield return new WaitForSeconds( time );

		MathManager.use.SelectOperation( currentOperationIcon.type );

		ChangeState( InputState.TARGET1 );
	}

	public void ProcessTargetSelect()
	{
		ArrowDrawer arrow = null;

		if( state == InputState.TARGET1 )
			arrow = arrow1;
		else
			arrow = arrow2;

		if( LugusInput.use.dragging )
		{
			Vector3 screenPos = LugusCamera.ui.WorldToScreenPoint( currentOperationRendererLocation.position );
			arrow.CreateArrow( screenPos, LugusInput.use.lastPoint, true );

			arrow.renderer.enabled = true;
		}
		else if( LugusInput.use.up )
		{
			Transform hit = LugusInput.use.RayCastFromMouse( LugusCamera.numerator );
			
			if( hit == null )
				hit = LugusInput.use.RayCastFromMouse( LugusCamera.denominator );


			if( hit == null )
				return; 


			Debug.LogWarning("HITTING : " + hit.name);


			Character character = hit.GetComponent<Character>();
			
			if( character == null )
			{
				hit = LugusInput.use.RayCastFromMouse( LugusCamera.denominator );
				if( hit != null )
					character = hit.GetComponent<Character>();
			}

			if( character == null )
			{
				Debug.LogError("NO CHARACTER COMPONENT ON HIT " + hit.name);
				return;
			}

			Debug.LogWarning("Character : " + character.name);

			bool operationComplete = false;
			if( state == InputState.TARGET1 )
				operationComplete = MathManager.use.OnTarget1Selected( character.Number.Fraction );
			else if( state == InputState.TARGET2 )
				operationComplete = MathManager.use.OnTarget2Selected( character.Number.Fraction );

			if( operationComplete )
			{
				// TODO: hide arrows
				// TODO: hide operationIconRenderer
				// TODO: Decrement OperationAmount

				ChangeState( InputState.DISABLED );
				MathManager.use.onOperationCompleted += OnOperationCompleted;

				MathManager.use.ProcessCurrentOperation();
			}
			else
			{
				ChangeState( InputState.TARGET2 );
			}
		}
		else
		{
			// hide arrow
			arrow.renderer.enabled = false;
		}
	}

	public void OnOperationCompleted(IOperation operation)
	{
		MathManager.use.onOperationCompleted -= OnOperationCompleted;
		ChangeState( InputState.IDLE );
	}


	/*
	public void ProcessClick()
	{
		Transform hit = LugusInput.use.RayCastFromMouseDown( LugusCamera.numerator );

		if( hit == null )
			hit = LugusInput.use.RayCastFromMouseDown( LugusCamera.denominator );

		if( hit == null )
		{
			MathManager.use.ResetOperationState();
			return;
		}
			
		Debug.Log ("Hitting + " + hit.name);
		
		Character character = hit.GetComponent<Character>();
		
		if( character == null )
		{
			hit = LugusInput.use.RayCastFromMouseDown( LugusCamera.denominator );
			if( hit != null )
				character = hit.GetComponent<Character>();
		}

		if( character == null )
		{
			MathManager.use.ResetOperationState();
			return;
		}
		
		// 1. New operation : selecting first target
		if( MathManager.use.currentState == null )
		{
			MathManager.use.currentState = new OperationState(MathManager.use.currentOperation.type, character.Number.Fraction, null); 
			MathManager.use.currentState.StartNumber = character.Number;
			
			bool ok = MathManager.use.currentOperation.AcceptState( MathManager.use.currentState );
			
			if( ok )
			{
				//currentState.StartFraction.Numerator.Renderer.transform.localScale /= 2.0f;
				//currentState.StartFraction.Denominator.Renderer.transform.localScale /= 2.0f;
				
				if( !MathManager.use.currentOperation.RequiresTwoFractions() )
				{
					MathManager.use.ProcessCurrentOperation();
					return;
				}
			}
			else 
			{
				// TODO: deselect all + show error message?
				// OnOperationStartFailed();
				MathManager.use.ResetOperationState();
				return;
			}
		}
		
		// FINAL: can be removed 
		if( MathManager.use.currentState.StopFraction != null )
		{
			Debug.LogError("MathInputManager:ProcessClick : clicked with a full operationState... shouldn't be possible!");
		}
		
		// 2. Operation in progress : select 2nd target
		if( MathManager.use.currentState.StartFraction != null )
		{
			if( MathManager.use.currentState.StartNumber == character.Number )
				return;
			
			MathManager.use.currentState.StopFraction = character.Number.Fraction;
			MathManager.use.currentState.StopNumber = character.Number;
			
			
			bool ok = MathManager.use.currentOperation.AcceptState( MathManager.use.currentState );
			
			// FINAL: can be removed 
			if( !MathManager.use.currentOperation.RequiresTwoFractions() )
			{
				Debug.LogError("MathInputManager:ProcessClick : RequiresTwoFractions() is false, yet it didn't execute after select 1...");
			}
			
			
			if( ok )
			{
				MathManager.use.ProcessCurrentOperation();  
			}
			else
			{
				Debug.Log ("ProcessClick: 2nd target is not valid");
				// Don't just reset operation state completely... the user might want to select another thing if he misclicked
				MathManager.use.currentState.StopFraction = null;
				MathManager.use.currentState.StopNumber = null;
			}
		}
		
	}
	*/

	
	void OnGUI()
	{
		if( !LugusDebug.debug )
			return;
		
		
		GUILayout.BeginArea( new Rect(0, 100, 300, 300) );	
		GUILayout.BeginVertical();
		
		GUILayout.Box("Current operation : " + MathManager.use.currentOperation);
		
		if( MathManager.use.currentState != null )
			GUILayout.Box("Current state : " + MathManager.use.currentState.StartFraction + " -> " + MathManager.use.currentState.StopFraction);
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
