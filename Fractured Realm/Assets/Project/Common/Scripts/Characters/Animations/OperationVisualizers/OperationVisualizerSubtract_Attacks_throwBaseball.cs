
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_throwBaseball : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.throwBaseball;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer attacker, NumberRenderer defender )
	{
        yield return attacker.Animator.RotateTowards(defender).Coroutine;

        attacker.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(.61f);

        Prop baseball = PropFactory.use.CreateProp(FR.PropType.BaseBall);
        baseball.transform.parent = attacker.interactionCharacter.LeftHand;
        baseball.transform.localPosition = new Vector3(-0.3184976f, -0.5560089f, 0.3467883f);
        baseball.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        yield return new WaitForSeconds(.1f);

        baseball.gameObject.MoveTo(defender.interactionCharacter.Body).Time(.25f).Execute();

        yield return new WaitForSeconds(.2f);

        PropFactory.use.FreeProp(baseball);

        defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);

        yield return new WaitForSeconds(.3f);
	}
}
