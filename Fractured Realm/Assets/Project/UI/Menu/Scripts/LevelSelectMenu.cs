using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectMenu : IMenu 
{
	public Transform BackButton = null;
	public Transform worldBackground = null;

	public List<Transform> levelButtons = new List<Transform>();

	protected ILugusCoroutineHandle animationRoutine = null;
	public override bool Activate(bool force = false)
	{
		if( activated && !force )
			return false;
		
		this.transform.position = Vector3.zero;

		worldBackground = transform.FindChild("" + CampaignManager.use.currentCampaign.currentCampaignPart.worldType + "LevelSelect");
		if( worldBackground == null )
		{
			Debug.LogError("LevelSelectMenu:Activate : no LevelSelect background found for " + CampaignManager.use.currentCampaign.currentCampaignPart.worldType);
			worldBackground = transform.FindChild("" + FR.WorldType.FOREST + "LevelSelect");
		}

		if( worldBackground != null )
		{
			worldBackground.gameObject.SetActive(true);
		}

		int count = 1;
		foreach( Transform button in levelButtons )
		{
			button.collider2D.enabled = false;

			if( count <= CampaignManager.use.currentCampaign.currentCampaignPart.exerciseGroups.Count )
			{
				button.gameObject.SetActive(true);

				if( count != 1 ) // first one is always unlocked at level select
				{
					if( CampaignManager.use.currentCampaign.currentCampaignPart.IsGroupUnlocked( count - 1 ) )
					{
						button.collider2D.enabled = true;

						button.GetComponent<SpriteRenderer>().color = Color.white;
						button.transform.FindChild("Title").GetComponent<TextMesh>().color = button.transform.FindChild("Title").GetComponent<TextMesh>().color.a ( 1.0f );
					}
					else
					{
						button.GetComponent<SpriteRenderer>().color = new Color(121.0f / 255.0f, 189.0f / 255.0f, 255.0f / 255.0f);
						button.transform.FindChild("Title").GetComponent<TextMesh>().color = button.transform.FindChild("Title").GetComponent<TextMesh>().color.a ( 0.25f );
					}
				}
				else
				{
					button.collider2D.enabled = true;
				}
			}
			else
			{
				button.gameObject.SetActive(false);
			}

			++count;
		}
		
		if( animationRoutine != null )
		{
			animationRoutine.StopRoutine();
			animationRoutine = null;
		}
		
		animationRoutine = gameObject.StartLugusRoutine( ActivateRoutine() );

		
		activated = true;
		
		return true;
	}
	
	protected IEnumerator ActivateRoutine()
	{
		// we need to do this as a coroutine, because the ping-pong floating movement of the logo
		// would take the Delay(1.5f) into account in the loop, which is not what we want...

		float delayPerItem = 0.1f;
		float timePerItem = 0.75f;

		int count = 1;
		foreach( Transform button in levelButtons )
		{
			if( !button.gameObject.activeSelf )
				continue;

			button.gameObject.StopTweens();
			button.RevertLocal();

			//button.collider2D.enabled = true;

			if( count < 5 )
			{
				// top 4 buttons slide in from the top
				Vector3 target = button.transform.localPosition;
				button.transform.localPosition = button.transform.localPosition.y( LugusUtil.UIHeight + button.renderer.bounds.extents.y );

				button.gameObject.MoveTo( target ).Delay( (count % 5) * delayPerItem ).Time ( timePerItem ).EaseType( iTween.EaseType.easeOutBounce ).Execute();
			}
			else
			{
				// bottom 4 buttons slide in from the bottom
				Vector3 target = button.transform.localPosition;
				button.transform.localPosition = button.transform.localPosition.y( - button.renderer.bounds.extents.y );

				// eyeball for extra delay per item
				button.gameObject.MoveTo( target ).Delay( (count % 5) * delayPerItem + (3*delayPerItem) ).Time ( timePerItem ).EaseType( iTween.EaseType.easeOutBounce ).Execute();
			}

			++count;
		}

		yield return new WaitForSeconds( (4 * delayPerItem) + timePerItem + 0.3f ); // 0.3f == eyeball

		count = 0;
		foreach( Transform button in levelButtons ) 
		{
			if( !button.gameObject.activeSelf )
				continue;

			button.gameObject.StopTweens();

			if( count < 5 )
			{
				// top 4 buttons slide in from the top
				Vector3 target = button.transform.localPosition.yAdd( 0.2f );

				button.gameObject.MoveTo( target ).IsLocal(true).Delay( Random.Range(0.0f, 0.6f) ).Looptype(iTween.LoopType.pingPong).EaseType(iTween.EaseType.easeInOutSine).Time(Random.Range(1.5f, 2.0f)).Execute();
			}
			else
			{
				// bottom 4 buttons slide in from the bottom
				Vector3 target = button.transform.localPosition.yAdd( -0.2f );
				
				button.gameObject.MoveTo( target ).IsLocal(true).Delay( Random.Range(0.0f, 0.6f) ).Looptype(iTween.LoopType.pingPong).EaseType(iTween.EaseType.easeInOutSine).Time(Random.Range(1.5f, 2.0f)).Execute();
			}

			
			++count;
		}
		
		//Logo.gameObject.MoveTo( Logo.transform.position.y(9.498807f) ).Looptype(iTween.LoopType.pingPong).EaseType(iTween.EaseType.easeInOutSine).Time(2.0f).Execute();
	}

	
	public override bool Deactivate(bool force = false)
	{
		if( !activated && !force )
			return false;
		
		
		this.transform.position = LugusUtil.OFFSCREEN;

		if( worldBackground != null )
		{
			worldBackground.gameObject.SetActive(false);
		}
		
		
		activated = false;
		
		return true;
	}
	
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		BackButton = transform.FindChild("BackButton");

		Transform levelButtonsRoot = transform.FindChild("LevelButtons");
		for( int i = 1; i < 9; ++i )
		{
			Transform button = levelButtonsRoot.FindChild("LevelNumberBoard0" + i);
			button.StoreLocal();

			this.levelButtons.Add( button );
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
	
	protected void Update () 
	{
		if( !activated )
			return;

		Transform hit = LugusInput.use.RayCastFromMouseUp2D();
		if( hit == null )
			return;
		
		if( hit == BackButton )
		{
			MenuManager.use.ChangeStateDelayed(MenuManager.MenuState.WorldSelect, 0.35f);
			MenuManager.ButtonPressEffect( BackButton, 0.35f );
		}
		else
		{
			int index = 0;
			foreach( Transform levelButton in levelButtons )
			{
				if( hit == levelButton )
				{
					MenuManager.ButtonPressEffect( levelButton, 0.75f );

					LugusCoroutines.use.StartRoutine( LoadLevelRoutine(index) );
				}

				++index;
			}
		}

	}

	protected IEnumerator LoadLevelRoutine( int exerciseGroupIndex )
	{
		CampaignManager.use.currentCampaign.currentCampaignPart.currentExerciseGroupIndex = exerciseGroupIndex;

		activated = false; // make sure no further input happens from this point on
		
		ScreenFader.use.FadeOut(1.0f);

		yield return new WaitForSeconds(1.1f);

		CrossSceneInfo.use.Reset();
		CrossSceneInfo.use.nextScene = CrossSceneInfo.GameplaySceneName;
		//CrossSceneInfo.use.currentExerciseGroup = exerciseGroupName;
		
		Debug.Log("LevelSelectMenu : loading level " + CampaignManager.use.currentCampaign.currentCampaignPart.currentExerciseGroup );

		CrossSceneInfo.use.LoadNextScene();

	}

	/*
	public void OnGUI()
	{
		if( !activated )
			return;
		
		if( CampaignManager.use.currentCampaign == null || CampaignManager.use.currentCampaign.currentCampaignPart == null )
			return;
		
		GUILayout.BeginArea( new Rect(0,200, Screen.width, Screen.height) );
		GUILayout.BeginHorizontal();

		int count = 0;
		foreach( string groupname in CampaignManager.use.currentCampaign.currentCampaignPart.exerciseGroups )
		{
			if( GUILayout.Button( "\n\n" + count + "\n" + groupname + "\n\n" ) )
			{
				Debug.LogError ("Start exerciseGroup " + groupname );
			}

			++count;
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	*/
}
