using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExerciseManager : LugusSingletonRuntime<ExerciseManager> 
{
	public ExerciseGroup currentExerciseGroup = null;

	public List<string> allExerciseGroups = new List<string>();
	public int currentExerciseGroupIndex = -1; 

	public ExerciseGroup NextExerciseGroup()
	{
		if( allExerciseGroups == null || allExerciseGroups.Count == 0 )
		{
			return null;
		}
		
		++currentExerciseGroupIndex;
		
		if( currentExerciseGroupIndex >= allExerciseGroups.Count )
		{
			return null;
		}
		else
		{
			currentExerciseGroup = LoadExerciseGroup( allExerciseGroups[ currentExerciseGroupIndex ] );
			return currentExerciseGroup;
		}
	}

	public void Start()
	{
		LoadDebugData();
	}

	public void LoadDebugData()
	{
		allExerciseGroups = ExerciseLoader.LoadAllExerciseGroupNames();

		if( allExerciseGroups == null || allExerciseGroups.Count == 0 )
		{
			Debug.LogError("ExerciseManager:Load : no AllExerciseGroups found!");
			return;
		}
	}

	/*
	public void LoadExerciseGroup(ExerciseGroup group)
	{
		currentExerciseGroup = group;
	}
	*/

	public ExerciseGroup LoadExerciseGroup(string name)
	{
		ExerciseGroup group = ExerciseLoader.LoadExerciseGroup( name );

		if( group == null )
		{
			Debug.LogError("ExerciseManager:LoadExerciseGroup : group " + name + " not found!");
			return null;
		}
		else
		{
			return group;
		}
	}

	public void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		ShowGroupContentsGUI();
	}

	
	protected Vector2 guiScrollPos1 = Vector2.zero;
	public void ShowGroupContentsGUI()
	{
		GUILayout.BeginArea( new Rect(Screen.width - 300, Screen.height / 2.0f, 300, (Screen.height / 2.0f) - 100), GUI.skin.box );
		
		guiScrollPos1 = GUILayout.BeginScrollView(guiScrollPos1);
		
		if( currentExerciseGroup == null )
		{
			GUILayout.Label("No current exercise group!");
		}
		else
		{
			GUILayout.Label("" + currentExerciseGroup.name + "( " + currentExerciseGroupIndex + "/" + (allExerciseGroups.Count - 1) + " )" );

			foreach( Exercise exercise in currentExerciseGroup.exercises )
			{
				if( exercise != null )
				{
					GUILayout.Label("x " + exercise.difficulty );

					foreach( ExercisePart part in exercise.parts )
					{
						if( part != null )
						{
							GUILayout.Label("   p " + part.ToString() );
						}
						else
						{
							Color c = GUI.color;
							GUI.color = Color.red;
							GUILayout.Label("   p NULL");
							GUI.color = c;
						}
					}
				}
				else
				{
					Color c = GUI.color;
					GUI.color = Color.red;
					GUILayout.Label("x NULL");
					GUI.color = c;
				}
			}
		}
		  
		   
		GUILayout.EndScrollView();
		
		
		if( GUILayout.Button("Next group") )
		{
			NextExerciseGroup();
		}

		GUILayout.EndArea();
	}
		  
}
