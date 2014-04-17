using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Util class for use in animations and movement of characters
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
	
	public void Fill( Transform subject, Vector3 target )
	{
		_positionType = RelativePosition.NONE;
		
		// use the LOCAL xPosition (local space of the subject) to see if the target is to the "left" or "right"
		// we can use use x because we're in the local space and the x-axis is always setup correctly along our "horizontal base"
		
		// following a left handed coordinate system (forward z is positive, right x is positive)
		// x > 0 => target is on the right side
		// x < 0 => target is on the left side
		// x == 0 => target is either in front of us, or behind us (use angle to differentiate between those cases)
		xPosition = subject.InverseTransformPoint( target ).x;
		
		
		targetDirection = target - subject.position;
		
		// calculate angle between subject looking forward and the direction of the target
		// this gives us the angle we need to rotate to face the target directly
		
		// Vector3.Angle() always gives us the smallest angle (between 0 and 180) between the vectors
		angle = Vector3.Angle( subject.forward, targetDirection );
		
		
		// directly usable version of angle
		// easy to use in a lerp to rotatte towards the target using code (as opposed to root motion in an animation)
		// ex. c.transform.rotation = Quaternion.Lerp( c.transform.rotation, info.lookRotation, Time.deltaTime );
		lookRotation = Quaternion.LookRotation( targetDirection, Vector3.up );
		
		CalculateAnimationDegrees();
	}
	
	// direction should be in WORLD space!
	public void FillDirection( Transform subject, Vector3 direction )
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
		lookRotation = Quaternion.LookRotation( targetDirection, Vector3.up );
		
		
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

		float originalAngle = angle;

		if( angle > 90.0f )
			angle = 90.0f; // TODO: Fix this so we can also do correct animations for rotations > 180 degrees (extra animations or chaining animations)
		
		if( angle > 5.0f )
		{
			int bucketIndex = Mathf.FloorToInt( (angle - 5.0f) / 10.0f );
			animationDegrees = (bucketIndex + 1) * 10;
		}

		angle = originalAngle;
	}
}


public class CharacterAnimator : MonoBehaviour 
{
	protected Character _renderer = null;
	public Character Renderer
	{
		get
		{ 	
			if( _renderer == null )
				_renderer = GetComponent<Character>();

			return _renderer; 
		}
		set{ _renderer = value; }
	}

	protected Animator _animator = null;

	public ILugusCoroutineHandle RotateTowards(Vector3 target)
	{
		return this.gameObject.StartLugusRoutine( RotateTowardsRoutine(target, false) );
	}
	
	public void RotateTowardsDirect(Vector3 target)
	{
		CharacterOrientationInfo info = new CharacterOrientationInfo();
		info.Fill( this.transform, target );

		this.transform.rotation = info.lookRotation;
	}
	
	public ILugusCoroutineHandle RotateInDirection(Vector3 direction)
	{
		//return LugusCoroutines.use.GetHandle().StartRoutine( RotateTowardsRoutine(null, direction) );
		return this.gameObject.StartLugusRoutine( RotateTowardsRoutine(direction, true) );
	}

