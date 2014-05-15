
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_magnetAtrractRight : OperationVisualizerAdd_Starter
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.magnetAtrractRight;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 
		
		_nextAnimations.Add(FR.Animation.lowFiveReceiveLefttHand);
		_nextAnimations.Add(FR.Animation.lowFiveReceiveRightHand);
		_nextAnimations.Add(FR.Animation.magnetAttractedFront);
		_nextAnimations.Add(FR.Animation.magnetAttractedSideLeft);
		_nextAnimations.Add(FR.Animation.magnetAttractedSideRight);
		_nextAnimations.Add(FR.Animation.supermanWatch);

	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}

	public override IEnumerator VisualizeAnimation(FractionRenderer Starter, FractionRenderer Receiver)
	{	
		// starter turns to receiver
		// starter uses magnet
		// receiver is pulled (animation)
		// receiver is moved towards starter (itween)
		// collision with particles

		yield return Starter.Animator.RotateTowards( FR.Target.BOTH, Receiver ).Coroutine;

		Starter.Animator.CrossFade( FR.Target.BOTH, AnimationType() );

		yield return new WaitForSeconds( 0.5f );
		Receiver.Animator.CrossFade( FR.Target.BOTH, FR.Animation.magnetAttractedSideRight ); // this animation only really starts at 1.0f
		
		yield return new WaitForSeconds( 0.5f );

		
		yield return new WaitForSeconds( 0.8f );

		List<CharacterRenderer> characters = Receiver.GetCharacters(FR.Target.BOTH);
		foreach( CharacterRenderer rend in characters )
		{
			if( rend.Number.IsNumerator )
				rend.gameObject.MoveTo( Starter.Numerator.interactionCharacter.Body ).Time (0.5f).Execute();
			else
				rend.gameObject.MoveTo( Starter.Denominator.interactionCharacter.Body ).Time (0.5f).Execute();

		}

		yield return new WaitForSeconds(0.42f);
		
		Starter.Animator.SpawnEffect( FR.Target.BOTH, FR.EffectType.JOIN_HIT );
		
		// wait untill the height of the hit effect (covering all)
		yield return new WaitForSeconds(0.2f);
	}
}
