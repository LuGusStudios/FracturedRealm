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

		CampaignManager.use.LoadCampaign(1);

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
			Debug.LogError("hit back button level select");

			MenuManager.use.ChangeStateDelayed(MenuManager.MenuState.Start, 0.35f);
			MenuManager.ButtonPressEffect( BackButton, 0.35f );
		}
	}

	public void OnGUI()
	{
		if( !activated )
			return;

		if( CampaignManager.use.currentCampaign == null )
			return;

		GUILayout.BeginArea( new Rect(0,200, Screen.width, Screen.height) );
		GUILayout.BeginHorizontal();

		foreach( CampaignPart part in CampaignManager.use.currentCampaign.parts )
		{
			if( GUILayout.Button( "\n\n" + part.nr + "\n" + part.name.LugusLocalized() + "\n" + part.description.LugusLocalized() + "\n\n" ) )
			{
				CampaignManager.use.currentCampaign.currentCampaignPart = part;

				MenuManager.use.ChangeState( MenuManager.MenuState.LevelSelect );
			}
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
