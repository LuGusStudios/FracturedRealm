
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_highFiveRightHand : OperationVisualizerAdd_Starter
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.highFiveRightHand;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	public override float ApproximateDuration()
	{
		return 3.95f; // approx 3.9s long
	}

	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 
		
		_nextAnimations.Add(FR.Animation.highFiveLeftHand);
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
		//Debug.LogError("VisualizeAnimationPart : highFiveRightHand");

		// don't wait for both to turn. Receiver can start running as soon as he has turned
		// if we wait for both, this will look weird when used in combination with a multiply operation
		Starter.Animator.RotateTowards( Receiver );
		yield return Receiver.Animator.RotateTowards( Starter ).Coroutine;

		//Starter.Animator.RotateTowards( Starter );


		// Receiver should run towards starter
		// when almost arrived, he should stop, play highfive
		// the starter should at the same time start the LeftHand highfive to match

		Vector3 originalStarterPosition = Starter.interactionCharacter.transform.position;

		// we can't do Receiver.Animator.RunTo( Starter ) directly, because they would clip
		// so we need to run to right before the Starter
		// how to calculate this position? use direction, normalize it, and then we can just subtract it from the starterPos to move back towards the receiverPos
		Vector3 receiverPos = Starter.Animator.GetOpponentPosition( Receiver );
		Vector3 starterPos = Receiver.Animator.GetOpponentPosition( Starter );

		Vector3 direction = starterPos - receiverPos;
		direction.Normalize();

		starterPos -= (direction * 3.0f);

		yield return Receiver.Animator.RunTo( starterPos ).Coroutine;

		Receiver.Animator.CrossFade( FR.Animation.highFiveRightHand );
		Starter.Animator.CrossFade( FR.Animation.highFiveLeftHand );

		yield return new WaitForSeconds( 1.17f );

		Starter.Animator.SpawnEffect( FR.EffectType.JOIN_HIT, Starter.interactionCharacter.LeftHand.position, new Vector3(0.3f,0,0) );

		// need to let the highFive animation play out before shifting to idle afterwards
		// but we can't wait too long, because the hit effect would have faded off...
		// so: in betweener : wait long enough and then hard-shift the character to the right position (little jump, but shouldn't be noticeable)
		yield return new WaitForSeconds( 0.15f );
		Starter.Animator.CrossFade(FR.Animation.idle);
		yield return new WaitForSeconds(0.06f);
		Starter.interactionCharacter.transform.position = originalStarterPosition;
	}
}
