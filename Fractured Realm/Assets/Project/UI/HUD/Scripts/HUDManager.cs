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
}
