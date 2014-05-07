using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiritCameraEffectTester : MonoBehaviour 
{
	public int state = -1;

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

	public void SwitchState(int newState)
	{
		int oldState = state;
		state = newState;

		if( oldState == newState )
		{
			Debug.LogWarning ("SpiritCameraEffectTester:SwitchState EQUAL STATES not accepted : " + newState);
			return;
		}

		if( oldState == 0 )
		{
			DisablePostProcess();
		}
		else if( oldState == 1 )
		{
			DisableQuad();
		}
		else if( oldState == 2 )
		{
			DisableInvididualShaders();
		}
		
		
		if( newState == 0 )
		{
			EnablePostProcess();
		}
		else if( newState == 1 )
		{
			EnableQuad();
		}
		else if( newState == 2 )
		{
			EnableInvididualShaders();
		}

		Debug.Log ("SpiritCameraEffectTester:SwitchState from " + oldState + " to " + newState);
	}

	protected void EnablePostProcess()
	{
		GrayscaleEffect grayscale = GetComponent<GrayscaleEffect>();
		grayscale.enabled = true;
	}

	protected void DisablePostProcess()
	{
		GrayscaleEffect grayscale = GetComponent<GrayscaleEffect>();
		grayscale.enabled = false;
	}

	protected void EnableQuad()
	{
		Transform quad = transform.FindChild("Quad");
		quad.gameObject.SetActive(true);

		if( FRCamera.use.mode == FR.Target.DENOMINATOR )
		{
			quad.localScale = new Vector3( 2.684894f, 2.032446f, 1.0f );
		}
		else
		{
			quad.localScale = new Vector3( 2.684894f, 1.0f, 1.0f );
		}
	}

	protected void DisableQuad()
	{
		Transform quad = transform.FindChild("Quad");
		quad.gameObject.SetActive(false);
	}

	protected void EnableInvididualShaders()
	{
		World world = GameObject.FindObjectOfType<World>();

		Highlighter.use.ToggleGrayScale( true, world.denominator.transform );
	}

	protected void DisableInvididualShaders()
	{
		World world = GameObject.FindObjectOfType<World>();
		
		Highlighter.use.ToggleGrayScale( false, world.denominator.transform );
	}

	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		GUILayout.BeginArea( new Rect(0, Screen.height - 150, 200, 150 ), GUI.skin.box);
		GUILayout.BeginVertical();

		string current = "NONE";
		if( state == 0 )
			current = "Post process";
		else if( state == 1 )
			current = "Quad blend";
		else if( state == 2 )
			current = "Individual shaders";

		GUILayout.Label("Current: " + current);

		if( GUILayout.Button("Post process\n") )
		{
			SwitchState(0);
		}
		
		if( GUILayout.Button("Quad blend\n") )
		{
			SwitchState(1);
		}
		
		if( GUILayout.Button("Individual shaders\n") )
		{
			SwitchState(2);
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
