using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MathManager : LugusSingletonRuntime<MathManager>  
{
	public enum MathState
	{
		None = -1,

		Idle = 1,
		OperationSelected = 2, // waiting for OperationSelected
		Target1Selected = 3, // waiting for Target2Selected
		Target2Selected = 4, // waiting for Visualizing
		Visualizing = 5 // waiting until visualisation is completed
	}


	[HideInInspector]
	public IOperation currentOperation = null;
	
	protected List<IOperation> operations = new List<IOperation>();
	protected List<IOperationVisualizer> operationVisualizers = new List<IOperationVisualizer>();

	
	public MathState state = MathState.None;
	public FR.OperationMessage lastOperationMessage = FR.OperationMessage.None;

	public OperationState operationInfo = null;

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

		Reset();
	}

	public void ChangeState(MathState newState)
	{
		MathState oldState = state;
		state = newState;

		if( oldState == MathState.Idle && newState != MathState.OperationSelected )
		{
			Debug.LogError("MathManager:ChangeState : idle should go to OperationSelected, instead of " + newState);
		}
		else if( oldState == MathState.OperationSelected && newState != MathState.Target1Selected )
		{
			Debug.LogError("MathManager:ChangeState : OperationSelected should go to target1Selected, instead of " + newState);
		}
		else if( oldState == MathState.Target1Selected && newState != MathState.Target2Selected && newState != MathState.Visualizing )
		{
			Debug.LogError("MathManager:ChangeState : target1Selected should go to target2 or visualizing, instead of " + newState);
		}
		else if( oldState == MathState.Visualizing && newState != MathState.Idle )
		{
			Debug.LogError("MathManager:ChangeState : Visualizing should go to idle, instead of " + newState);
		}

		Debug.Log("MathManager:ChangeState : changed from " + oldState + " to " + newState );
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
		//IOperationVisualizer visualizer = null;

		/*
		foreach( IOperationVisualizer viz in operationVisualizers )
		{
			if( viz.type == type )
				visualizer = viz;
		}
		*/

		FRAnimationData animationData = null;

		do
		{
			animationData = FRAnimations.use.GetRandomStarterAnimation( type );
		}
		while( animationData.visualizer.GetImplementationStatus() == FR.VisualizerImplementationStatus.NONE );

		Debug.Log("MathManager:GetVisualizer : chosen visualization is " + animationData.VisualizerClassName() );

		return animationData.visualizer;
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
		
		ChangeState( MathState.OperationSelected );
	}

	public void Reset()
	{
		operationInfo = null;
		ChangeState( MathState.Idle );
	}

	public FR.OperationMessage OnTarget1Selected(Fraction fr)
	{
		lastOperationMessage = FR.OperationMessage.None;

		operationInfo = new OperationState(currentOperation.type, fr, null); 
		
		if( FRTargetExtensions.TargetFromFraction(fr).HasNumerator() )
			operationInfo.StartNumber = fr.Numerator;
		else
			operationInfo.StartNumber = fr.Denominator;

		
		bool ok = currentOperation.AcceptState( operationInfo );

		if( !ok )
		{
			lastOperationMessage = currentOperation.lastMessage;

			// state not accepted -> stay in Idle
		}
		else
		{
			if( currentOperation.RequiresTwoFractions() )
			{
				lastOperationMessage = FR.OperationMessage.Error_Requires2Fractions;
			}
			else
			{
				lastOperationMessage = FR.OperationMessage.None;
			}

			ChangeState( MathState.Target1Selected );
		}

		return lastOperationMessage;
	}

	public FR.OperationMessage OnTarget2Selected(Fraction fr)
	{
		lastOperationMessage = FR.OperationMessage.None;

		if( operationInfo == null || operationInfo.StartFraction == null )
		{
			Debug.LogError("MathManager:OnTarget2Selected : currentState was invalid to accept 2nd target!");
			lastOperationMessage = FR.OperationMessage.Error_Requires2Fractions;
		}

		if( operationInfo.StartFraction == fr )
		{
			Debug.LogWarning("MathManager:OnTarget2Selected : cannot select the same target twice. Ignoring");
			lastOperationMessage = FR.OperationMessage.Error_IdenticalTargets;
		}

		if( !currentOperation.RequiresTwoFractions() )
		{
			Debug.LogError("MathManager:OnTarget2Selected : RequiresTwoFractions() is false, yet it didn't execute after select 1...");
			lastOperationMessage = FR.OperationMessage.Error_Requires1Fraction;
		}

		if( lastOperationMessage != FR.OperationMessage.None )
			return lastOperationMessage;



		operationInfo.StopFraction = fr;
		if( FRTargetExtensions.TargetFromFraction(fr).HasNumerator() )
			operationInfo.StopNumber = fr.Numerator;
		else
			operationInfo.StopNumber = fr.Denominator;
		
		
		bool ok = currentOperation.AcceptState( operationInfo );

		if( !ok )
		{
			Debug.LogError("MathManager:OnTarget2Selected : operation doesn't accept state ");
			lastOperationMessage = currentOperation.lastMessage;

			// state not accepted -> stay with target1
		}
		else
		{
			lastOperationMessage = FR.OperationMessage.None;
			ChangeState( MathState.Target2Selected );
		}

		return lastOperationMessage;
	}

	public void ProcessCurrentOperation()
	{

		lastOperationMessage = FR.OperationMessage.None;

		//Debug.LogError ("ProcessCurrentOperation " + currentOperation.type);

		this.gameObject.StartLugusRoutine( ProcessCurrentOperationRoutine() );
	}

	protected IEnumerator ProcessCurrentOperationRoutine()
	{
		GameManager.use.ChangeState( FR.GameState.ProcessingOperation );


		ChangeState( MathState.Visualizing );

		OperationState outcome = currentOperation.Process( operationInfo );
		
		IOperationVisualizer visualizer = GetVisualizer( operationInfo.Type );
		
		if( visualizer == null ) 
		{
			Debug.LogError("MathInputManager:ProcessCurrentOperation : no visualizer found for " + operationInfo.Type);
		}
		else
		{
			yield return gameObject.StartLugusRoutine( visualizer.Visualize(operationInfo, outcome) ).Coroutine;
		}
		
		if( onOperationCompleted != null )
			onOperationCompleted( currentOperation );

		
		GameManager.use.ChangeState( FR.GameState.PartEndSequence, 0.75f ); // extra delay to make sure the visualizer is 100% complete (all tweeners etc.)

		Reset();

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
