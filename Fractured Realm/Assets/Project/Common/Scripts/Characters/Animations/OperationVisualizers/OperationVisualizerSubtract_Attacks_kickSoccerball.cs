
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_kickSoccerball : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.kickSoccerball;
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

        yield return new WaitForSeconds(.8f);

        Prop soccerball = PropFactory.use.CreateProp(FR.PropType.Soccerball);
        soccerball.transform.parent = null;

        Vector3 offset;

        if (attacker.transform.position.x > defender.transform.position.x)
            offset = attacker.interactionCharacter.RightToeEnd.position + new Vector3(-0.8f, 0.32f, 0);
        else
            offset = attacker.interactionCharacter.RightToeEnd.position + new Vector3(0.8f, 0.32f, 0);

        soccerball.transform.position = offset;

        yield return new WaitForSeconds(1.9f); 

        soccerball.gameObject.MoveTo(defender.interactionCharacter.Head.position + new Vector3(0, .3f, 0)).Time(0.2f).Execute();

        yield return new WaitForSeconds(.15f);

        PropFactory.use.FreeProp(soccerball);
        defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);

        yield return new WaitForSeconds(.2f);
	}
}
