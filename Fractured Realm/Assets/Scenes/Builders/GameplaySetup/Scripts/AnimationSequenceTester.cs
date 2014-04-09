using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CharacterOrientationInfo
{
	public enum RelativePosition
	{
		NONE = -1,
		
		RIGHT = 1,
		LEFT = 2,
		FRONT = 3,
		BACK = 4
	}

	protected RelativePosition _positionType = RelativePosition.NONE;
	public RelativePosition positionType
	{
		get
		{
			if( _positionType != RelativePosition.NONE )
			{
				return _positionType;
			}

			// take a region around 0 to check (to make up for small errors in rotations etc.)
			// in general, we want FRONT and BACK if we only have a small angle difference (max 10 degrees) and left/right if not

			// TODO: tryout alternative method : use angles as first resource to determine direction and only use xPosition if we're not use if we need to turn left or right at BACK
			// might be more robust? 
			// this code will not work if the targets are very close to each other...

			if( xPosition > 2.0f )
			{
				_positionType = RelativePosition.RIGHT;
			}
			else if( xPosition < -2.0f )
			{
				_positionType = RelativePosition.LEFT;
			}
			else
			{
				// position is around 0, but we're not sure if we're facing the target or if it's behind us
				// if the angle is larger than 90 degrees, the target is behind us
				if( angle <= 90.0f )
					_positionType = RelativePosition.FRONT;
				else
					_positionType = RelativePosition.BACK;

			}

			return _positionType;
		}
	}

	public float xPosition = 0.0f;
	public float angle = 0.0f;
	public Vector3 targetDirection = Vector3.zero;
	public Quaternion lookRotation = Quaternion.identity;

	public int animationDegrees = 0;

	public void Fill( Transform subject, Transform target )
	{
		_positionType = RelativePosition.NONE;

		// use the LOCAL xPosition (local space of the subject) to see if the target is to the "left" or "right"
		// we can use use x because we're in the local space and the x-axis is always setup correctly along our "horizontal base"

		// following a left handed coordinate system (forward z is positive, right x is positive)
		// x > 0 => target is on the right side
		// x < 0 => target is on the left side
		// x == 0 => target is either in front of us, or behind us (use angle to differentiate between those cases)
		xPosition = subject.InverseTransformPoint( target.position ).x;


		targetDirection = target.position - subject.position;

		// calculate angle between subject looking forward and the direction of the target
		// this gives us the angle we need to rotate to face the target directly

		// Vector3.Angle() always gives us the smallest angle (between 0 and 180) between the vectors
		angle = Vector3.Angle( subject.forward, targetDirection );


		// directly usable version of angle
		// easy to use in a lerp to rotatte towards the target using code (as opposed to root motion in an animation)
		// ex. c.transform.rotation = Quaternion.Lerp( c.transform.rotation, info.lookRotation, Time.deltaTime );
		lookRotation = Quaternion.LookRotation( targetDirection );

		CalculateAnimationDegrees();
	}
	
	// direction should be in WORLD space!
	public void Fill( Transform subject, Vector3 direction )
	{
		_positionType = RelativePosition.NONE;

		// if we get a direction directly (targetDir), we can calculate everything but the xPosition... or can we! :)
		// we just add the LOCALIZED direction to the subject's position, to get an xPosition that will work

		// direction * 5 to make sure it's big enough to work in our positionType calculation
		// basically: create our own "target" position by adding the direction (enlarged) to the current vector, pretending it's an actual point in space
		// TODO: make positionType calculation more robust :)
		Vector3 directionPosition = subject.position + (direction * 5);
		xPosition = subject.InverseTransformPoint( directionPosition ).x;

		targetDirection = direction; // we can use direction directly, or we can use directionPosition - subject.position. Both are the same "direction", just different magnitudes
		angle = Vector3.Angle( subject.forward, targetDirection );
		lookRotation = Quaternion.LookRotation( targetDirection );
		
		
		CalculateAnimationDegrees();

		//Debug.Log("Fill orientationinfo from " + subject.transform.position + " and direction " + direction + " gives " + positionType + ", " + angle + ", " + xPosition );
	}
	
	protected void CalculateAnimationDegrees()
	{
		// calculate "bucket" for the correct animations
		// buckets:
		// 0 - 5 degrees : no bucket, no rotation
		// 5 - 15 degrees : bucket index 0, 10 degrees rotation
		// 15 - 25 degrees : bucket index 1, 20 degrees rotation
		// etc.
		
		// algorithm: FLOOR( (angle - 5) / 10 )
		// ex. 24 - 5 = 19. 19/10 = 1.9. Floor(1.9) = 1
		// lastly, take the index + 1 and multiply by 10 to get the desired rotation and fetch the correct animation
		
		animationDegrees = 0;
		
		if( angle > 90.0f )
			angle = 90.0f; // TODO: Fix this so we can also do correct animations for rotations > 180 degrees (extra animations or chaining animations)
		
		if( angle > 5.0f )
		{
			int bucketIndex = Mathf.FloorToInt( (angle - 5.0f) / 10.0f );
			animationDegrees = (bucketIndex + 1) * 10;
		}
	}
}

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

	CharacterOrientationInfo info0 = new CharacterOrientationInfo();
	CharacterOrientationInfo info1 = new CharacterOrientationInfo();
	CharacterOrientationInfo infox = new CharacterOrientationInfo();


	protected void OnGUI()
	{
		
		GUILayout.BeginArea( new Rect(0, 100, 400, 300) );	
		GUILayout.BeginVertical();

		Character[] characters = GameObject.FindObjectsOfType<Character>();
	
		//characters = new Character[]{ characters[0] };
		Character character = characters[0];

		if( GUILayout.Button("Rotate randomly") )
		{
			characters[0].transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);
			characters[1].transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);
		}

		if( GUILayout.Button("Run to each other") )
		{
			LugusCoroutines.use.StartRoutine(TestCrossFadeAfterNormalized(0, characters) );
		}
		
		if( GUILayout.Button("Run to camera") )
		{
			LugusCoroutines.use.StartRoutine(TestCrossFadeAfterNormalized(1, characters) );
		}
		if( GUILayout.Button("Run away from camera") )
		{
			LugusCoroutines.use.StartRoutine(TestCrossFadeAfterNormalized(2, characters) );
		}
		
		if( GUILayout.Button("Run left on screen") )
		{
			LugusCoroutines.use.StartRoutine(TestCrossFadeAfterNormalized(3, characters) );
		}

		if( GUILayout.Button("Run right on screen") )
		{
			LugusCoroutines.use.StartRoutine(TestCrossFadeAfterNormalized(4, characters) );
		}
		
		if( GUILayout.Button("FRAnimationsTest mortar") )
		{
			characters[0].GetComponent<Animator>().CrossFade( (int) FRAnimation.shootMortar, 0.1f );
			characters[1].GetComponent<Animator>().CrossFade( "/Subtract/Attacks.shootMortar", 0.1f );

		}

		AnimatorStateInfo stateInfo = character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
		GUILayout.Label( stateInfo.length + " // " + stateInfo.normalizedTime );

		
		//GUILayout.Label( "Character 0 is facing " + characters[0].transform.forward );
		info0.Fill( characters[0].transform, characters[1].transform );
		info1.Fill( characters[1].transform, characters[0].transform );
		GUILayout.Label( "Character 0's target is to the " + info0.positionType + " at angle " + info0.angle );
		//GUILayout.Label( "Character 1 is facing " + characters[1].transform.forward );
		GUILayout.Label( "Character 1's target is to the " + info1.positionType + " at angle " + info1.angle );

		infox.Fill( characters[0].transform, Vector3.back );
		GUILayout.Label( "Character 0's is relative to global forward " + infox.positionType + " at angle " + infox.angle + " and pos " + infox.xPosition );

		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	int crossfadeCounter = 0;

	protected IEnumerator TestCrossFadeAfterNormalized(int type, Character[] characters)
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
					orientationInfo.Fill( characters[0].transform, characters[1].transform );
				else if( type == 1 )
					orientationInfo.Fill( characters[0].transform, Vector3.back ); // facing camera (is actually looking back)
				else if( type == 2 )
					orientationInfo.Fill( characters[0].transform, Vector3.forward ); // looking away from camera
				else if( type == 3 )
					orientationInfo.Fill( characters[0].transform, Vector3.left ); // looking left
				else if( type == 4 )
					orientationInfo.Fill( characters[0].transform, Vector3.right ); // looking right


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
					orientationInfo.Fill( characters[1].transform, characters[0].transform );
				else if( type == 1 )
					orientationInfo.Fill( characters[1].transform, Vector3.back );
				else if( type == 2 )
					orientationInfo.Fill( characters[1].transform, Vector3.forward );
				else if( type == 3 )
					orientationInfo.Fill( characters[1].transform, Vector3.right );
				else if( type == 4 )
					orientationInfo.Fill( characters[1].transform, Vector3.left );
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
					// TODO: fix this so the turnLeft becomes turnLeft90 for consistency
					if( orientationInfo.animationDegrees == 90 )
					{
						animationName = "turnLeft";
					}
					else
					{
						animationName += "" + orientationInfo.animationDegrees;
					}
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
				animator.SetTrigger("turnRight"); 


				while( animator.GetCurrentAnimatorStateInfo(0).nameHash != (int) FRAnimation.turnRight )
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








