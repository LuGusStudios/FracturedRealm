using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_throwAmericanFootballLeftHand : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.throwAmericanFootballLeftHand;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
        return FR.VisualizerImplementationStatus.TESTING;
;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Attacker, NumberRenderer Defender )
	{
        yield return Attacker.Animator.RotateTowards(Defender).Coroutine;

        Attacker.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(0.57f); // hand is behind back

        Prop football = PropFactory.use.CreateProp(FR.PropType.Football);
        GameObject mover = new GameObject("mover");

        football.transform.parent = mover.transform;


        football.transform.position = Vector3.zero;
        football.transform.localEulerAngles = Vector3.zero;

        mover.transform.parent = Attacker.interactionCharacter.LeftHand;
        mover.transform.localPosition = new Vector3(-0.6165741f, -0.7046605f, 0.7056813f);
        mover.transform.localEulerAngles = new Vector3(61.15715f, 201.9581f, 23.36553f);





        yield return new WaitForSeconds(0.95f); // three steps back


        mover.transform.parent = null;

        football.transform.localEulerAngles = new Vector3(0, 90, 0);


        Vector3[] path = new Vector3[3];
        path[0] = Attacker.interactionCharacter.LeftHand.transform.position;
        path[2] = Defender.interactionCharacter.Head.transform.position;
        path[1] = Vector3.Lerp(path[0], path[2], 0.5f) + new Vector3(0, 1.8f, 0);

        mover.gameObject.MoveTo(path).MoveToPath(false).OrientToPath(true).Time(0.5f).Execute();
        football.gameObject.RotateTo(new Vector3(5 * 360, 0, 0)).Looptype(iTween.LoopType.loop).IsLocal(true).EaseType(iTween.EaseType.linear).Execute();

        yield return new WaitForSeconds(0.45f);

        PropFactory.use.FreeProp(football);
        GameObject.Destroy(mover);

        Defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);

        yield return new WaitForSeconds(0.2f); 
	}
}
