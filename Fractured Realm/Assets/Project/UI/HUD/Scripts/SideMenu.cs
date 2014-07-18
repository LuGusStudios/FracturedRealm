using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SideMenu : LugusSingletonExisting<SideMenu> 
{
	public Transform MenuButton = null;
	public Transform HelpButton = null;
	public Transform SoundButton = null;
	public Transform RetryButton = null;
	public Transform SideMenuButton = null;

	public Collider2D ScreenCollider = null;
	
	public Sprite soundOnTexture = null;
	public Sprite soundOffTexture = null;

	protected Vector3 slideOutPosition;
	protected Vector3 slideInPosition;
	protected bool activated = false;

	public void SetupLocal()
	{
		slideOutPosition = this.transform.localPosition.x(6.2f);
		slideInPosition = this.transform.localPosition.x(0.0f);

		if( MenuButton == null )
		{
			MenuButton = transform.FindChild("MenuButton");
		}
		if( MenuButton == null )
		{
			Debug.LogError("SideMenu:SetupLocal : MenuButton not found!");
		}
		
		if( HelpButton == null )
		{
			HelpButton = transform.FindChild("HelpButton");
		}
		if( HelpButton == null )
		{
			Debug.LogError("SideMenu:SetupLocal : HelpButton not found!");
		}
		
		if( SoundButton == null )
		{
			SoundButton = transform.FindChild("SoundButton");
		}
		if( SoundButton == null )
		{
			Debug.LogError("SideMenu:SetupLocal : SoundButton not found!");
		}
		
		if( RetryButton == null )
		{
			RetryButton = transform.FindChild("RetryButton");
		}
		if( RetryButton == null )
		{
			Debug.LogError("SideMenu:SetupLocal : RetryButton not found!");
		}
		
		if( SideMenuButton == null )
		{
			SideMenuButton = transform.FindChild("SideMenuButton");
		}
		if( SideMenuButton == null )
		{
			Debug.LogError("SideMenu:SetupLocal : SideMenuButton not found!");
		}
		
		if( ScreenCollider == null )
		{
			Transform ScreenColliderTransform = transform.FindChild("ScreenCollider");
			
			if( ScreenColliderTransform == null )
			{
				Debug.LogError("SideMenu:SetupLocal : ScreenColliderTransform not found!");
			}
			else
			{
				ScreenCollider = ScreenColliderTransform.GetComponent<BoxCollider2D>();
			}
		}
		if( ScreenCollider == null )
		{
			Debug.LogError("SideMenu:SetupLocal : ScreenCollider not found!");
		}
		
		if( soundOnTexture == null )
		{
			Debug.LogError("SideMenu:SetupLocal : soundOnTexture not set!");
		}
		if( soundOffTexture == null )
		{
			Debug.LogError("SideMenu:SetupLocal : soundOffTexture not set!");
		}

	}

	public void Activate(bool animate = true)
	{
		ScreenCollider.enabled = true;

		gameObject.StopTweens();

		if( animate )
		{
			gameObject.MoveTo( slideOutPosition ).IsLocal(true).Time (0.5f).Execute();
		}
		else
		{
			gameObject.transform.position = slideOutPosition;
		}

		activated = true;
	}

	public void DeActivate(bool animate = true)
	{
		ScreenCollider.enabled = false;

		gameObject.StopTweens();
		
		if( animate )
		{
			gameObject.MoveTo( slideInPosition ).IsLocal(true).Time (0.5f).Execute();
		}
		else
		{
			gameObject.transform.position = slideInPosition;
		}
		
		activated = false;
	}
	
	public void SetupGlobal()
	{
		DeActivate(false);
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
		Transform hit = LugusInput.use.RayCastFromMouseUp2D();
		if( hit == null )
		{
			return;
		}

		if( hit == SideMenuButton )
		{
			if( activated )
				DeActivate();
			else
				Activate();
		}

		if( !activated )
			return;

		if( hit == RetryButton )
		{
			if( GameManager.use.CanRestartCurrentExercisePart() )
			{
				GameManager.use.RestartCurrentExercisePart();
				DeActivate();
			}
			else
			{
				// TODO: add user feedback (or maybe make the button greyed-out if not available?
				Debug.LogError("SideMenu:Update : RetryButton clicked : cannot while in this game state " + GameManager.use.currentState );
				MenuManager.ButtonPressEffect( RetryButton, 1.0f );
			}
		}

		if( hit == HelpButton )
		{ 
			// TODO: show help overlay
			Debug.Log ("SideMenu:Update : help button clicked");
			MenuManager.ButtonPressEffect( HelpButton, 1.0f );
		}

		if( hit == MenuButton )
		{
			// TODO: back to menu scene

			CrossSceneInfo.use.nextScene = CrossSceneInfo.MainMenuSceneName;

			Debug.Log ("SideMenu:Update : menu button clicked");
			MenuManager.ButtonPressEffect( MenuButton, 1.0f );

			CrossSceneInfo.use.LoadNextScene(1.0f);
		}

		if( hit == SoundButton )
		{
			// TODO: properly toggle sound
			if( SoundButton.GetComponent<SpriteRenderer>().sprite == soundOnTexture )
				SoundButton.GetComponent<SpriteRenderer>().sprite = soundOffTexture;
			else
				SoundButton.GetComponent<SpriteRenderer>().sprite = soundOnTexture;
		}
	}


}
