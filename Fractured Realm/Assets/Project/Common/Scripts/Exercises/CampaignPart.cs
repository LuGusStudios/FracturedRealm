using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CampaignPart
{
	public Campaign parent = null;

	public string fileName = "";
	public int nr = -1;

	public string name = "";
	public string description = "";

	public FR.WorldType worldType = FR.WorldType.NONE;

	public List<string> exerciseGroups = new List<string>();

	public int currentExerciseGroupIndex = -1;
	public string currentExerciseGroup
	{
		get
		{
			if( currentExerciseGroupIndex < 0 || currentExerciseGroupIndex >= exerciseGroups.Count )
				return null;

			return exerciseGroups[ currentExerciseGroupIndex ];
		}
	}

	public int NextLevelIndex()
	{
		if( currentExerciseGroupIndex < (exerciseGroups.Count - 1) ) // if == count - 1, we're at the last available index already
		{
			return currentExerciseGroupIndex + 1;
		}
		else
		{
			return -1;
		}
	}


	protected string KeyGroupCompleted(int index)
	{
		return "Campaign_" + parent.nr + "_" + this.nr + "_" + index + ".completed";
	}

	protected string KeyGroupScore(int index)
	{
		return "Campaign_" + parent.nr + "_" + this.nr + "_" + index + ".score";
	}

	public int GetGroupScore(int index)
	{		
		if( index < 0 || index >= this.exerciseGroups.Count )
		{
			Debug.LogError("CampaignPart:GetGroupScore : group index was not found in this campaign part " + index);
			return 0;
		}

		string key = KeyGroupScore(index);
		return LugusConfig.use.User.GetInt( key, 0 );
	}

	public bool IsGroupUnlocked(int index)
	{
		// a group is unlocked if the previous group in this CampaignPart has been completed
		if( index == 0 )
			return true;

		if( index >= exerciseGroups.Count )
		{
			Debug.LogError("CampaignPart:IsGroupUnlocked : unknown group for index " + index);
			return false;
		}

		index = index - 1; // previous group

		string key = KeyGroupCompleted(index);

		//if( LugusDebug.allowDebugging ) // when debugging, all groups are unlocked
		//	return true;
		//else
			return LugusConfig.use.User.GetBool( key, false );
	}

	public void CompleteGroup(int index, int score)
	{
		if( index < 0 || index >= this.exerciseGroups.Count )
		{
			Debug.LogError("CampaignPart:CompleteGroup : group index was not found in this campaign part " + index);
			return;
		}

		string key = KeyGroupCompleted(index);
		LugusConfig.use.User.SetBool( key, true, true );

		key = KeyGroupScore(index);
		LugusConfig.use.User.SetInt( key, score, true );


		LugusConfig.use.User.Store(); 
	}

	public int IndexForGroup(string groupName)
	{
		return exerciseGroups.IndexOf( groupName );
	}

	public void CompleteGroup(string groupName, int score)
	{
		CompleteGroup( IndexForGroup(groupName), score );
	}

	public bool IsGroupCompleted(string groupName)
	{
		return IsGroupCompleted( IndexForGroup(groupName) );
	}

	public bool IsGroupCompleted(int index)
	{
		string key = KeyGroupCompleted(index);
		return LugusConfig.use.User.GetBool( key, false );
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
