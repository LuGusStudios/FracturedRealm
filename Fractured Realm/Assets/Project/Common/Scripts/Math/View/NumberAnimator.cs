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


	public ILugusCoroutineHandle MoveTo( Vector3 target )
	{
		gameObject.MoveTo( target ).Time ( 2.0f ).Execute();

		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( FR.Animation.running, 0.05f );
		}
		
		return this.gameObject.StartLugusRoutine( MoveToRoutine(2.0f) );
	}

	protected IEnumerator MoveToRoutine( float duration )
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
		foreach( CharacterRenderer c in Renderer.Characters )
		{
			c.Animator.CrossFade( animation, fadeDuration );
		}
	}

	public ILugusCoroutineHandle SpawnEffect( FR.EffectType effectType )
	{
		FR.Target selector = FR.Target.NONE;
		if( this.Renderer.Number.IsNumerator )
			selector = FR.Target.NUMERATOR;
		else
			selector = FR.Target.DENOMINATOR;

		Effect[] effects = EffectFactory.use.CreateEffects( selector, effectType );

		Effect effect = null;
		if( selector == FR.Target.NUMERATOR )
			effect = effects[0];
		else
			effect = effects[1];
		
		float duration = float.MinValue;
		float durationTemp = 0;
		
		effect.transform.position = this.Renderer.interactionCharacter.Head.position + new Vector3(0, 0.5f, -3.0f);

		return LugusCoroutines.use.StartRoutine( LugusCoroutineUtil.DelayRoutine( effect.Duration() ) );
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