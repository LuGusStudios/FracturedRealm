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
	public ArrowDrawer arrow1 = null;
	public ArrowDrawer arrow2 = null;
	public ArrowDrawer arrow3 = null;

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
		ChangeState( InputState.IDLE );
	}

	public IEnumerator ToggleInputDelayed(bool inputEnabled, float delay )
	{
		if( delay != 0.0f )
		{
			yield return new WaitForSeconds( delay );
		}

		//Debug.LogWarning("ToggleInput : " + inputEnabled + " // " + delay);

		acceptInput = inputEnabled;

		yield break;
	}

	public void SetMode( FR.Target mode )
	{
		// when called from inside editor :)
		if( currentOperationRendererLocation == null )
			Awake ();


		Transform operationButtons = HUDManager.use.transform.FindChild("OperationButtons");

		if( mode == FR.Target.BOTH )
		{
			currentOperationRendererLocation.position = new Vector3(10.24f, 13.80313f ,0.0f);

			operationButtons.position = new Vector3(0, 0, 0);
		}
		
		else if( mode == FR.Target.NUMERATOR)
		{
			currentOperationRendererLocation.position = new Vector3(10.24f, 13.80313f ,0.0f);

			operationButtons.position = new Vector3(0, -6.003259f, 0);
		}
		
		else if( mode == FR.Target.DENOMINATOR )
		{
			currentOperationRendererLocation.position = new Vector3(10.24f, 13.80313f ,0.0f);

			operationButtons.position = new Vector3(0, -6.003259f, 0);
		}
	}


	public void ChangeState(InputState newState)
	{
		InputState oldState = state;
		state = newState;

		if( newState == InputState.IDLE )
		{
			currentOperationIcon = null;
			targetSelectDragging = false;

			arrow1.renderer.enabled = false;
			arrow2.renderer.enabled = false;
			arrow3.renderer.enabled = false;
		}
		else if( newState == InputState.TARGET1 )
		{
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

				arrow1.MakePositive();
				arrow2.MakePositive();

				arrow1.MoveTowards( arrow1DefaultTarget, 0.4f );
				arrow2.MoveTowards( arrow2DefaultTarget, 0.4f );
			}
			else
			{
				arrow3.renderer.enabled = true;
				arrow3.MakePositive();
				arrow3.MoveTowards( arrow3DefaultTarget, 0.4f );
			}

			acceptInput = false;
			gameObject.StartLugusRoutine( ToggleInputDelayed(true, 0.45f) );
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
		if( !acceptInput ) 
			return;

		if( state == InputState.DISABLED )
			return;
		
		if( LugusInput.use.down || LugusInput.use.dragging || LugusInput.use.up )
		{
			//Debug.LogWarning(Time.frameCount + " : One of the mouse inputs was true." + this.state);

			//if( state == InputState.IDLE ) // not only on idle: allow change of operation at all times
			//{
				ProcessOperationSelect();
			//}
			if( state == InputState.TARGET1 || state == InputState.TARGET2 )
			{
				ProcessTargetSelect();
			}
		}
	}

	protected OperationIcon currentOperationIcon = null;
	protected Vector2 mouseDownPosition;

	public void ProcessOperationSelect()
	{
		if( LugusInput.use.down )
		{
			Transform hit = LugusInput.use.RaycastFromScreenPoint2D( LugusCamera.ui, LugusInput.use.lastPoint );
			if( hit == null )
			{
				//Debug.LogError("No operation hit ! " + LugusInput.use.lastPoint);
				return;
			}

			OperationIcon clickedIcon = null;

			if( operationIcons == null )
			{
				operationIcons = new List<OperationIcon>();
				operationIcons.AddRange ( GameObject.FindObjectsOfType<OperationIcon>() );
			} 

			// check if we've clicked one of the operation icons
			foreach( OperationIcon icon in operationIcons )
			{
				//Debug.Log("Checking icon " + icon.name + " -> " + hit.name + " from " + LugusCamera.ui.name);
				if( hit == icon.transform )
				{
					clickedIcon = icon;
					break;
				}
			}

			// if no icon selected, nothing to do here
			if( clickedIcon == null )
			{
				//Debug.LogError("No operation hit ! " + LugusInput.use.lastPoint);
				return; 
			}


			mouseDownPosition = LugusInput.use.lastPoint;

			if( currentOperationIcon != null )
			{
				if( currentOperationIcon == clickedIcon ) // clicked the same icon
				{
					iTween.PunchScale( currentOperationIcon.GetTopRenderer().gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.5f );

					// TODO: auditive feedback
					Debug.LogWarning("Selected same icon. Giving visual and auditive feedback");
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
			gameObject.StartLugusRoutine( MoveTopRendererRoutine() );
		}
	}

	protected IEnumerator MoveTopRendererRoutine()
	{
		if( currentOperationIcon == null )
		{
			Debug.LogError("MathInputManager:MoveTopRendererRoutine : currentOperationIcon was null! " + this.state );
			yield break;
		}

		acceptInput = false;

		// move with constant speed. Calculate time needed
		float time = Vector3.Distance( currentOperationIcon.GetTopRenderer().transform.position, currentOperationRendererLocation.position ) / 15.0f; // 1500px / s

		currentOperationIcon.GetTopRenderer().gameObject.MoveTo( currentOperationRendererLocation.position ).Time ( time ).Execute();

		yield return new WaitForSeconds( time );

		MathManager.use.SelectOperation( currentOperationIcon.type );

		ChangeState( InputState.TARGET1 );
	}

	protected bool targetSelectDragging = false;
	protected float targetSelectDownTime = 0.0f;
	public void ProcessTargetSelect()
	{
		ArrowDrawer arrow = null;
		//Vector3 arrowDefaultTarget;

		if( state == InputState.TARGET1 )
		{
			if( MathManager.use.currentOperation.RequiresTwoFractions() )
			{
				arrow = arrow1;
				//arrowDefaultTarget = arrow1DefaultTarget;
			}
			else
			{
				arrow = arrow3;
				//arrowDefaultTarget = arrow3DefaultTarget;
			}
		}
		else
		{
			arrow = arrow2;
			//arrowDefaultTarget = arrow2DefaultTarget;
		}

		bool outsideSlideArea = true;
		outsideSlideArea = !FRCamera.use.SlideScreenArea.Contains( LugusInput.use.lastPoint );

		bool tapDetected = (LugusInput.use.up && ((Time.time - targetSelectDownTime) < 0.5f)); // if within 0.3s of start release, we've tapped
		// this tapDetected is necessary in the following case:
		// SlideScreenArea (partly) overlaps one of the characters 
		// if it's just a quick tap, we still want the user to select the character as a target, even though the coordinates are in the ScreenSlideArea
		// so handle this as a special case
		// note: if we just release in the SlideArea, this case is handle by the ok boolean (needs to start outside)

		if(  LugusInput.use.down )
		{
			targetSelectDownTime = Time.time;

			if( outsideSlideArea )
			{
				// TODO: test : if down is close enough to end of first arrow (and we're target 2)
				// we will revert back to TARGET1 phase, because the user probably wants to re-align the arrow
				arrow.MakePositive();
				targetSelectDragging = true;
			}
		}

		// NOTE: keep this if, not else if... for some reason, it sometimes skips the dragging then (down stays on until up if quick clicks... wtf)
		if( (targetSelectDragging || tapDetected) && LugusInput.use.dragging  )
		{
			Vector3 worldPos = LugusInput.use.ScreenTo3DPoint( LugusInput.use.lastPoint, arrow.transform.position, LugusCamera.ui );//LugusCamera.ui.WorldToScreenPoint( currentOperationRendererLocation.position );
			arrow.targetTransform.position = worldPos;

			arrow.CreateArrow(true);
			arrow.underlyingPositionSet = false;
			
			arrow.renderer.enabled = true;
		}

		if( (targetSelectDragging || tapDetected) && LugusInput.use.up )
		{
			targetSelectDragging = false;

			Camera arrowCamera = null;


			// raycast from both camera's (numerator and denominator) separately
			Transform hit = LugusInput.use.RayCastFromMouse( LugusCamera.numerator );
			CharacterRenderer character = null;

			if( hit != null )
			{
				character = hit.GetComponent<CharacterRenderer>();
				arrowCamera = LugusCamera.numerator;
			}

			if( character == null )
			{
				// no character found in Numerator : try Denominator
				hit = LugusInput.use.RayCastFromMouse( LugusCamera.denominator );
				
				if( hit != null )
				{
					character = hit.GetComponent<CharacterRenderer>();
					arrowCamera = LugusCamera.denominator;
				}
			}

			if( character == null )
			{
				// move arrow back towards icon on the same direction it was dragged
				Vector3 arrowDirection = arrow.targetTransform.position - arrow.startTransform.position;
				arrowDirection.Normalize();

				arrow.MoveTowards( arrow.startTransform.position + (arrowDirection * 1.4f), 0.2f );

				Debug.LogWarning("MathInputManager:ProcessTargetSelect : NO CHARACTER COMPONENT ON HIT " + ((hit != null) ? hit.name : "") );
				return;
			}


			Debug.Log("MathInputManager:ProcessTargetSelect : Character hit for operation : " + character.name);

			FR.OperationMessage operationResult = FR.OperationMessage.None;
			if( state == InputState.TARGET1 )
				operationResult = MathManager.use.OnTarget1Selected( character.Number.Fraction );
			else if( state == InputState.TARGET2 )
				operationResult = MathManager.use.OnTarget2Selected( character.Number.Fraction );


			if( operationResult == FR.OperationMessage.None ) // no errors, selection success
			{
				// TODO: hide arrows
				// TODO: hide operationIconRenderer
				// TODO: Decrement OperationAmount

				// if final target, collapse arrows towards their targets
				if( state == InputState.TARGET2 )
				{
					arrow1.MakePositive();
					arrow2.MakePositive();

					arrow1.CollapseTowards( arrow1.targetTransform.position, 0.5f );
					arrow2.CollapseTowards( arrow2.targetTransform.position, 0.5f );
				}
				else
				{
					arrow3.MakePositive(); 
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
			else if( operationResult == FR.OperationMessage.Error_Requires2Fractions ) // not really an "error" here!
			{
				//Debug.LogError("Setting underlying position : " + character.transform.position + " // "+ character.transform.Path() );
				//arrow.underlyingWorldPosition = character.transform.position;//LugusInput.use.ScreenTo3DPoint( LugusInput.use.lastPoint, character.transform.position, LugusCamera.game );
				arrow.underlyingWorldPosition = LugusInput.use.ScreenTo3DPoint( LugusInput.use.lastPoint, character.transform.position, arrowCamera );
				arrow.usedWorldCamera = arrowCamera;

				ChangeState( InputState.TARGET2 );
			}
			else
			{
				// note: dirty, because same code as in the else if right above
				// however... no direct nice way to do this in 1 go, since it should be done before the changeState! 
				arrow.underlyingWorldPosition = LugusInput.use.ScreenTo3DPoint( LugusInput.use.lastPoint, character.transform.position, arrowCamera );
				arrow.usedWorldCamera = arrowCamera;
				
				// there was an error.
				// stay with target1, make arrow red
				Debug.LogWarning("MathInputManager:ProcessTargetSelect : error selecting, negative arrow. " + operationResult);
				arrow.MakeNegative();

				errorShowStartTime = Time.time; 
				errorMessage = operationResult;
			}
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

	public float errorShowStartTime = -10.0f;
	public FR.OperationMessage errorMessage = FR.OperationMessage.None;
	
	void OnGUI()
	{
		if( Time.time - errorShowStartTime < 3.0f )
		{
			GUILayout.BeginArea( new Rect(Screen.width / 2.0f - 100, Screen.height - 100, 200, 50), GUI.skin.box );	
			GUILayout.BeginVertical();

			GUILayout.Label("ERROR : " +  errorMessage );// + "\n" + Time.time + " - " + errorShowStartTime + " = "  + (Time.time - errorShowStartTime));

			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		if( !LugusDebug.debug )
			return;
		
		
		GUILayout.BeginArea( new Rect(Screen.width / 2.0f - 150.0f, 100, 300, 300) );	
		GUILayout.BeginVertical();
		
		GUILayout.Box("Current operation : " + MathManager.use.currentOperation);
		
		GUILayout.Box("Current state : " + MathManager.use.state);
		
		GUILayout.Box("Current gameState : " + GameManager.use.currentState );

		if( MathManager.use.operationInfo != null )
		{
			GUILayout.Box("Current fractions : " + MathManager.use.operationInfo.StartFraction + " -> " + MathManager.use.operationInfo.StopFraction);
		}

		if( MathManager.use.lastOperationMessage != FR.OperationMessage.None )
		{
			GUILayout.Box("\nCurrent message : " + MathManager.use.lastOperationMessage + "\n");
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
