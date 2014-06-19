
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_throwAmericanFootballRightHand : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.throwAmericanFootballRightHand;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Attacker, NumberRenderer Defender )
	{
		yield return Attacker.Animator.RotateTowards( Defender ).Coroutine;

		Attacker.Animator.CrossFade( this.AnimationType() ); 
		
		yield return new WaitForSeconds(0.57f);

		Prop football = PropFactory.use.CreateProp( FR.PropType.Football );
		football.transform.parent = Attacker.interactionCharacter.RightHand;
		football.transform.localPosition = new Vector3(0.4113141f, 0.6171437f, -1.154315f);
		football.transform.localEulerAngles = new Vector3(318.2309f, 159.6911f, 14.3963f);

		yield return new WaitForSeconds(1.084f);

		// TODO: make football move in an arc + SPIN (iTween.MoveTo path?)

		football.transform.parent = null;
		football.transform.localEulerAngles = new Vector3(318.2309f, 159.6911f, 14.3963f);
		football.gameObject.MoveTo( Defender.interactionCharacter.Head ).Time( 0.5f ).Execute();

		yield return new WaitForSeconds( 0.4f );

		PropFactory.use.FreeProp( football );

		Defender.Animator.SpawnEffect( FR.EffectType.FIRE_HIT );
		
		yield return new WaitForSeconds(0.2f);
	}
}
