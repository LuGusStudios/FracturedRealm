using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossSceneInfo : LugusSingletonRuntime<CrossSceneInfo> 
{
	public string previousScene = "";
	public string nextScene = "";

	public string completedExerciseGroup = "";

	public string currentExerciseGroup = "";

	public void Reset()
	{
		previousScene = "";
		nextScene = "";

		completedExerciseGroup = "";
		currentExerciseGroup = "";
	}

	public void SetupLocal()
	{ 
		// assign variables that have to do with this class only
		GameObject.DontDestroyOnLoad( this ); 
	}

	public void LoadNextScene()
	{
		if( !string.IsNullOrEmpty(nextScene) )
		{
			previousScene = Application.loadedLevelName;

			Application.LoadLevel( nextScene );
		}
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
