
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_mrWillieBam : OperationVisualizerAdd_Starter
{
    public override FR.Animation AnimationType()
    {
        return FR.Animation.mrWillieBam;
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

        starterPosition -= (direction * 4.2f);

        yield return receiver.Animator.RunTo(starterPosition).Coroutine;

        receiver.Animator.CrossFade(this.AnimationType());
        starter.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(0.72f);

        starter.Animator.SpawnEffect(FR.EffectType.JOIN_HIT, starter.interactionCharacter.RightHand.position, new Vector3(1.5f, 0, 0));

        yield return new WaitForSeconds(.15f);
        starter.Animator.CrossFade(FR.Animation.idle);
        yield return new WaitForSeconds(.06f);
        starter.interactionCharacter.transform.position = originalStarterPosition;
    }
}
