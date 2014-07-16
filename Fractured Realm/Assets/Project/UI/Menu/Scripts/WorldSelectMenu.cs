using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldSelectMenu : IMenu 
{
	public Transform BackButton = null;

	public override bool Activate(bool force = false)
	{
		if( activated && !force )
			return false;
		
		this.transform.position = Vector3.zero;

		activated = true;
		
		return true;
	}
	
	public override bool Deactivate(bool force = false)
	{
		if( !activated && !force )
			return false;


		this.transform.position = LugusUtil.OFFSCREEN;


		activated = false;
		
		return true;
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		BackButton = transform.FindChild("BackButton");
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
		Transform hit = LugusInput.use.RayCastFromMouseUp2D();
		if( hit == null )
			return;
		
		if( hit == BackButton )
		{
			MenuManager.use.ChangeState( MenuManager.MenuState.Start );
		}
	}
}
