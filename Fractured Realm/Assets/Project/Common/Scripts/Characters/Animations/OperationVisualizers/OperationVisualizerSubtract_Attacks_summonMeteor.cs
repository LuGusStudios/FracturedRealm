using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_summonMeteor : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.summonMeteor;
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

        attacker.Animator.CrossFade(this.AnimationType(), 0.1f);

        yield return new WaitForSeconds(1f);

        LugusCamera.game.Shake(LugusCameraExtensions.ShakeAmount.SMALL, 3.2f);

        Prop meteor = PropFactory.use.CreateProp(FR.PropType.MeteorProp);
        meteor.transform.localScale = new Vector3(0, 0, 0);
        meteor.transform.position = new Vector3(Vector3.Lerp(attacker.transform.position, defender.transform.position, .5f).x, 13f, attacker.transform.position.z);

        meteor.gameObject.ScaleTo(new Vector3(1, 1, 1)).Time(1.1f).Execute();
        
        yield return new WaitForSeconds(2.2f);

        LugusCamera.game.Shake(LugusCameraExtensions.ShakeAmount.LARGE, .9f);

        meteor.gameObject.MoveTo(defender.interactionCharacter.RightToeEnd).EaseType(iTween.EaseType.linear).Time(.6f).Execute();

        yield return new WaitForSeconds(.1f);

        defender.Animator.CrossFade(FR.Animation.hitFront);

        LugusCamera.game.Shake(LugusCameraExtensions.ShakeAmount.HUGE);

        defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);

        PropFactory.use.FreeProp(meteor);
        
        yield return new WaitForSeconds(.3f);
	}
}
