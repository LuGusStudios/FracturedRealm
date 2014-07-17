using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CampaignManager : LugusSingletonRuntime<CampaignManager> 
{
	public Campaign currentCampaign = null;

	public void LoadCampaign(int nr)
	{
		if( currentCampaign != null && currentCampaign.nr == nr ) // already loaded this campaign as the current one
			return;

		currentCampaign = CampaignLoader.LoadCampaign(nr);

		if( currentCampaign == null )
		{
			Debug.LogError("CampaignManager:LoadCampaign : campaign " + nr + " not found!");
		}
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
