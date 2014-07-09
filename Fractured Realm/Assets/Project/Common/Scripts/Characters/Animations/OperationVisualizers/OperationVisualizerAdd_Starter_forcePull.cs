using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_forcePull : OperationVisualizerAdd_Starter
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.forcePull;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

    protected override void PrepareNextAnimations()
    {
        _nextAnimations = new List<FR.Animation>();

        _nextAnimations.Add(FR.Animation.magnetAttractedSideLeft);
        _nextAnimations.Add(FR.Animation.magnetAttractedSideRight);
    }

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
        // TODO: apparently I never made a receiver animation for this

        yield return Starter.Animator.RotateTowards(Receiver).Coroutine;



        Starter.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(.578f);

        //if (Starter.transform.position.x < Receiver.transform.position.x)
        //    // play right side animation
        //else
        //    // play left side animation

        Receiver.gameObject.MoveTo(Receiver.transform.position + new Vector3(0, 2,0)).Time(1f).EaseType(iTween.EaseType.easeOutBounce).Execute();

        yield return new WaitForSeconds(1.6f);

        foreach (CharacterRenderer characterRenderer in Receiver.Characters)
        {
            characterRenderer.gameObject.MoveTo(Starter.interactionCharacter.Body).Time(0.3f).Execute();
        }

        yield return new WaitForSeconds(.33f);


        Starter.Animator.SpawnEffect(FR.EffectType.JOIN_HIT);


	}
}
