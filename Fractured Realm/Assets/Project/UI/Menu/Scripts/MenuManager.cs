using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : LugusSingletonExisting<MenuManager> 
{
	public enum MenuState
	{
		None = -1,

		Start = 1,
		WorldSelect = 2,
		LevelSelect = 3
	}

	public MenuState currentState = MenuState.None;

	protected List<IMenu> allMenus = new List<IMenu>();

	public StartMenu startMenu = null;
	public WorldSelectMenu worldMenu = null;
	public LevelSelectMenu levelMenu = null;

	public delegate void OnMenuStateChanged(MenuState oldState, MenuState newState);
	public OnMenuStateChanged onMenuStateChanged;

	public void SetupLocal()
	{
		if( startMenu == null )
		{
			StartMenu[] ms = transform.GetComponentsInChildren<StartMenu>(true);
			if( ms != null && ms.Length > 0 )
				startMenu = ms[0];
		}

		if( startMenu == null )
		{
			Debug.LogError("MenuManager:SetupLocal : no startMenu found!");
		}

		
		if( worldMenu == null )
		{
			WorldSelectMenu[] ms = transform.GetComponentsInChildren<WorldSelectMenu>(true);
			if( ms != null && ms.Length > 0 )
				worldMenu = ms[0];
		}
		
		if( worldMenu == null )
		{
			Debug.LogError("MenuManager:SetupLocal : no worldMenu found!");
		}
		
		
		if( levelMenu == null )
		{
			LevelSelectMenu[] ms = transform.GetComponentsInChildren<LevelSelectMenu>(true);
			if( ms != null && ms.Length > 0 )
				levelMenu = ms[0];
		}
		
		if( levelMenu == null )
		{
			Debug.LogError("MenuManager:SetupLocal : no levelMenu found!");
		}

		allMenus.Add( startMenu ); 
		allMenus.Add( worldMenu );
		allMenus.Add( levelMenu );
	}

	public void ChangeStateDelayed(MenuState newState, float delay) 
	{
		gameObject.StartLugusRoutine( ChangeStateDelayedRoutine(newState, delay) );
	}

	protected IEnumerator ChangeStateDelayedRoutine(MenuState newState, float delay)
	{
		if( delay > 0.0f )
			yield return new WaitForSeconds(delay);

		ChangeState( newState );
	}

	public void ChangeState(MenuState newState)
	{
		MenuState oldState = currentState;
		currentState = newState;

		if( oldState == newState )
		{
			Debug.LogWarning("MenuManager:ChangeState : state was the same! " + newState + ". Nothing happened.");
			return;
		}


		foreach( IMenu menu in allMenus )
		{
			menu.Deactivate();
		}

		if( newState == MenuState.Start )
		{
			startMenu.Activate();
		}
		else if( newState == MenuState.WorldSelect )
		{
			worldMenu.Activate();
		}
		else if( newState == MenuState.LevelSelect )
		{
			levelMenu.Activate();
		}

		Debug.Log ("MenuManager:ChangeState : changed from " + oldState + " to " + newState);

		if( onMenuStateChanged != null )
			onMenuStateChanged(oldState, newState);
	}

	public static void ButtonPressEffect( Transform button, float time = 1.0f )
	{
		float amount = Mathf.Max(time, 0.7f) / 2.0f;
		iTween.PunchScale( button.gameObject, new Vector3( amount, amount, amount ), time); //new Vector3( 0.75f, 0.75f, 0.75f ), time);
	}

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		ChangeState( MenuState.Start );
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
}
