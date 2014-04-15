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

	public Coroutine MoveTo( FR.Target selector, FractionRenderer target )
	{
		Coroutine output = null;

		Vector3 targetNumeratorPos, targetDenominatorPos;
		GetOpponentPositions( target, out targetNumeratorPos, out targetDenominatorPos );

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			output = _renderer.Fraction.Numerator.Renderer.Animator.MoveTo( targetNumeratorPos );
		}

		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			if( output == null )
				output = _renderer.Fraction.Denominator.Renderer.Animator.MoveTo( targetDenominatorPos );
			else
				_renderer.Fraction.Denominator.Renderer.Animator.MoveTo( targetDenominatorPos );
		}

		return output;
	}

	public Coroutine RotateTowards(FR.Target selector,  FractionRenderer target )
	{
		Coroutine output = null;
		
		Vector3 targetNumeratorPos, targetDenominatorPos;
		GetOpponentPositions( target, out targetNumeratorPos, out targetDenominatorPos );

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			output = _renderer.Fraction.Numerator.Renderer.Animator.RotateTowards( targetNumeratorPos );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			if( output == null )
				output = _renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( targetDenominatorPos );
			else
				_renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( targetDenominatorPos );
		}
		
		return output;
	}
	
	public Coroutine RotateTowards( FR.Target selector, Vector3 target )
	{
		Coroutine output = null;
		
		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			output = _renderer.Fraction.Numerator.Renderer.Animator.RotateTowards( target );
		}
		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			if( output == null )
				output = _renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( target );
			else
				_renderer.Fraction.Denominator.Renderer.Animator.RotateTowards( target );
		}
		
		return output;
	}

	public Coroutine RotateTowardsCamera()
	{
		RotateTowards( FR.Target.NUMERATOR, LugusCamera.numerator.transform.position );
		return RotateTowards( FR.Target.DENOMINATOR, LugusCamera.denominator.transform.position );
	}

	public Coroutine SpawnEffect( FR.Target selector, FR.EffectType effectType )
	{
		
		Effect[] hits = EffectFactory.use.CreateEffects( selector, effectType );

		float duration = float.MinValue;
		float durationTemp = 0;

		if( _renderer.Fraction.Numerator.Value != 0 && selector.HasNumerator() )
		{
			hits[0].transform.position = _renderer.Numerator.transform.position + new Vector3(0, 0.5f, -1.0f);

			durationTemp = hits[0].Duration();
			if( durationTemp > duration )
				duration = durationTemp;
		}

		
		if( _renderer.Fraction.Denominator.Value != 0 && selector.HasDenominator() )
		{
			hits[1].transform.position = _renderer.Denominator.transform.position + new Vector3(0, 0.5f, -1.0f);
			
			durationTemp = hits[1].Duration();
			if( durationTemp > duration )
				duration = durationTemp;
		}

		ILugusCoroutineHandle handle = LugusCoroutines.use.GetHandle();
		return handle.StartRoutine( WaitForSecondsRoutine(duration) );
	}

	protected IEnumerator WaitForSecondsRoutine( float duration )
	{
		if( duration > 0 )
		{
			yield return new WaitForSeconds( duration );
		}

		yield break;
	}

}
