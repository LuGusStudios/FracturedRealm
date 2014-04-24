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
	protected ArrowDrawer arrow3 = null;

	protected Vector3 arrow1DefaultTarget;
	protected Vector3 arrow2DefaultTarget;
	protected Vector3 arrow3DefaultTarget;

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

		if( arrow3 == null )
		{
			arrow3 = GameObject.Find ("Arrow3").GetComponent<ArrowDrawer>();
		}

		// 45 degrees to the left
		arrow1DefaultTarget = arrow1.transform.position + new Vector3( -1.0f, -1.0f, 0.0f );
		// 45 degrees to the right
		arrow2DefaultTarget = arrow2.transform.position + new Vector3(  1.0f, -1.0f, 0.0f );
		// straight down
		arrow3DefaultTarget = arrow3.transform.position + new Vector3(   0.0f, -1.5f, 0.0f );
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
			if( icon.type == FR.OperationType.SIMPLIFY || icon.type == FR.OperationType.DOUBLE )
				icon.OperationAmount = -1;
			else
				icon.OperationAmount = Random.Range(0, 3);
		}
	}


	public void ChangeState(InputState newState)
	{
		InputState oldState = state;
		state = newState;

		if( newState == InputState.IDLE )
		{
			// TODO: hide arrows
			arrow1.renderer.enabled = false;
			arrow2.renderer.enabled = false;
			arrow3.renderer.enabled = false;
		}
		else if( newState == InputState.TARGET1 )
		{
			// TODO: show arrows
			arrow1.startTransform.localPosition = Vector3.zero;
			arrow2.startTransform.localPosition = Vector3.zero;
			arrow3.startTransform.localPosition = Vector3.zero;
			arrow1.targetTransform.position = arrow1.startTransform.position;
			arrow2.targetTransform.position = arrow2.startTransform.position;
			arrow3.targetTransform.position = arrow3.startTransform.position;
			
			if( MathManager.use.currentOperation.RequiresTwoFractions() )
			{
				arrow1.renderer.enabled = true;
				arrow2.renderer.enabled = true;

				arrow1.MoveTowards( arrow1DefaultTarget, 0.4f );
				arrow2.MoveTowards( arrow2DefaultTarget, 0.4f );
			}
			else
			{
				arrow3.renderer.enabled = true;
				arrow3.MoveTowards( arrow3DefaultTarget, 0.4f );
			}
		}
		else if( newState == InputState.TARGET2 )
		{

		}
		else if( newState == InputState.DISABLED )
		{
			currentOperationIcon = null;
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
		if( LugusInput.use.KeyDown(KeyCode.D) )
		{
			operationIcons[0].OperationAmount += 2;
		}
		if( LugusInput.use.KeyDown(KeyCode.F) )
		{
			operationIcons[0].GetTopRenderer().gameObject.MoveTo( new Vector3(LugusUtil.UIWidth, LugusUtil.UIHeight,0) ).Time(1.0f).Execute();
			Destroy( operationIcons[0].GetTopRenderer().gameObject, 2.0f );
			operationIcons[0].OperationAmount -= 1;
			
			operationIcons[0].GetTopRenderer().gameObject.MoveTo( new Vector3(LugusUtil.UIWidth, LugusUtil.UIHeight,0) ).Time(1.0f).Execute();
			Destroy( operationIcons[0].GetTopRenderer().gameObject, 2.0f );
			operationIcons[0].OperationAmount -= 1;
		}
		if( LugusInput.use.KeyDown(KeyCode.G) )
		{
			operationIcons[0].OperationAmount = -1;
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

					//Debug.LogError("Selected another input icon : deselecting previous one and starting over");
				}
			} 

			// TODO: add and keep clicked-offset so texture doesn;t "snap" to the mouse position
			currentOperationIcon = clickedIcon;

			//Debug.LogError("Begun selecting operationIcon : " + currentOperationIcon.name);
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

			
			//Debug.LogError("Selected operationIcon : " + currentOperationIcon.name);
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
		Vector3 arrowDefaultTarget;

		if( state == InputState.TARGET1 )
		{
			if( MathManager.use.currentOperation.RequiresTwoFractions() )
			{
				arrow = arrow1;
				arrowDefaultTarget = arrow1DefaultTarget;
			}
			else
			{
				arrow = arrow3;
				arrowDefaultTarget = arrow3DefaultTarget;
			}
		}
		else
		{
			arrow = arrow2;
			arrowDefaultTarget = arrow2DefaultTarget;
		}

		if( LugusInput.use.down )
		{
			// TODO: test : if down is close enough to end of first arrow (and we're target 2)
			// we will revert back to TARGET1 phase, because the user probably wants to re-align the arrow
			arrow.MakePositive();
		}
		else if( LugusInput.use.dragging )
		{
			Vector3 worldPos = LugusInput.use.ScreenTo3DPoint( LugusInput.use.lastPoint, arrow.transform.position, LugusCamera.ui );//LugusCamera.ui.WorldToScreenPoint( currentOperationRendererLocation.position );
			arrow.targetTransform.position = worldPos;

			//arrow.CreateArrowScreen( screenPos, LugusInput.use.lastPoint, true );
			arrow.CreateArrow(true);

			arrow.renderer.enabled = true;
		}
		else if( LugusInput.use.up )
		{
			/*
			Transform hit = LugusInput.use.RayCastFromMouse( LugusCamera.numerator );
			
			if( hit == null )
				hit = LugusInput.use.RayCastFromMouse( LugusCamera.denominator );

			Debug.LogWarning("HITTING : " + hit.name);


			Character character = hit.GetComponent<Character>();
			
			if( character == null )
			{
				hit = LugusInput.use.RayCastFromMouse( LugusCamera.denominator );
				if( hit != null )
					character = hit.GetComponent<Character>();
				else
					return;

			}
			*/

			
			Transform hit = LugusInput.use.RayCastFromMouse( LugusCamera.numerator );
			Character character = null;

			if( hit != null )
			{
				character = hit.GetComponent<Character>();
			}

			if( character == null )
			{
				// no character found in Numerator : try Denominator
				hit = LugusInput.use.RayCastFromMouse( LugusCamera.denominator );
				
				if( hit != null )
				{
					character = hit.GetComponent<Character>();
				}
			}

			if( character == null )
			{
				// move arrow back towards icon on the same direction it was dragged
				Vector3 arrowDirection = arrow.targetTransform.position - arrow.startTransform.position;
				arrowDirection.Normalize();

				arrow.MoveTowards( arrow.startTransform.position + (arrowDirection * 1.4f), 0.2f );

				Debug.LogWarning("NO CHARACTER COMPONENT ON HIT " + ((hit != null) ? hit.name : "") );
				return;
			}


			Debug.LogWarning("Character hit for operation : " + character.name);

			FR.OperationMessage operationResult = FR.OperationMessage.None;
			if( state == InputState.TARGET1 )
				operationResult = MathManager.use.OnTarget1Selected( character.Number.Fraction );
			else if( state == InputState.TARGET2 )
				operationResult = MathManager.use.OnTarget2Selected( character.Number.Fraction );

			if( operationResult == FR.OperationMessage.None )
			{
				// TODO: hide arrows
				// TODO: hide operationIconRenderer
				// TODO: Decrement OperationAmount

				if( state == InputState.TARGET2 )
				{
					arrow1.CollapseTowards( arrow1.targetTransform.position, 0.5f );
					arrow2.CollapseTowards( arrow2.targetTransform.position, 0.5f );
				}
				else
				{
					arrow3.CollapseTowards( arrow3.targetTransform.position, 0.5f );
				}
				
				Transform iconRenderer = currentOperationIcon.GetTopRenderer();
				if( currentOperationIcon.OperationAmount > -1 )
				{
					// move iconRenderer out of screen
					iconRenderer.transform.parent = null;
					iconRenderer.gameObject.MoveTo( iconRenderer.position.y( LugusUtil.UIHeight + 3.0f ) ).Time ( 1.3f ).EaseType(iTween.EaseType.easeInBack).Execute();

					currentOperationIcon.OperationAmount -= 1;
					
					GameObject.Destroy( iconRenderer.gameObject, 1.5f );
				}
				else
				{
					// move iconRenderer back to normal position (infinite uses)
					iconRenderer.gameObject.MoveTo( currentOperationIcon.GetTopRendererPosition() ).Time ( 1.3f ).EaseType(iTween.EaseType.easeInBack).Execute();
				}


				ChangeState( InputState.DISABLED );
				MathManager.use.onOperationCompleted += OnOperationCompleted;

				MathManager.use.ProcessCurrentOperation();
			}
			else if( operationResult == FR.OperationMessage.Error_Requires2Fractions )
			{
				ChangeState( InputState.TARGET2 );
			}
			else
			{
				// there was an error.
				// stay with target1, make arrow red
				Debug.LogWarning("THERE WAS ERROR. NEGATIVE ARROW BABY");
				arrow.MakeNegative();
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
