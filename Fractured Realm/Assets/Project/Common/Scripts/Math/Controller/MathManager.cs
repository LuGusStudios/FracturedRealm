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
	public bool hasProcessedMainOperation = false; // if we've already done add, subtract, multiply or divide in this run. This is NOT changed back to false inside this class, should be set from the outside (GameManager)

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

		if( newState == MathState.Idle )
		{
			currentOperation = null;
			lastOperationMessage = FR.OperationMessage.None;
			operationInfo = null;
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

		int iterations = 0;
		do
		{
			animationData = FRAnimations.use.GetRandomStarterAnimation( type );
			++iterations;
		}
		while( (iterations < 50) && (animationData.visualizer.GetImplementationStatus() == FR.VisualizerImplementationStatus.NONE) );

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
		hasProcessedMainOperation = false;
		currentOperation = null;
		lastOperationMessage = FR.OperationMessage.None;
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

		UpdateDenominatorLinks();

		if( operationInfo.Type == FR.OperationType.ADD ||
		   operationInfo.Type == FR.OperationType.SUBTRACT ||
		   operationInfo.Type == FR.OperationType.MULTIPLY ||
		   operationInfo.Type == FR.OperationType.DIVIDE )
		{
			hasProcessedMainOperation = true;
		}

		
		if( onOperationCompleted != null )
			onOperationCompleted( currentOperation );

		if( ContinueCalculations() )
		{
			//MathInputManager.use.ChangeState( MathInputManager.InputState.IDLE );
			ChangeState( MathState.Idle );
			GameManager.use.ChangeState( FR.GameState.WaitingForInput );
		}
		else
		{
			GameManager.use.ChangeState( FR.GameState.PartEndSequence, 0.75f ); // extra delay to make sure the visualizer is 100% complete (all tweeners etc.)
			Reset();
		}

		yield break;
	}

	protected void UpdateDenominatorLinks()
	{
		// denominators have to be equal when using spirit world for Add / Subtract
		// we also have visual indicators for this.
		// Thus, when a denominator has changed value (after an operation), we need to check if the visual indicator needs to change as well

		List<NumberRenderer> denominators = new List<NumberRenderer>();
		NumberRenderer[] allNumbers = GameObject.FindObjectsOfType<NumberRenderer>();

		foreach( NumberRenderer number in allNumbers )
		{
			if( number.Number.IsDenominator )
				denominators.Add( number );
		}

		// for now, assume just 2 fractions
		// but if more it also works: if AT LEAST 1 fraction has the same value, a brother has been found
		foreach( NumberRenderer number in denominators )
		{
			if( !number.WaitingForEqualBrother ) 
				continue;

			foreach( NumberRenderer brother in denominators )
			{
				if( brother == number )
					continue;

				if( brother.Number.Value == number.Number.Value )
				{
					Debug.LogWarning("FOUND BROTHER! " + number.transform.Path() + " == " + brother.transform.Path() );
					number.WaitingForEqualBrother = false;

					// if the denominator is waiting, the corresponding numerator was also waiting, so also let him know it has changed
					if( number.FractionRenderer.Fraction.HasNumerator() )
					{
						number.FractionRenderer.Numerator.WaitingForEqualBrother = false;
					}
				}
			}
		}
	}

	protected bool ContinueCalculations()
	{
		// TODO: FIXME: just for debugging!
		if( Application.loadedLevelName == "AnimationTest" )
		{
			return false;
		}

		ExercisePart part = GameManager.use.currentExercisePart;

		Fraction outcome = part.FinalOutcome;

		// 2 possible options after operation performed:
		// 1. correct solution 
		// 	   1.1 : main operation done : correct finish, proceed
		//	   1.2 : no main operation done yet : cannot finish, need to wait for operation
		// 			(this can happen when only doing denominator stuff: ex. 6 + 3 = 3, but first need to simplify the 3. This already gives "correct" answer, but still need to perform the +
		// 2. wrong solution : 	
		//     2.1 : no main operation done yet (just simplify or double done) : continue with input
		//	   2.2 : main operation done, simplify and double available : continue with input
		// 	   2.3 : main operation done, no simplify / double available : show wrong by running to portal (control back to GameManager)

		// TODO: note: when working with more than 2 fractions, this logic has to be updated!

		// 1
		if( operationInfo.StartFraction.Numerator.Value == outcome.Numerator.Value 
		    &&
		    operationInfo.StartFraction.Denominator.Value == outcome.Denominator.Value )
		{
			if( hasProcessedMainOperation ) // 1.1
			{
				Debug.Log ("MathManager:ContinueCalculations : outcome was correct!");
				GameManager.use.outcomeWasCorrect = true;
				return false;
			}
			else // 1.2
			{
				Debug.Log ("MathManager:ContinueCalculations : outcome was correct already, but no main operation done yet. Should only happen in denominator-only levels!!!");
				return true;
			}
		}
		else // 2
		{
			if( !hasProcessedMainOperation ) // 2.1
			{
				Debug.Log ("MathManager:ContinueCalculations : outcome not correct yet, but also no main operation done yet. continue");
				return true;
			}
			else 
			{
				// TODO: probably do calculations (limited depth) with simplify + double to see if we can ever reach the intended result...
				List<OperationIcon> operationIcons = new List<OperationIcon>();
				operationIcons.AddRange ( GameObject.FindObjectsOfType<OperationIcon>() );

				bool hasOptions = false;
				foreach( OperationIcon icon in operationIcons )
				{
					if( icon.type == FR.OperationType.SIMPLIFY ||
					    icon.type == FR.OperationType.DOUBLE )
					{
						if( icon.OperationAmount != 0 ) // amount < 0 is infinite amount
						{
							hasOptions = true;
							break;
						}
					}
				}

				if( hasOptions ) // 2.2
				{
					Debug.Log ("MathManager:ContinueCalculations : outcome not correct yet, but has simplify/double leftover. continue");
					return true;
				}
				else // 2.3
				{
					Debug.Log ("MathManager:ContinueCalculations : outcome not correct but no more options... force replay");
					GameManager.use.outcomeWasCorrect = false;
					return false;
				}
			}
		}
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
