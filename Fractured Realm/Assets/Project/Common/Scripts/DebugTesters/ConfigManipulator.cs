using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigManipulator : MonoBehaviour 
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
	
	protected void Update () 
	{
	
	}

	Vector2 scrollPos = Vector2.zero;

	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		GUILayout.BeginArea( new Rect(0, 100, Screen.width / 2.0f, Screen.height - 100) );
		
		if( GUILayout.Button("\nClear User config\n") )
		{
			LugusConfig.use.User.Clear();
			LugusConfig.use.User.Store();
		}

		scrollPos = GUILayout.BeginScrollView( scrollPos, GUI.skin.box );

		LugusConfigProfileDefault config = (LugusConfigProfileDefault) LugusConfig.use.User;

		foreach( KeyValuePair<string, string> pair in config.Data )
		{
			GUILayout.Label("" + pair.Key + " : " + pair.Value );
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
