
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_lowFiveSlapRightHand : OperationVisualizerAdd_Starter
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.lowFiveSlapRightHand;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

    public override IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer starter, NumberRenderer receiver)
	{
        starter.Animator.RotateTowards(receiver);
        yield return receiver.Animator.RotateTowards(starter).Coroutine;

        Vector3 originalStarterPosition = starter.interactionCharacter.transform.position;

        Vector3 receiverPosition = starter.Animator.GetOpponentPosition(receiver);
        Vector3 starterPosition = receiver.Animator.GetOpponentPosition(starter);

        Vector3 direction = starterPosition - receiverPosition;
        direction.Normalize();

        starterPosition -= (direction * 4.02f);

        yield return receiver.Animator.RunTo(starterPosition).Coroutine;

        receiver.Animator.CrossFade(FR.Animation.lowFiveSlapRightHand);
        yield return new WaitForSeconds(.03f);
        starter.Animator.CrossFade(FR.Animation.lowFiveReceiveLefttHand);

        yield return new WaitForSeconds(1.17f);

        starter.Animator.SpawnEffect(FR.EffectType.JOIN_HIT, starter.interactionCharacter.RightHand.position, new Vector3(0.3f, 0, 0));

        yield return new WaitForSeconds(0.15f);
        starter.Animator.CrossFade(FR.Animation.idle);
        yield return new WaitForSeconds(0.06f);
        starter.interactionCharacter.transform.position = originalStarterPosition;
	}
}
