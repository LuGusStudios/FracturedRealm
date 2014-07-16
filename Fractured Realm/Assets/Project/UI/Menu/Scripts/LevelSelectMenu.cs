using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectMenu : IMenu 
{
	public override bool Activate(bool force = false)
	{
		if( activated && !force )
			return false;
		
		activated = true;
		
		return true;
	}
	
	public override bool Deactivate(bool force = false)
	{
		if( !activated && !force )
			return false;
		
		activated = false;
		
		return true;
	}

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
	
	protected void Update () 
	{
	
	}
}
