using UnityEngine;
using System.Collections;

public class NumberAnimator : MonoBehaviour 
{
	protected NumberRenderer _renderer = null;
	public NumberRenderer Renderer
	{
		get
		{ 	
			if( _renderer == null )
				_renderer = GetComponent<NumberRenderer>();
			
			return _renderer; 
		}
		set{ _renderer = value; }
	}

	public Vector3 GetOpponentPosition( NumberRenderer target )
	{
		Vector3 output = Vector3.zero;

		if( target != null )
			output = target.interactionCharacter.Body.transform.position.y( target.transform.position.y );

		return output;
	}

	public Vector3 GetEffectPosition(Vector3 basePosition)
	{
		// effect usually has to cover up an animation (ex. add hit)
		// so we need to take camera position into account.

		//so: take basePosition and move it towards camera in perspective line (direction)

		Camera cam = null;
		if( this.Renderer.Number.IsNumerator )
			cam = LugusCamera.numerator;
		else
			cam = LugusCamera.denominator;

		Vector3 direction = basePosition - cam.transform.position;
		direction.Normalize();

		return (basePosition - (direction * 5.0f));
	}

	public ILugusCoroutineHandle SpawnEffect(FR.EffectType effectType)
	{
		return SpawnEffect( effectType, this.Renderer.interactionCharacter.Head.position );
	}
	
	public ILugusCoroutineHandle SpawnEffect( FR.EffectType effectType, Vector3 basePosition )
	{		
		return SpawnEffect( effectType, this.Renderer.interactionCharacter.Head.position, new Vector3(0, 0.5f, 0) );
	}

	public ILugusCoroutineHandle SpawnEffect( FR.EffectType effectType, Vector3 basePosition, Vector3 offset )
	{		
		Effect effect = EffectFactory.use.CreateEffect( this.Renderer.Number.ToTarget(), effectType );
		
		effect.transform.position = GetEffectPosition(basePosition) + offset; //this.Renderer.interactionCharacter.Head.position + new Vector3(0, 0.5f, -3.0f);

		return LugusCoroutines.use.StartRoutine( LugusCoroutineUtil.DelayRoutine(effect.Duration()) );
	}

	public ILugusCoroutineHandle RotateTowardsCamera()
	{
		if( this.Renderer.Number.IsNumerator )
			return RotateTowards( LugusCamera.numerator.transform.position );
		else
			return RotateTowards( LugusCamera.denominator.transform.position );

	}

	public ILugusCoroutineHandle RotateTowards( NumberRenderer target )
	{
		Vector3 targetPos = GetOpponentPosition( target );
		return RotateTowards( targetPos );
	}

	public ILugusCoroutineHandle RotateTowards( Vector3 target )
	{
		ILugusCoroutineHandle output = null;

		foreach( CharacterRenderer c in Renderer.Characters )
		{
			if( _renderer.interactionCharacter == c )
				output = c.Animator.RotateTowards( target );
			else
				c.Animator.RotateTowards( target );
		}

		return output;
	}

	// TODO: make this a better interface (ex. duration = 0 on RotateTowards or sthing )
	public void RotateTowardsDirect( Vector3 target )
	{
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.RotateTowardsDirect( target );
		}
	}
	
	public ILugusCoroutineHandle RotateInDirection(Vector3 direction)
	{
		ILugusCoroutineHandle output = null;
		
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			if( _renderer.interactionCharacter == c )
				output = c.Animator.RotateInDirection( direction );
			else
				c.Animator.RotateInDirection( direction );
		}
		
		return output;
	}

	public ILugusCoroutineHandle RunTo( NumberRenderer target )
	{
		Vector3 targetPos = GetOpponentPosition( target );
		return RunTo( targetPos );
	}

	public ILugusCoroutineHandle RunTo( Vector3 target )
	{
		gameObject.MoveTo( target ).Time ( 2.0f ).Execute();

		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( FR.Animation.running, 0.05f );
		}
		
		return this.gameObject.StartLugusRoutine( RunToRoutine(2.0f) );
	}

	protected IEnumerator RunToRoutine( float duration )
	{
		yield return new WaitForSeconds( duration );
		
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( FR.Animation.idle, 0.05f );
		}
	}

	
	public void CrossFade( string animationName, float fadeDuration = 0.05f )
	{
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( animationName, fadeDuration );
		}
	}
	
	public void CrossFade( FRAnimationData animation, float fadeDuration = 0.05f )
	{
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( animation, fadeDuration );
		}
	}
	
	public void CrossFade( FR.Animation animation, float fadeDuration = 0.05f )
	{
		//Debug.Log ("CROSSFADE : " + this.transform.Path () + " -> " + animation);
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( animation, fadeDuration );
		}
	}


	
	public void SetupLocal()
	{

	}
	
	public void SetupGlobal()
	{

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