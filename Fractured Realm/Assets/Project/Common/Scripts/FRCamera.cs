using UnityEngine;
using System.Collections;

public class FRCamera : LugusSingletonExisting<FRCamera> 
{
	public FR.Target mode = FR.Target.NONE;

	public void Awake()
	{
		// this makes the class play nicely with editor scripts and different viewpoints / setups in editor mode vs play mode
		mode = FR.Target.NONE;
	}

	public void SetMode( FR.Target newMode )
	{
		//Debug.LogError("FRCamera setting mode " + newMode + " from " + this.mode + ". " + LugusCamera.numerator );

		if( newMode == mode )
		{
			Debug.LogWarning("FRCamera:SetMode : mode not changed! same as before! Might give inconsistent results...");
			return;
		}

		mode = newMode;

		if( mode == FR.Target.BOTH )
		{
			LugusCamera.numerator.gameObject.SetActive(true);
			LugusCamera.numerator.fieldOfView = 30;
			LugusCamera.numerator.rect = new Rect(0, 0.5f, 1, 1);


			
			LugusCamera.denominator.gameObject.SetActive(true);
			LugusCamera.denominator.fieldOfView = 30;
			LugusCamera.denominator.rect = new Rect(0, 0, 1, 0.5f);

            SetCharactersPosition(1f);
		}

		else if( mode == FR.Target.NUMERATOR)
		{
			LugusCamera.numerator.gameObject.SetActive(true);

			LugusCamera.numerator.rect = new Rect(0, 0, 1, 1);
			
			// looks fine, but if you look straight, it will actually drop some details that were visible on the sides 
			// so the visible are actually becomes a little smaller this way
			//LugusCamera.numerator.transform.position = new Vector3(0,0, -20.0f); 

			// gives almost the exact same view
			// only thing that probably needs to be done here is y-scaling of the background gradient
			// since that is a big difference depening on the FOV
			// note: i think for the desert example it actually looks nicer in Both if the background is cropped
			LugusCamera.numerator.fieldOfView = 57;


			LugusCamera.denominator.gameObject.SetActive(false);

            SetCharactersPosition(-1f);
		}

		else if( mode == FR.Target.DENOMINATOR )
		{
			LugusCamera.denominator.gameObject.SetActive(true);
			
			LugusCamera.denominator.rect = new Rect(0, 0, 1, 1);
			LugusCamera.denominator.fieldOfView = 57;


			LugusCamera.numerator.gameObject.SetActive(false);

            SetCharactersPosition(-1f);
		}
	}

	public void MoveToDefaultPositions()
	{
		LugusCamera.numerator.transform.position = new Vector3(0,0, -10.0f);
		LugusCamera.denominator.transform.position = new Vector3(0,-200, -10.0f);
	}

    void SetCharactersPosition(float zPosition)
    {
		//Debug.LogError("Setting characters position to " + zPosition);

		NumberRenderer[] numbers = GameObject.FindObjectsOfType<NumberRenderer>();
		
		if (numbers.Length < 1)
		{
			Debug.LogWarning("FRCamera: Couldn't find any NumberRenderers in the scene");
			return;
		}
		
		foreach (NumberRenderer number in numbers)
		{
			number.transform.position = number.transform.position.z( number.SpawnPosition.z + zPosition );
		}

		/*
        Character[] characters = GameObject.FindObjectsOfType<Character>();

        if (characters.Length < 1)
        {
            Debug.LogWarning("FRCamera: Couldn't find any characters in the scene");
            return;
        }

        foreach (Character character in characters)
        {
            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zPosition);
        }
        */
    }

	public void DrawInteractionGroupSelectionGUI()
	{
		World world = GameObject.FindObjectOfType<World>();
 
		int groupCount = 0;
		if( world.numerator != null )
		{
			groupCount = world.numerator.InteractionGroups.Length;
		}
		else if( world.denominator != null )
		{
			groupCount = world.denominator.InteractionGroups.Length;
		}
		else
		{
			// no worldparts found, nothing to draw
			return;
		}

		GUILayout.BeginVertical();

		GUILayout.Space(10);

		for( int i = 0; i < groupCount; ++i )
		{			

			GUILayout.BeginHorizontal();

			GUILayout.Label("Group " + (i+1) + " ", GUILayout.MaxWidth(100) );

			if( GUILayout.Button("\nEntry\n") )
			{
				if( world.numerator != null )
					MoveTo( LugusCamera.numerator,   world.numerator.InteractionGroups[i].PortalEntryCamera , false );

				if( world.denominator != null )
					MoveTo( LugusCamera.denominator, world.denominator.InteractionGroups[i].PortalEntryCamera, false );

			}
			if( GUILayout.Button("\nCENTER\n") )
			{
				
				if( world.numerator != null )
					MoveTo( LugusCamera.numerator,   world.numerator.InteractionGroups[i].Camera.position, false );
				
				if( world.denominator != null )
					MoveTo( LugusCamera.denominator, world.denominator.InteractionGroups[i].Camera.position, false );
			}
			if( GUILayout.Button("\nExit\n") )
			{
				
				if( world.numerator != null )
					MoveTo( LugusCamera.numerator,   world.numerator.InteractionGroups[i].PortalExitCamera, false );
				
				if( world.denominator != null )
					MoveTo( LugusCamera.denominator, world.denominator.InteractionGroups[i].PortalExitCamera, false );
			}

			GUILayout.EndHorizontal();
		}

		if( Application.isPlaying )
		{
			if( GUILayout.Button("\nCycle through all\n") )
			{
				StopAllCoroutines();
				gameObject.StartLugusRoutine( InteractionGroupCycleRoutine(groupCount) );
			}
		}

		GUILayout.EndVertical();
	}

	protected IEnumerator InteractionGroupCycleRoutine(int groupCount)
	{
		World world = GameObject.FindObjectOfType<World>();

		float waitTime = 2.0f;

		for( int i = 0; i < groupCount; ++i )
		{
			if( world.numerator != null )
			{
				MoveTo( LugusCamera.numerator, world.numerator.InteractionGroups[i].PortalEntryCamera, true );
			}
			else if( world.denominator != null )
			{
				MoveTo( LugusCamera.denominator, world.denominator.InteractionGroups[i].PortalEntryCamera, true );
			}

			yield return new WaitForSeconds( waitTime );
			
			if( world.numerator != null )
			{
				MoveTo( LugusCamera.numerator, world.numerator.InteractionGroups[i].Camera.position, true );
			}
			else if( world.denominator != null )
			{
				MoveTo( LugusCamera.denominator, world.denominator.InteractionGroups[i].Camera.position, true );
			}
			
			yield return new WaitForSeconds( waitTime );

			if( world.numerator != null )
			{
				MoveTo( LugusCamera.numerator, world.numerator.InteractionGroups[i].PortalExitCamera, true );
			}
			else if( world.denominator != null )
			{
				MoveTo( LugusCamera.denominator, world.denominator.InteractionGroups[i].PortalExitCamera, true );
			}
			
			yield return new WaitForSeconds( waitTime );
		}
	}

	public void MoveTo( Camera camera, Vector3 target, bool animate )
	{
#if UNITY_EDITOR
		if( !Application.isPlaying )
			animate = false;
#endif

		float time = 1.0f;
		if( animate )
			camera.gameObject.MoveTo( target ).Time ( time ).Execute();
		else
			camera.transform.position = target;
	}

	public void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		GUILayout.BeginArea( new Rect(Screen.width - 200, 0, 200, 300), GUI.skin.box );

		DrawInteractionGroupSelectionGUI();

		GUILayout.EndArea();
	}
}
