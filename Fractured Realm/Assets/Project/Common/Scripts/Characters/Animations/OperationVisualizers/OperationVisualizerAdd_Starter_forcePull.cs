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

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer starter, NumberRenderer receiver )
	{
        // TODO: apparently I never made a receiver animation for this

        yield return starter.Animator.RotateTowards(receiver).Coroutine;


        starter.Animator.CrossFade(this.AnimationType());

        // TODO: uncomment this line when the animation tree is rebuilt. 
        //starter.Animator.CrossFade(FR.Animation.forcePullReceive);   

        yield return new WaitForSeconds(.578f);


        receiver.gameObject.MoveTo(receiver.transform.position + new Vector3(0, 2,0)).Time(1f).EaseType(iTween.EaseType.easeOutBounce).Execute();

        yield return new WaitForSeconds(1.6f);

        foreach (CharacterRenderer characterRenderer in receiver.Characters)
        {
            characterRenderer.gameObject.MoveTo(starter.interactionCharacter.Body).Time(0.3f).Execute();
        }

        yield return new WaitForSeconds(.33f);


        starter.Animator.SpawnEffect(FR.EffectType.JOIN_HIT);


	}
}