	public IEnumerator RotateTowardsRoutine(Vector3 target, bool direction)
	{
		CharacterOrientationInfo info = new CharacterOrientationInfo();
		if( !direction )
			info.Fill( this.transform, target );
		else
			info.FillDirection( this.transform, target );

		// if we're already looking at target, there's no need to rotate
		if( info.animationDegrees == 0 ||
		   info.positionType == CharacterOrientationInfo.RelativePosition.BACK ||
		   info.positionType == CharacterOrientationInfo.RelativePosition.FRONT )
		{
			// TODO: possibly still rotate, but without animation (just use lookRotation and Quaternion.Lerp)
			Debug.LogWarning("CharacterAnimator:RotateTowardsRoutine : no rotation needed, already facing target");
			yield break;
		}

		FRAnimation turnAnimationType = FRAnimation.NONE;
		string animationName = "turnLeft";

		if( info.positionType == CharacterOrientationInfo.RelativePosition.RIGHT )
		{
			animationName = "turnRight";
		}

		animationName += info.animationDegrees;

		Debug.Log("CharacterAnimator:RotateTowardsRoutine : Rotating "+ info.positionType +" by " + info.angle + " degrees with animation degrees " + info.animationDegrees + " and anim " + animationName + " // " + this.transform.name );




		turnAnimationType = FRAnimations.TypeFromString( animationName );

		if( turnAnimationType == FRAnimation.NONE )
		{
			Debug.LogError("CharacterAnimator:RotateTowardsRoutine : turnAnimationType was NONE for string " + animationName);
			yield break;
		}

		//Debug.Log ("Looking up FRANimationData for key " + turnAnimationType + "//" + ( (int) turnAnimationType) + " from string " + animationName );

		FRAnimationData turnAnimation = FRAnimations.animations[ (int) turnAnimationType ];
		
		Animator animator = GetComponent<Animator>();

		int hash = animator.GetCurrentAnimatorStateInfo(turnAnimation.layer).nameHash;

		CrossFade( turnAnimation, 0.05f );
		//animator.CrossFade( turnAnimation.hash, 0.05f ); // similar to SetTrigger, but we can use FRAnimation directly here!

		// wait untill we've started to play the turnAnimation
		while( animator.GetCurrentAnimatorStateInfo(turnAnimation.layer).nameHash == hash )
		{
			yield return null;
		}
		
		while( animator.GetCurrentAnimatorStateInfo(turnAnimation.layer).normalizedTime < 0.85f )
			yield return null;

		// our animations are a maximum of 90 degrees rotation per time
		// if we have to rotate more than 90 degrees, we need to rotate again...
		if( info.angle <= 90.0f )
		{
			// nothing to do here, we're done
		}
		else
		{
			if( info.angle - 90.0f <= 5.0f ) // if smaller than 5 degrees, we manually lerp the rotation (no animation available) (See below)
			{
				//yield return gameObject.StartLugusRoutine( RotateTowardsLerp(info.lookRotation) );
			}
			else
			{
				// if more than 5 degreest still to turn, call this function recursively
				// we have already rotated 90 degrees by now, so the leftover value should be 
				yield return gameObject.StartLugusRoutine( RotateTowardsRoutine(target, direction) ).Coroutine;
			}
		}

		// now that we have rotated, we might still be off by a couple of degrees (5 max) due to the animations
		// just interpolate to correct, but allow other animations to start already

		gameObject.StartLugusRoutine( RotateTowardsLerp( info.lookRotation, 1.0f) );

		yield break;
	}

	protected IEnumerator RotateTowardsLerp(Quaternion targetRotation, float duration)
	{
		float startTime = Time.time;
		Quaternion originalRotation = transform.rotation;

		while( Time.time - startTime < duration )
		{
			transform.rotation = Quaternion.Lerp( originalRotation, targetRotation, (Time.time - startTime) / duration );

			yield return null;
		}
	}

	public void CrossFade( string animationName, float fadeDuration )
	{
		FRAnimation animationType = FRAnimations.TypeFromString( animationName );
		
		if( animationType == FRAnimation.NONE )
		{
			Debug.LogError("CharacterAnimator:CrossFade : animationType was NONE for string " + animationName);
			return;
		}

		CrossFade( animationType, fadeDuration );
	}

	public void CrossFade( FRAnimationData animation, float fadeDuration )
	{
		CrossFade( (FRAnimation) animation.hash, fadeDuration );
	}

	public void CrossFade( FRAnimation animation, float fadeDuration )
	{
		_animator.CrossFade( (int) animation, 0.05f );
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		_animator = GetComponent<Animator>();

		if( _animator == null )
		{
			Debug.LogError("CharacterAnimator:SetupLocal : no Animator found for this object!");
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
	
	}
}
