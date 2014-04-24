using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MathManager : LugusSingletonRuntime<MathManager>  
{
	[HideInInspector]
	public IOperation currentOperation = null;
	
	protected List<IOperation> operations = new List<IOperation>();
	protected List<IOperationVisualizer> operationVisualizers = new List<IOperationVisualizer>();


	public OperationState currentState = null;

	public delegate void OnOperationCompleted(IOperation operation);
	public OnOperationCompleted onOperationCompleted;

	public void SetupLocal()
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
			Debug.LogError("MathManager:SelectOperation : no operation handler found for " + type);
			currentOperation = operations[0];
		}
		
		Debug.Log("Selected operation : " + currentOperation.type);	
	}

	public void ResetOperationState()
	{
		currentState = null;
	}

	public FR.OperationMessage OnTarget1Selected(Fraction fr)
	{
		currentState = new OperationState(currentOperation.type, fr, null); 
		
		if( FRTargetExtensions.TargetFromFraction(fr).HasNumerator() )
			currentState.StartNumber = fr.Numerator;
		else
			currentState.StartNumber = fr.Denominator;

		
		bool ok = currentOperation.AcceptState( currentState );

		if( !ok )
		{
			return currentOperation.lastMessage;
		}
		else
		{
			if( currentOperation.RequiresTwoFractions() )
			{
				return FR.OperationMessage.Error_Requires2Fractions;
			}
			else
			{
				return FR.OperationMessage.None;
			}
		}
	}

	public FR.OperationMessage OnTarget2Selected(Fraction fr)
	{
		if( currentState == null || currentState.StartFraction == null )
		{
			Debug.LogError("MathManager:OnTarget2Selected : currentState was invalid to accept 2nd target!");
			return FR.OperationMessage.Error_Requires2Fractions;
		}

		if( currentState.StartFraction == fr )
		{
			Debug.LogWarning("MathManager:OnTarget2Selected : cannot select the same target twice. Ignoring");
			return FR.OperationMessage.Error_IdenticalTargets;
		}

		if( !currentOperation.RequiresTwoFractions() )
		{
			Debug.LogError("MathManager:OnTarget2Selected : RequiresTwoFractions() is false, yet it didn't execute after select 1...");
			return FR.OperationMessage.Error_Requires1Fraction;
		}


		currentState.StopFraction = fr;
		if( FRTargetExtensions.TargetFromFraction(fr).HasNumerator() )
			currentState.StopNumber = fr.Numerator;
		else
			currentState.StopNumber = fr.Denominator;
		
		
		bool ok = currentOperation.AcceptState( currentState );

		if( !ok )
		{
			Debug.LogError("MathManager:OnTarget2Selected : operation doesn't accept state ");
			return currentOperation.lastMessage;
		}
		else
		{
			return FR.OperationMessage.None;
		}
	}

	public void ProcessCurrentOperation()
	{
		Debug.LogError ("ProcessCurrentOperation " + currentOperation.type);

		this.gameObject.StartLugusRoutine( ProcessCurrentOperationRoutine() );
	}

	protected IEnumerator ProcessCurrentOperationRoutine()
	{
		OperationState outcome = currentOperation.Process( currentState );
		
		IOperationVisualizer visualizer = GetVisualizer( currentState.Type );
		
		if( visualizer == null ) 
		{
			Debug.LogError("MathInputManager:ProcessCurrentOperation : no visualizer found for " + currentState.Type);
		}
		else
		{
			yield return gameObject.StartLugusRoutine( visualizer.Visualize(currentState, outcome) ).Coroutine;
		}
		
		if( onOperationCompleted != null )
			onOperationCompleted( currentOperation );
		
		ResetOperationState();

		yield break;
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
