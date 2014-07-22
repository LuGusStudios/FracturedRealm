using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugStoryScene : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		CampaignManager.use.currentCampaign = CrossSceneInfo.use.currentCampaign;
		routine = gameObject.StartLugusRoutine( AutoProgressRoutine() );
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	ILugusCoroutineHandle routine = null;
	protected void Update () 
	{
		if( LugusInput.use.down || LugusInput.use.KeyDown(KeyCode.Return) )
		{
			if( routine != null )
				routine.StopRoutine();

			Continue();
		}
	}

	protected IEnumerator AutoProgressRoutine()
	{
		yield return new WaitForSeconds(5.0f);

		Continue ();
	}

	public void Continue()
	{
		
		CrossSceneInfo.use.nextScene = CrossSceneInfo.GameplaySceneName;
		CrossSceneInfo.use.LoadNextScene();
	}
}
