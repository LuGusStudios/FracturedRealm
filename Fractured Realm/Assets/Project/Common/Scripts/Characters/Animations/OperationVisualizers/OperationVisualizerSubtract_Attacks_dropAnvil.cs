
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_dropAnvil : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.dropAnvil;
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
        //TODO: Fix the animation FBX

        

        yield return attacker.Animator.RotateTowards(defender).Coroutine;

        attacker.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(1.3f);

        Prop anvil = PropFactory.use.CreateProp(FR.PropType.Anvil);
        anvil.transform.parent = null;
        anvil.transform.position = defender.interactionCharacter.Head.position + new Vector3(0, 4f, 0);
        anvil.transform.localScale = Vector3.zero;

        anvil.gameObject.ScaleTo(new Vector3(0.321f, 0.321f, 0.321f)).Time(.3f).EaseType(iTween.EaseType.easeOutBounce).Execute();

        yield return new WaitForSeconds(1.5f);

        anvil.gameObject.MoveTo(defender.interactionCharacter.RightToeEnd.position.yAdd(-.1f)).Time(.28f).Execute();

        yield return new WaitForSeconds(0.02f);

        PropFactory.use.FreeProp(anvil);

        defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);
        LugusCamera.game.Shake(LugusCameraExtensions.ShakeAmount.HUGE, .75f);

        yield return new WaitForSeconds(.2f);
	}
}
