using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MathInputManager : LugusSingletonExisting<MathInputManager> 
{	
	public bool acceptInput = true;
	
	[HideInInspector]
	protected IOperation currentOperation = null;
	
	protected List<IOperation> operations = new List<IOperation>();
	protected List<IOperationVisualizer> operationVisualizers = new List<IOperationVisualizer>();
	
	// FIXME: move this to a GUI-manager like-thing
	protected List<OperationIcon> operationIcons;// = new List<OperationIcon>();

	void Awake () 
	{
		operations.Add( new OperationAdd() );
		operations.Add( new OperationSubtract() );
		operations.Add( new OperationSimplify() );
		operations.Add( new OperationDouble() );
		operations.Add( new OperationMultiply() );
		operations.Add( new OperationDivide() );
		
		operationVisualizers.Add( new OperationVisualizerAdd() );
		operationVisualizers.Add( new OperationVisualizerSubtract() );
		operationVisualizers.Add( new OperationVisualizerSimplify() );
		operationVisualizers.Add( new OperationVisualizerDouble() );
		operationVisualizers.Add( new OperationVisualizerMultiply() );
		operationVisualizers.Add( new OperationVisualizerDivide() );
		
		//currentOperation = operations[0];
	}

	public void Start()
	{
		InitializeOperationIcons();
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
	                                    

	public IOperation GetOperation(FR.OperationType type)
	{
		IOperation output = null;
		foreach( IOperation operation in operations )
		{
			if( operation.type == type )
				output = operation;
		}
		
		return output;
	}
	
	public IOperationVisualizer GetVisualizer(FR.OperationType type)
	{
		IOperationVisualizer visualizer = null;
		foreach( IOperationVisualizer viz in operationVisualizers )
		{
			if( viz.type == type )
				visualizer = viz;
		}
		
		return visualizer;
	}
	
	
	public void SelectOperation(FR.OperationType type)
	{
		
		currentOperation = GetOperation(type);
		
		if( currentOperation == null ) 
		{
			Debug.LogError("MathInputManager:SelectOperation : no operation handler found for " + type);
			currentOperation = operations[0];
		}
		
		Debug.Log("Selected operation : " + currentOperation.type);	
	}
	
	/*
	void OnGUI()
	{
		float xStart = 50;
		float yStart = 10; 
		
		bool currentSolverChanged = false;
		
		foreach( IOperation solver in operations )//System.Enum.GetValues(typeof(SolverTool)) )
		{
			if( solver.type == OperationType.NONE )
				continue;
			
			float height = 50;
			if( solver == currentOperation )
				height = 100;
			
			if( GUI.Button( new Rect(xStart, yStart, 120, height), "" + solver.type ) )
			{
				currentOperation = solver;
				currentSolverChanged = true;
			}
			
			
			xStart += 150;
		}
		
		if( currentSolverChanged )
		{
			// ... do other stuff
			Debug.Log("Current solver changed to " + currentOperation.type);
		}
	}
	*/
	
	protected Vector3 currentStart;
	protected Vector3 currentEnd;
	
	protected OperationState currentState = null;
	
	protected void ResetOperationState()
	{
		// TODO: deselect characters that were selected
		
		currentState = null;
	}
	
	public void ProcessClick()
	{
		Transform hit = LugusInput.use.RayCastFromMouseDown( LugusCamera.numerator );

		if( hit == null )
			hit = LugusInput.use.RayCastFromMouseDown( LugusCamera.denominator );

		if( hit == null )
		{
			ResetOperationState();
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
			ResetOperationState();
			return;
		}
		
		// 1. New operation : selecting first target
		if( currentState == null )
		{
			currentState = new OperationState(currentOperation.type, character.Number.Fraction, null); 
			currentState.StartNumber = character.Number;
			
			bool ok = currentOperation.AcceptState( currentState );
			
			if( ok )
			{
				//currentState.StartFraction.Numerator.Renderer.transform.localScale /= 2.0f;
				//currentState.StartFraction.Denominator.Renderer.transform.localScale /= 2.0f;
				
				if( !currentOperation.RequiresTwoFractions() )
				{
					ProcessCurrentOperation();
					return;
				}
			}
			else 
			{
				// TODO: deselect all + show error message?
				// OnOperationStartFailed();
				ResetOperationState();
				return;
			}
		}
		
		// FINAL: can be removed 
		if( currentState.StopFraction != null )
		{
			Debug.LogError("MathInputManager:ProcessClick : clicked with a full operationState... shouldn't be possible!");
		}
		
		// 2. Operation in progress : select 2nd target
		if( currentState.StartFraction != null )
		{
			if( currentState.StartNumber == character.Number )
				return;
			
			currentState.StopFraction = character.Number.Fraction;
			currentState.StopNumber = character.Number;
			
			
			bool ok = currentOperation.AcceptState( currentState );
			
			// FINAL: can be removed 
			if( !currentOperation.RequiresTwoFractions() )
			{
				Debug.LogError("MathInputManager:ProcessClick : RequiresTwoFractions() is false, yet it didn't execute after select 1...");
			}
			
			
			if( ok )
			{
				ProcessCurrentOperation();  
			}
			else
			{
				Debug.Log ("ProcessClick: 2nd target is not valid");
				// Don't just reset operation state completely... the user might want to select another thing if he misclicked
				currentState.StopFraction = null;
				currentState.StopNumber = null;
			}
		}
		
	}
	
	protected void ProcessCurrentOperation()
	{
		// TODO: ...
		
		Debug.LogError ("ProcessCurrentOperation " + currentOperation.type);
		
		
		OperationState outcome = currentOperation.Process( currentState );
		
		IOperationVisualizer visualizer = GetVisualizer( currentState.Type );
		
		if( visualizer == null ) 
		{
			Debug.LogError("MathInputManager:ProcessCurrentOperation : no visualizer found for " + currentState.Type);
		}
		else
		{
			StartCoroutine( visualizer.Visualize(currentState, outcome) );
		}
		
		
		ResetOperationState();
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
		if( currentOperation == null )
			return;
		
		if( LugusInput.use.down )
		{
			ProcessClick();
			
			/*
			Character character = hit.GetComponent<Character>();
			if( character == null )
				return;
			
			if( character.floor != Character.Floor.TOPSIDE )
				return;
			
			startPointScreen = LuGusInput.i.lastPoint;
			
			startCharacter = character;
			
			Debug.Log("OnInputStart:add : selecting "+ startCharacter + " " + startCharacter.significantOther);
			
			*/
			//visualHelper.SelectCharacter( startCharacter );
			//visualHelper.SelectCharacter( startCharacter.significantOther );
		}
		
		/*
		if( LuGusInput.use.dragging )
		{
			//currentEnd = LuGusInput.i.lastPoint;
			
			
			Transform hit = LuGusInput.use.RayCastFromMouse();
			if( hit == null )
				return;
			
			Character numberRenderer = hit.GetComponent<Character>();
			if( numberRenderer == null )
				return;
			
			if( currentState.StartNumber == numberRenderer.Number )
				return;
			
			currentState.StopNumber = numberRenderer.Number;
			
			
			bool ok = currentOperation.AcceptState(currentState);
			
			if( ok )
			{
				//visualHelper.SelectCharacter( stopCharacter );
				//visualHelper.SelectCharacter( stopCharacter.significantOther );
			}
			
		}
		
		
		// todo: maybe do this if( !dragging ) ?
		if( LuGusInput.use.up )
		{
			
			if( currentState == null )
				return;
			
			
			if( currentState.StopFraction == null )
			{
				// TODO: undo all
				return;
			}
			
			
			bool ok = currentOperation.Process( currentState );
			
			// TODO!
		}
		*/
	}
	
	void OnGUI()
	{
		if( !LugusDebug.debug )
			return;
		
		
		GUILayout.BeginArea( new Rect(0, 100, 300, 300) );	
		GUILayout.BeginVertical();
		
		GUILayout.Box("Current operation : " + currentOperation);
		
		if( currentState != null )
			GUILayout.Box("Current state : " + currentState.StartFraction + " -> " + currentState.StopFraction);
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
