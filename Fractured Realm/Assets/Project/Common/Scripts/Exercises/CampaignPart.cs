using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CampaignPart
{
	public Campaign parent = null;

	public string fileName = "";
	public int nr = -1;

	public string name = "";
	public string description = "";

	public FR.WorldType worldType = FR.WorldType.NONE;

	public List<string> exerciseGroups = new List<string>();

	public bool IsGroupUnlocked(int index)
	{
		string key = "Campaign_" + parent.nr + "_" + this.nr + "_" + index + ".unlocked";

		if( LugusDebug.allowDebugging )
			return true;
		else
			return LugusConfig.use.User.GetBool( key, false );
	}

	public void UnlockGroup(int index)
	{
		string key = "Campaign_" + parent.nr + "_" + this.nr + "_" + index + ".unlocked";
		LugusConfig.use.User.SetBool( key, true, true );
	}


	public bool IsUnlocked()
	{
		return LugusConfig.use.User.GetBool("fileName.unlocked", false);
	}

	public void Unlock()
	{
		LugusConfig.use.User.SetBool("fileName.unlocked", true, true);
	}
	
	public void Lock()
	{
		LugusConfig.use.User.SetBool("fileName.unlocked", false, true);
	}
}
