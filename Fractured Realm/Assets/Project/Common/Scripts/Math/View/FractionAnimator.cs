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

	protected void GetOpponentPositions( FractionRenderer target, out Vector3 numeratorPos, out Vector3 denominatorPos )
	{
		numeratorPos = Vector3.zero;
		denominatorPos = numeratorPos;

		if( target.Fraction.Numerator.Value != 0 )
			numeratorPos = target.Fraction.Numerator.Renderer.transform.position;
		
		if( target.Fraction.Denominator.Value != 0 )
			denominatorPos = target.Fraction.Denominator.Renderer.transform.position;
		
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

	public ILugusCoroutineHandle MoveTo( FR.Target selector, FractionRenderer target )
	{
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();

		Vector3 targetNumeratorPos, targetDenominatorPos;
		GetOpponentPositions( target, out targetNumeratorPos, out targetDenominatorPos );

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			waiter.Add ( _renderer.Fraction.Numerator.Renderer.Animator.MoveTo( targetNumeratorPos ) );
		}

		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			waiter.Add ( _renderer.Fraction.Denominator.Renderer.Animator.MoveTo( targetDenominatorPos ) );
		}

		return waiter.Start();
	}

	public ILugusCoroutineHandle RotateTowards(FR.Target selector,  FractionRenderer target )
	{
		//ILugusCoroutineHandle output = null;
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();
		
		Vector3 targetNumeratorPos, targetDenominatorPos;
		GetOpponentPositions( target, out targetNumeratorPos, out targetDenominatorPos );

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			waiter.Add ( _renderer.Fraction.Numerator.Renderer.Animator.RotateTowards( targetNumeratorPos ) );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{

			waiter.Add ( _renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( targetDenominatorPos ) );
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
		// if no numerator or denominator present, we don't need to spawn a effect for them
		if( _renderer.Fraction.Numerator.Value == 0 && selector.HasNumerator() )
		{
			selector = selector & ~FR.Target.NUMERATOR;
		}

		if( _renderer.Fraction.Denominator.Value == 0 && selector.HasDenominator() )
		{
			selector = selector & ~FR.Target.DENOMINATOR;
		}


		Effect[] hits = EffectFactory.use.CreateEffects( selector, effectType );

		float duration = float.MinValue;
		float durationTemp = 0;

		if( selector.HasNumerator() )
		{
			hits[0].transform.position = _renderer.Numerator.transform.position + new Vector3(0, 0.5f, -1.0f);

			durationTemp = hits[0].Duration();
			if( durationTemp > duration )
				duration = durationTemp;
		}

		
		if( selector.HasDenominator() )
		{
			hits[1].transform.position = _renderer.Denominator.transform.position + new Vector3(0, 0.5f, -1.0f);
			
			durationTemp = hits[1].Duration();
			if( durationTemp > duration )
				duration = durationTemp;
		}


		return LugusCoroutines.use.StartRoutine( LugusCoroutineUtil.DelayRoutine(duration) );
	}

}
