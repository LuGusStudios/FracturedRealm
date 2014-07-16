using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartMenu : IMenu 
{
	public Transform Logo = null;
	public Transform PlayButton = null;

	public Transform OptionsButton = null;
	public Transform ExtraButton = null;

	public Transform SoundButton = null;
	public Transform InfoButton = null;
	public Transform FacebookButton = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		Logo = transform.FindChildRecursively("Logo");
		PlayButton = transform.FindChild("PlayButton");

		OptionsButton = transform.FindChild("OptionsButton");
		ExtraButton = transform.FindChild("ExtraButton");

		SoundButton = transform.FindChildRecursively("SoundButton");
		InfoButton = transform.FindChildRecursively("InfoButton"); 
		FacebookButton = transform.FindChildRecursively("FacebookButton");
	}

	public override bool Activate(bool force = false)
	{
		if( activated && !force )
			return false;

		this.transform.position = Vector3.zero;

		Logo.transform.position = Logo.transform.position.y( 20.28841f ); 
		PlayButton.transform.position = PlayButton.transform.position.y( -2.647452f );
		OptionsButton.transform.position = OptionsButton.transform.position.x ( -1.408128f );
		ExtraButton.transform.position = ExtraButton.transform.position.x( 22.06981f );

		Logo.gameObject.MoveTo( Logo.transform.position.y(9.198807f) ).Delay(0.5f).Time(1.0f).EaseType(iTween.EaseType.easeOutBack).Execute();
		PlayButton.gameObject.MoveTo( PlayButton.transform.position.y(3.097763f) ).Delay(0.8f).Time(1.0f).EaseType(iTween.EaseType.easeOutBack).Execute();
		OptionsButton.gameObject.MoveTo( OptionsButton.transform.position.x(1.598085f) ).Delay(1.5f).EaseType(iTween.EaseType.easeOutBack).Time(0.5f).Execute();
		ExtraButton.gameObject.MoveTo( ExtraButton.transform.position.x(18.72956f) ).Delay(1.5f).EaseType(iTween.EaseType.easeOutBack).Time(0.5f).Execute();

		activated = true;
		return true;
	}

	public override bool Deactivate(bool force = false)
	{
		if( !activated && !force )
			return false;

		this.transform.position = LugusUtil.OFFSCREEN;

		OptionsButton.GetComponent<FoldoutMenu>().Close(false);
		ExtraButton.GetComponent<FoldoutMenu>().Close(false);

		activated = false;

		return true;
	}
	
	
	protected void Update () 
	{
		Transform hit = LugusInput.use.RayCastFromMouseUp2D();
		if( hit == null )
			return;

		if( hit == PlayButton )
		{
			MenuManager.use.ChangeState(MenuManager.MenuState.WorldSelect);
		}

		if( hit == InfoButton )
		{
			Debug.LogError("InfoButton clicked");
		}
		
		if( hit == FacebookButton )
		{
			Debug.LogError("FacebookButton clicked");
		}

		if( hit == SoundButton )
		{
			Debug.LogError("SoundButton clicked");
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
