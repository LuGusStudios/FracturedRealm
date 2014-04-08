using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSequenceTester : MonoBehaviour 
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

	protected void OnGUI()
	{
		
		GUILayout.BeginArea( new Rect(0, 100, 300, 300) );	
		GUILayout.BeginVertical();

		Character[] characters = GameObject.FindObjectsOfType<Character>();
	
		//characters = new Character[]{ characters[0] };
		Character character = characters[0];

		if( GUILayout.Button("CrossFade 1") )
		{
			LugusCoroutines.use.StartRoutine( TestCrossFadeAfterNormalized(characters) );
		}
		
		if( GUILayout.Button("FRAnimationsTest mortar") )
		{
			characters[0].GetComponent<Animator>().CrossFade( (int) FRAnimation.shootMortar, 0.1f );
			characters[1].GetComponent<Animator>().CrossFade( "/Subtract/Attacks.shootMortar", 0.1f );

		}

		AnimatorStateInfo stateInfo = character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
		GUILayout.Label( stateInfo.length + " // " + stateInfo.normalizedTime );
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	protected IEnumerator TestCrossFadeAfterNormalized(Character[] characters)
	{
		foreach( Character c in characters )
		{
			Animator animator = c.GetComponent<Animator>();

			animator.SetTrigger("turnLeft"); 

			while( !animator.IsInTransition(0) )
				yield return null;

			Debug.Log ("Animator is in transition now. " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime );

			while( animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.85f )
				yield return null;

			animator.CrossFade( "Movements.test.running", 0.15f );
			//animator.Play ( "running" );
		}
	}
}








