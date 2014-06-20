using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDManager : LugusSingletonExisting<HUDManager> 
{
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

	public void SetMode( FR.Target mode )
	{
		FRCamera.use.SetMode( mode );
		MathInputManager.use.SetMode( mode );
	}

	public void UpdateOperationIcons(int amount)
	{
		Dictionary<FR.OperationType, int> counts = new Dictionary<FR.OperationType, int>();

		List<OperationIcon> operationIcons = new List<OperationIcon>();
		operationIcons.AddRange ( GameObject.FindObjectsOfType<OperationIcon>() );
		
		foreach( OperationIcon icon in operationIcons )
		{
			if( icon.type == FR.OperationType.SIMPLIFY || icon.type == FR.OperationType.DOUBLE )
				counts[icon.type] = (amount == 0 ) ? 0 : -1;
			else if( amount == 666 )
				counts[icon.type] = Random.Range(0, 3);
			else
				counts[icon.type] = amount;
		}

		UpdateOperationIcons( counts );
	}

	// pass null to get 3 icons for all operations (Beside simplify and double)
	public void UpdateOperationIcons( Dictionary<FR.OperationType, int> counts = null )
	{
		List<OperationIcon> operationIcons = new List<OperationIcon>();
		operationIcons.AddRange ( GameObject.FindObjectsOfType<OperationIcon>() );

		if( counts == null )
		{
			UpdateOperationIcons(3);
			return;
		}

		foreach( OperationIcon icon in operationIcons )
		{
			icon.Clear();

			if( counts.ContainsKey( icon.type ) )
			{
				icon.OperationAmount = counts[ icon.type ];
			}
		}
	}
	
	public void Update()
	{
		if( LugusInput.use.KeyDownDebug(KeyCode.Alpha1) )
		{
			SetMode( FR.Target.NUMERATOR );
		}
		else if( LugusInput.use.KeyDownDebug(KeyCode.Alpha2) )
		{
			SetMode( FR.Target.DENOMINATOR );
		}
		else if( LugusInput.use.KeyDownDebug(KeyCode.Alpha3) )
		{
			SetMode( FR.Target.BOTH );
		}
	}
	
	public void OnGUI()
	{
		if( !LugusDebug.debug )
			return;
		
		GUILayout.BeginArea( new Rect( Screen.width - 275, Screen.height - 50, 275, 50 ) );
		GUILayout.BeginHorizontal();
		
		if( GUILayout.Button("\nBOTH\n") )
		{
			SetMode( FR.Target.BOTH );
		}
		
		if( GUILayout.Button("\nNUMERATOR\n") )
		{
			SetMode( FR.Target.NUMERATOR );
		}
		
		if( GUILayout.Button("\nDENOMINATOR\n") )
		{
			SetMode( FR.Target.DENOMINATOR );
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
