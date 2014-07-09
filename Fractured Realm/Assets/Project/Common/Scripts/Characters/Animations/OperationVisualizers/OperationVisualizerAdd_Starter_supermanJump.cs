
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_supermanJump : OperationVisualizerAdd_Starter
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.supermanJump;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer starter, NumberRenderer receiver )
	{
        // TODO: this animation should blend back to idle, so I manually removed the connection to idle.
        // If the animation tree ever gets rebuilt, this needs to be done again.

        starter.Animator.RotateTowards(receiver);
        receiver.Animator.RotateTowards(starter);
        yield return receiver.Animator.RotateTowards(starter).Coroutine;

        starter.Animator.CrossFade(this.AnimationType());
        receiver.Animator.CrossFade(FR.Animation.supermanWatch);

        yield return new WaitForSeconds(1.8f);

        Vector3 halfwayPoint = Vector3.Lerp(starter.transform.position, receiver.transform.position, .5f);
        starter.gameObject.MoveTo(halfwayPoint.yAdd(10f)).Time(1.02f).EaseType(iTween.EaseType.linear).Execute();

        yield return new WaitForSeconds(1.6f);

        Debug.Log("going back down");

        starter.gameObject.MoveTo(receiver.interactionCharacter.Body).Time(.5f).EaseType(iTween.EaseType.linear).Execute();

        receiver.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);

        yield return new WaitForSeconds(.2f);
	}
}
