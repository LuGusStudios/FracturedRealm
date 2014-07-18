using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExerciseGroupEndMenu : MonoBehaviour 
{
	public Transform Background = null;
	public Transform MenuButton = null;
	public Transform RetryButton = null;
	public Transform NextButton = null;
	
	public Collider2D ScreenCollider = null;
	public Sprite featherFilledTexture = null;
	public Sprite featherEmptyTexture = null;

	public Transform Title = null;

	public List<Transform> feathers = new List<Transform>();

	protected bool activated = false;

	public void SetupLocal()
	{
		if( Background == null )
		{
			Background = transform.FindChild("Background");
		}
		if( Background == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : Background not found!");
		}

		if( MenuButton == null )
		{
			MenuButton = transform.FindChild("MenuButton");
		}
		if( MenuButton == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : MenuButton not found!");
		}
		
		if( RetryButton == null )
		{
			RetryButton = transform.FindChild("RetryButton");
		}
		if( RetryButton == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : RetryButton not found!");
		}
		
		if( NextButton == null )
		{
			NextButton = transform.FindChild("NextButton");
		}
		if( NextButton == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : NextButton not found!");
		}
		
		if( Title == null )
		{
			Title = transform.FindChild("Title");
		}
		if( Title == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : Title not found!");
		}
		
		if( ScreenCollider == null )
		{
			Transform ScreenColliderTransform = transform.FindChild("ScreenCollider");
			
			if( ScreenColliderTransform == null )
			{
				Debug.LogError("ExerciseGroupEndMenu:SetupLocal : ScreenColliderTransform not found!");
			}
			else
			{
				ScreenCollider = ScreenColliderTransform.GetComponent<BoxCollider2D>();
			}
		}
		if( ScreenCollider == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : ScreenCollider not found!");
		}

		feathers.Add( transform.FindChild("Feather1") );
		feathers.Add( transform.FindChild("Feather2") );
		feathers.Add( transform.FindChild("Feather3") );

		foreach( Transform feather in feathers )
		{
			if( feather == null )
				Debug.LogError("ExerciseGroupEndMenu:SetupLocal : A feather was not found!");
			else if( feather.GetComponent<SpriteRenderer>() == null )
			{
				Debug.LogError("ExerciseGroupEndMenu:SetupLocal : A feather didn't have a sprite renderer component! " + feather.Path() );
			}
			else
			{
				if( featherEmptyTexture == null )
					featherEmptyTexture = feather.GetComponent<SpriteRenderer>().sprite;
			}
		}

		if( featherEmptyTexture == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : featherEmptyTexture not set!");
		}



		if( featherFilledTexture == null )
		{
			Debug.LogError("ExerciseGroupEndMenu:SetupLocal : featherFilledTexture has to be assigend in-editor!");
		}
	}

	public void Activate(bool animate = true)
	{
		gameObject.transform.position = Vector3.zero.z(-2.0f);

		ScreenCollider.enabled = true;

		// TODO: set Title!

		Title.gameObject.StopTweens();
		NextButton.gameObject.StopTweens();
		RetryButton.gameObject.StopTweens();
		MenuButton.gameObject.StopTweens();
		Background.gameObject.StopTweens();

		Background.transform.position = Background.transform.position.y(23.50141f);
		Title.transform.position = Title.transform.position.y( 19.2776f );
		RetryButton.transform.position = RetryButton.transform.position.y( -2.02937f );
		MenuButton.transform.position = MenuButton.transform.position.y( -2.159803f );
		NextButton.transform.position = NextButton.transform.position.y( -2.13968f );

		Background.gameObject.MoveTo( Background.transform.position.y( 7.68f ) ).Time(0.5f).Execute();
		Title.gameObject.MoveTo( Title.transform.position.y(11.94979f) ).Delay(0.6f).Time( 0.75f ).EaseType(iTween.EaseType.easeOutBack).Execute();
		RetryButton.gameObject.MoveTo( RetryButton.transform.position.y(1.90008f) ).Delay (0.6f).Time( 0.75f ).EaseType(iTween.EaseType.easeOutBack).Execute();
		MenuButton.gameObject.MoveTo( MenuButton.transform.position.y(1.769647f) ).Delay (0.9f).Time( 0.75f ).EaseType(iTween.EaseType.easeOutBack).Execute();
		NextButton.gameObject.MoveTo( NextButton.transform.position.y(1.78977f) ).Delay (1.2f).Time( 0.75f ).EaseType(iTween.EaseType.easeOutBack).Execute();


		foreach( Transform feather in feathers )
		{
			feather.localScale = Vector3.zero;
			feather.gameObject.StopTweens();
		}

		int score = CampaignManager.use.currentCampaign.currentCampaignPart.GetGroupScore( CampaignManager.use.currentCampaign.currentCampaignPart.currentExerciseGroupIndex );
		int count = 1;
		foreach( Transform feather in feathers )
		{
			if( count <= score )
			{
				feather.GetComponent<SpriteRenderer>().sprite = featherFilledTexture;
			}
			else
			{
				feather.GetComponent<SpriteRenderer>().sprite = featherEmptyTexture;
			}

			Vector3 targetScale = new Vector3( 0.8392893f, 0.8392893f, 0.8392893f );

			if( count == 2 )
				targetScale = new Vector3( 1.032789f, 1.032789f, 1.032789f ); 

			feather.gameObject.ScaleTo( targetScale ).Delay( 1.0f + ( count * 0.5f ) ).Time( 0.5f ).EaseType( iTween.EaseType.easeOutBack ).Execute();

			++count;
		}

		
		Title.GetComponent<TextMeshWrapper>().SetTextKey( "score.title." + score );


		if( CampaignManager.use.currentCampaign.currentCampaignPart.NextLevelIndex() == -1 )
		{
			// no next level available
			NextButton.gameObject.SetActive(false);
		}
		else
		{
			NextButton.gameObject.SetActive(true);
		}
		
		activated = true;
	}
	
	public void DeActivate(bool animate = true)
	{
		ScreenCollider.enabled = false;
		
		gameObject.transform.position = LugusUtil.OFFSCREEN;

		// TODO: exit animation? is this even needed?
		
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
		if( LugusInput.use.KeyDownDebug(KeyCode.E) )
		{
			if( !activated )
				Activate();
			else
				DeActivate();
		}

		Transform hit = LugusInput.use.RayCastFromMouseUp2D();
		if( hit == null )
		{
			return;
		}
		
		if( !activated )
			return;
		
		if( hit == RetryButton )
		{
			CrossSceneInfo.use.nextScene = CrossSceneInfo.GameplaySceneName; // should be set if coming from main menu, but for debug it's easier to set it manually here as well

			// just reload level basically
			MenuManager.ButtonPressEffect( RetryButton, 1.0f ); 
			ScreenFader.use.FadeOut(1.0f);

			CrossSceneInfo.use.LoadNextScene(1.0f); 
		}

		if( hit == MenuButton )
		{
			CrossSceneInfo.use.nextScene = CrossSceneInfo.MainMenuSceneName;
			
			Debug.Log ("ExerciseGroupEndMenu:Update : menu button clicked");
			MenuManager.ButtonPressEffect( MenuButton, 1.0f );
			ScreenFader.use.FadeOut(1.0f);
			
			CrossSceneInfo.use.LoadNextScene(1.0f);
		}
		
		if( hit == NextButton )
		{
			// if no next level, the button is inactive, and user cannot click it
			CampaignManager.use.currentCampaign.currentCampaignPart.currentExerciseGroupIndex = CampaignManager.use.currentCampaign.currentCampaignPart.NextLevelIndex();

			MenuManager.ButtonPressEffect( NextButton, 1.0f ); 
			ScreenFader.use.FadeOut(1.0f);

			CrossSceneInfo.use.LoadNextScene(1.0f);
		}
	}
}
