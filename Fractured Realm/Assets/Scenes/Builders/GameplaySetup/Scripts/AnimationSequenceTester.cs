using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AnimationSequenceTester : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}

	bool setupComplete = false;
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		
		NumberRenderer[] characters = GameObject.FindObjectsOfType<NumberRenderer>();
		foreach( NumberRenderer c in characters )
		{
			GameObject.Destroy( c.gameObject );
		}

		WorldFactory.use.CreateFractions( WorldFactory.use.debug_initialFractions );

		setupComplete = true;
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

	CharacterOrientationInfo info0 = new CharacterOrientationInfo();
	CharacterOrientationInfo info1 = new CharacterOrientationInfo();
	CharacterOrientationInfo infox = new CharacterOrientationInfo();


	protected void OnGUI()
	{
		if( !setupComplete )
			return;
		
		GUILayout.BeginArea( new Rect(0, 100, 400, 300) );	
		GUILayout.BeginVertical();

		NumberRenderer[] characters = GameObject.FindObjectsOfType<NumberRenderer>();
	
		//characters = new Character[]{ characters[0] };
		//Character character = characters[0];

		if( GUILayout.Button("Rotate randomly") )
		{
			characters[0].transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);
			characters[1].transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);
		}

		if( GUILayout.Button("Run to each other") )
		{
			LugusCoroutines.use.StartRoutine(RotateAndRun2(0, characters) );
		}
		
		if( GUILayout.Button("Run to camera") )
		{
			LugusCoroutines.use.StartRoutine(RotateAndRun2(1, characters) );
		}
		if( GUILayout.Button("Run away from camera") )
		{
			LugusCoroutines.use.StartRoutine(RotateAndRun2(2, characters) );
		}
		
		if( GUILayout.Button("Run left on screen") )
		{
			LugusCoroutines.use.StartRoutine(RotateAndRun2(3, characters) );
		}

		if( GUILayout.Button("Run right on screen") )
		{
			LugusCoroutines.use.StartRoutine(RotateAndRun2(4, characters) );
		}

		/*
		if( GUILayout.Button("FRAnimationsTest mortar") )
		{
			characters[0].GetComponent<Animator>().CrossFade( (int) FRAnimation.shootMortar, 0.1f );
			characters[1].GetComponent<Animator>().CrossFade( "/Subtract/Attacks.shootMortar", 0.1f );

		}
		*/

		//AnimatorStateInfo stateInfo = character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
		//GUILayout.Label( stateInfo.length + " // " + stateInfo.normalizedTime );

		
		//GUILayout.Label( "Character 0 is facing " + characters[0].transform.forward );
		info0.Fill( characters[0].interactionCharacter.transform, characters[1].interactionCharacter.transform.position );
		info1.Fill( characters[1].interactionCharacter.transform, characters[0].interactionCharacter.transform.position );
		GUILayout.Label( characters[0].name + " : Character 0's target is to the " + info0.positionType + " at angle " + info0.angle );
		//GUILayout.Label( "Character 1 is facing " + characters[1].transform.forward );
		GUILayout.Label( characters[1].name + " : Character 1's target is to the " + info1.positionType + " at angle " + info1.angle );

		infox.FillDirection( characters[0].interactionCharacter.transform, Vector3.back );
		GUILayout.Label( "Character 0's is relative to global forward " + infox.positionType + " at angle " + infox.angle + " and pos " + infox.xPosition );

		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	int crossfadeCounter = 0;

	protected IEnumerator RotateAndRun2(int type, NumberRenderer[] characters)
	{

		
		foreach( NumberRenderer c in characters )
		{
			if( c.interactionCharacter == null )
				Debug.LogError(c.name + " InteractionCharacter");
			else if( c.interactionCharacter.Animator == null )
				Debug.LogError(c.name + " CharacterAnimator");

			Animator animator = c.interactionCharacter.Animator.GetComponent<Animator>();

			while( animator.IsInTransition(0) )
				yield return null;


			CharacterAnimator anim = c.interactionCharacter.Animator;

			if( type == 0 )
			{
				if( c == characters[0] )
				{
					yield return anim.RotateTowards( characters[1].transform.position );
				}
				else
				{
					yield return anim.RotateTowards( characters[0].transform.position );
				}
			}

			else if( type == 1 )
				yield return anim.RotateInDirection( Vector3.back ); // facing camera (is actually looking back)
			else if( type == 2 )
				yield return anim.RotateInDirection( Vector3.forward ); // looking away from camera
			else if( type == 3 )
				yield return anim.RotateInDirection( Vector3.left ); // looking left
			else if( type == 4 )
				yield return anim.RotateInDirection( Vector3.right ); // looking right

			
			//while( !animator.IsInTransition(0) )
			//	yield return null;
			
			
			
			//Debug.Log ("Animator is in transition now. " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime );
			
			
			animator.CrossFade( (int) FRAnimation.running /*"/Movements.running"*/, 0.15f );
			//animator.Play ( "running" );
			
			while( animator.GetCurrentAnimatorStateInfo(0).nameHash != (int) FRAnimation.running )
			{
				yield return null;
			}
			
			while( animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f )
				yield return null;
			
			/*
			Quaternion targetRotation = orientationInfo.lookRotation;//Quaternion.LookRotation( targetDir );
			Debug.LogError("Target rotation is " + targetRotation.eulerAngles + " from targetDir " + orientationInfo.targetDirection );
			
			float startTime = Time.time;
			while( Vector3.Angle( c.transform.forward, orientationInfo.targetDirection ) > 1.0f && (Time.time - startTime < 2.0f) )
			{
				//Debug.Log (Time.frameCount + " Lerping quaternions ");
				c.transform.rotation = Quaternion.Lerp( c.transform.rotation, targetRotation, Time.deltaTime );
				yield return null;
			}
			
			c.transform.rotation = targetRotation;
			*/
		}
	}

	protected IEnumerator RotateAndRun(int type, Character[] characters)
	{
		crossfadeCounter += 1;
		if( crossfadeCounter > 3 )
			crossfadeCounter = 0;

		//Debug.LogWarning("Crossfade counter is now : " + crossfadeCounter);


		CharacterOrientationInfo orientationInfo = new CharacterOrientationInfo();

		foreach( Character c in characters )
		{

			if( c == characters[0] )
			{
				if( type == 0 )
					orientationInfo.Fill( characters[0].transform, characters[1].transform.position );
				else if( type == 1 )
					orientationInfo.FillDirection( characters[0].transform, Vector3.back ); // facing camera (is actually looking back)
				else if( type == 2 )
					orientationInfo.FillDirection( characters[0].transform, Vector3.forward ); // looking away from camera
				else if( type == 3 )
					orientationInfo.FillDirection( characters[0].transform, Vector3.left ); // looking left
				else if( type == 4 )
					orientationInfo.FillDirection( characters[0].transform, Vector3.right ); // looking right


				/*
				Vector3 otherForward = characters[1].transform.forward;
				float otherForwardX = otherForward.x;

				float angleDot =  Vector3.Dot( characters[0].transform.forward, otherForward );
				float angle = Mathf.Acos( angleDot );
				Debug.LogWarning("Angle between two characters, seen from 0, is : " + angle + " // dot : " + angleDot);

				// angle close to -180 -> facing each other
				// close to 0 : same direction facing -> turn is needed
				// close to 180 : facing opposite direction: double turn needed
				
				Vector3 rotatedForward = otherForward.x (otherForward.y) .y( -otherForwardX );
				
				float dot = Vector3.Dot( characters[0].transform.forward, rotatedForward );
				if( dot > 0 )
				{
					Debug.Log ("For character 0, character 1 is on the right : TURN RIGHT : " + dot);
					left = false;
				}
				else if( dot < 0 )
				{
					Debug.Log ("For character 0, character 1 is on the left : TURN LEFT " + dot);
				}
				else
				{
					Debug.Log ("For character 0, character 1 is parallell : TURN ? " + dot);
				}
				*/

			}
			if( c == characters[1] )
			{
				if( type == 0 )
					orientationInfo.Fill( characters[1].transform, characters[0].transform.position );
				else if( type == 1 )
					orientationInfo.FillDirection( characters[1].transform, Vector3.back );
				else if( type == 2 )
					orientationInfo.FillDirection( characters[1].transform, Vector3.forward );
				else if( type == 3 )
					orientationInfo.FillDirection( characters[1].transform, Vector3.right );
				else if( type == 4 )
					orientationInfo.FillDirection( characters[1].transform, Vector3.left );
				/*
				Vector3 otherForward = characters[0].transform.forward;
				float otherForwardX = otherForward.x;
				
				float angleDot =  Vector3.Dot( characters[0].transform.forward, otherForward );
				float angle = Mathf.Acos( angleDot );
				Debug.LogWarning("Angle between two characters, seen from 1, is : " + angle + " // dot : " + angleDot);

				Vector3 rotatedForward = otherForward.x (otherForward.y) .y( -otherForwardX );
				
				float dot = Vector3.Dot( characters[1].transform.forward, rotatedForward );
				if( dot > 0 )
				{
					Debug.Log ("For character 0, character 1 is on the right : TURN RIGHT : " + dot);
					left = false;
				}
				else if( dot < 0 )
				{
					Debug.Log ("For character 0, character 1 is on the left : TURN LEFT " + dot);
				}
				else
				{
					Debug.Log ("For character 0, character 1 is parallell : TURN ? " + dot);
				}
				*/
			}

			Animator animator = c.GetComponent<Animator>();
			
			while( animator.IsInTransition(0) )
				yield return null;




			if( orientationInfo.positionType == CharacterOrientationInfo.RelativePosition.LEFT )
			{
				int hash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

				
				string animationName = "turnLeft";
				if( orientationInfo.animationDegrees != 0 )
				{
					animationName += "" + orientationInfo.animationDegrees;
				}


				// TODO: if degrees > 90... double rotation is better then! but should still be over quickly!
				// TODO: add buckets : 0-5 = no turn, 5 - 15 = 10deg turn, 15 - 25 = 20deg turn etc.
				Debug.LogWarning("Rotating left by " + orientationInfo.angle + " degrees with animation degrees " + orientationInfo.animationDegrees + " and anim " + animationName );
				animator.SetTrigger( animationName );//"turnLeft");

				//while( animator.GetCurrentAnimatorStateInfo(0).nameHash != (int) FRAnimation.turnLeft )
				while( animator.GetCurrentAnimatorStateInfo(0).nameHash == hash )
				{
					yield return null;
				}


				while( animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.85f )
					yield return null;
			}
			else if( orientationInfo.positionType == CharacterOrientationInfo.RelativePosition.RIGHT )
			{
				// TODO: if degrees > 90... double rotation is better then! but should still be over quickly!
				Debug.LogWarning("Rotating right by " + orientationInfo.angle + " degrees with animation degrees " + orientationInfo.animationDegrees );
				animator.SetTrigger("turnRight90"); 


				while( animator.GetCurrentAnimatorStateInfo(0).nameHash != (int) FRAnimation.turnRight90 )
				{
					yield return null;
				}

				while( animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.85f )
					yield return null;
			}


			//while( !animator.IsInTransition(0) )
			//	yield return null;



			//Debug.Log ("Animator is in transition now. " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime );


			animator.CrossFade( "/Movements.running", 0.15f );
			//animator.Play ( "running" );

			while( animator.GetCurrentAnimatorStateInfo(0).nameHash != (int) FRAnimation.running )
			{
				yield return null;
			}

			while( animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f )
				yield return null;


			
			/*
			while( animator.GetCurrentAnimatorStateInfo(0).nameHash != (int) FRAnimation.idle )
			{
				yield return null;
			}
			*/

			/*
			Vector3 targetDir = new Vector3( 0, 0, -1.0f ); // facing towards camera, counter == 0 // try replace with Vector3.front
			if( crossfadeCounter == 1 )
			{
				targetDir = new Vector3( 1.0f, 0, 0.0f ); // facing right, counter == 1 // try replace with Vector3.right
			}
			else if( crossfadeCounter == 2 )
			{
				targetDir = new Vector3( 0.0f, 0, 1.0f ); // facing back, counter == 2 // try replace with Vector3.back
			}
			else if( crossfadeCounter == 3 )
			{
				targetDir = new Vector3( -1.0f, 0, 0.0f ); // facing left, counter == 3 // try replace with Vector3.left
			}
			*/


			// Look at other character
			//http://stackoverflow.com/questions/13221873/determining-if-one-2d-vector-is-to-the-right-or-left-of-another

			/*
			if( c == characters[0] )
			{
				targetDir = characters[1].transform.position - characters[0].transform.position;

			}
			else
			{
				targetDir = characters[0].transform.position - characters[1].transform.position;
			}
			*/



			// begin transform.forward nemen en AngleAxis gebruiken om 90 graden te berekenen?
			// face forward is dan altijd gewoon die originele forward
			// rotatie mogelijk toch tijdens turnLeft al doen? als we telkens lookAt target gaan doen ipv per 90 graden te werken?
			// bijv. als ge gaat + doen is het nie rotate 90 maar rotate towards opponent?

			//Quaternion targetRotation = Quaternion.FromToRotation( targetDir, c.transform.forward  );//Quaternion.FromToRotation( c.transform.forward, targetDir );
			Quaternion targetRotation = orientationInfo.lookRotation;//Quaternion.LookRotation( targetDir );
			Debug.LogError("Target rotation is " + targetRotation.eulerAngles + " from targetDir " + orientationInfo.targetDirection );

			float startTime = Time.time;
			while( Vector3.Angle( c.transform.forward, orientationInfo.targetDirection ) > 1.0f && (Time.time - startTime < 2.0f) )
			{
				//Debug.Log (Time.frameCount + " Lerping quaternions ");
				c.transform.rotation = Quaternion.Lerp( c.transform.rotation, targetRotation, Time.deltaTime );
				yield return null;
			}

			c.transform.rotation = targetRotation;
		}
	}
}








