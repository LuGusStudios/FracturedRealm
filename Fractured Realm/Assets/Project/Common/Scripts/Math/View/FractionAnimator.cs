using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FractionAnimator
{
	protected FractionRenderer _renderer = null;
	public FractionRenderer Renderer
	{
		get{ return _renderer; }
		set{ _renderer = value; }
	}

	/*
	protected void GetOpponentPositions( FractionRenderer target, out Vector3 numeratorPos, out Vector3 denominatorPos )
	{
		numeratorPos = Vector3.zero;
		denominatorPos = numeratorPos;

		if( target.Fraction.Numerator.Value != 0 )
			numeratorPos = target.Fraction.Numerator.Renderer.interactionCharacter.Body.transform.position.y( target.Fraction.Numerator.Renderer.transform.position.y );
		
		if( target.Fraction.Denominator.Value != 0 )
			denominatorPos = target.Fraction.Denominator.Renderer.interactionCharacter.Body.transform.position.y( target.Fraction.Denominator.Renderer.transform.position.y );
		
		if(_renderer.Fraction.Numerator.Value != 0 && target.Fraction.Numerator.Value == 0 )
		{
			Debug.LogWarning("FractionAnimator:GetOpponentPositions : target numerator is 0. Aiming for denominator.");
			numeratorPos = denominatorPos;
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && target.Fraction.Denominator.Value == 0 )
		{
			Debug.LogWarning("FractionAnimator:GetOpponentPositions : target denominator is 0. Aiming for numerator.");
			denominatorPos = numeratorPos;
		}
	}
	*/

	public ILugusCoroutineHandle RunTo( FR.Target selector, FractionRenderer target )
	{
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();

		//Vector3 targetNumeratorPos, targetDenominatorPos;
		//GetOpponentPositions( target, out targetNumeratorPos, out targetDenominatorPos );

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			waiter.Add ( _renderer.Fraction.Numerator.Renderer.Animator.RunTo( target.Numerator ) ); //targetNumeratorPos ) );
		}

		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			waiter.Add ( _renderer.Fraction.Denominator.Renderer.Animator.RunTo( target.Denominator ) ); //targetDenominatorPos ) );
		}

		return waiter.Start();
	}

	public ILugusCoroutineHandle RotateTowards(FR.Target selector,  FractionRenderer target )
	{
		//ILugusCoroutineHandle output = null;
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();
		
		//Vector3 targetNumeratorPos, targetDenominatorPos;
		//GetOpponentPositions( target, out targetNumeratorPos, out targetDenominatorPos );

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			waiter.Add ( _renderer.Fraction.Numerator.Renderer.Animator.RotateTowards( target.Numerator ) );//targetNumeratorPos ) );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{

			waiter.Add ( _renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( target.Denominator ) ); // targetDenominatorPos ) );
		}
		
		return waiter.Start();
	}
	
	public ILugusCoroutineHandle RotateTowards( FR.Target selector, Vector3 target )
	{
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();
		
		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			waiter.Add ( _renderer.Fraction.Numerator.Renderer.Animator.RotateTowards( target ) );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			waiter.Add ( _renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( target ) );
		}
		
		return waiter.Start();
	}

	public ILugusCoroutineHandle RotateTowardsCamera()
	{
		return LugusCoroutineUtil.WaitForFinish( 
                	RotateTowards( FR.Target.NUMERATOR, LugusCamera.numerator.transform.position ),
                	RotateTowards( FR.Target.DENOMINATOR, LugusCamera.denominator.transform.position) 
               );
	}

	public ILugusCoroutineHandle SpawnEffect( FR.Target selector, FR.EffectType effectType )
	{

		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();
		
		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			waiter.Add ( _renderer.Fraction.Numerator.Renderer.Animator.SpawnEffect(effectType) );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			waiter.Add ( _renderer.Fraction.Denominator.Renderer.Animator.SpawnEffect(effectType) );
		}
		
		return waiter.Start();


		/*
		// if no numerator or denominator present, we don't need to spawn a effect for them
		if( _renderer.Fraction.Numerator.Value == 0 && selector.HasNumerator() )
		{
			selector = selector & ~FR.Target.NUMERATOR;
		}

		if( _renderer.Fraction.Denominator.Value == 0 && selector.HasDenominator() )
		{
			selector = selector & ~FR.Target.DENOMINATOR;
		}


		Effect[] effects = EffectFactory.use.CreateEffects( selector, effectType );

		float duration = float.MinValue;
		float durationTemp = 0;

		if( selector.HasNumerator() )
		{
			effects[0].transform.position = _renderer.Numerator.interactionCharacter.Head.position + new Vector3(0, 0.5f, -3.0f);

			durationTemp = effects[0].Duration();
			if( durationTemp > duration )
				duration = durationTemp;
		}

		
		if( selector.HasDenominator() )
		{
			effects[1].transform.position = _renderer.Denominator.interactionCharacter.Head.position + new Vector3(0, 0.5f, -3.0f);
			
			durationTemp = effects[1].Duration();
			if( durationTemp > duration )
				duration = durationTemp;
		}


		return LugusCoroutines.use.StartRoutine( LugusCoroutineUtil.DelayRoutine(duration) );
		*/
	}

	
	
	public void CrossFade( FR.Target selector, string animationName, float fadeDuration = 0.05f )
	{
		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			_renderer.Numerator.Animator.CrossFade( animationName, fadeDuration );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			_renderer.Denominator.Animator.CrossFade( animationName, fadeDuration );
		}
	}
	
	public void CrossFade( FR.Target selector, FRAnimationData animation, float fadeDuration = 0.05f )
	{
		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			_renderer.Numerator.Animator.CrossFade( animation, fadeDuration );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			_renderer.Denominator.Animator.CrossFade( animation, fadeDuration );
		}
	}
	
	public void CrossFade( FR.Target selector, FR.Animation animation, float fadeDuration = 0.05f )
	{
		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			_renderer.Numerator.Animator.CrossFade( animation, fadeDuration );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			_renderer.Denominator.Animator.CrossFade( animation, fadeDuration );
		}
	}

}
