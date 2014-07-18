using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossSceneInfo : LugusSingletonRuntime<CrossSceneInfo> 
{
	public static string GameplaySceneName = "GameplaySetup";
	public static string MainMenuSceneName = "LevelSelect";

	public string previousScene = "";
	public string nextScene = "";

	public int completedExerciseGroupIndex = -2;

	public Campaign currentCampaign = null;

	//public string currentExerciseGroup = "";

	public void Reset()
	{

		previousScene = "";
		nextScene = "";

		completedExerciseGroupIndex = -2;
		currentCampaign = null;
		//currentExerciseGroup = "";
	}

	public void SetupLocal()
	{ 
		// assign variables that have to do with this class only
		GameObject.DontDestroyOnLoad( this ); 
	}

	public void LoadNextScene(float delay = 0.0f)
	{
		if( !string.IsNullOrEmpty(nextScene) )
		{
			previousScene = Application.loadedLevelName;
			currentCampaign = CampaignManager.use.currentCampaign;

			LugusConfig.use.SaveProfiles();

			LugusCoroutines.use.StartRoutine( LoadNextSceneRoutine(delay) );
		}
	}

	protected IEnumerator LoadNextSceneRoutine(float delay)
	{
		if( delay > 0.0f )
			yield return new WaitForSeconds(delay);

		
		
		Application.LoadLevel( nextScene );
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
}
