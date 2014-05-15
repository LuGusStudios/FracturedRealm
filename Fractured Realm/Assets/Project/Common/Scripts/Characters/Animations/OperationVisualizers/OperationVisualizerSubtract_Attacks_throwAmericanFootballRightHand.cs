
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
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 
		
		_nextAnimations.Add(FR.Animation.hitFront);
		_nextAnimations.Add(FR.Animation.hitSideLeft);
		_nextAnimations.Add(FR.Animation.hitSideRight);

	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}

	protected override IEnumerator VisualizeSingleAttack( FractionRenderer Attacker, FractionRenderer Defender )
	{
		yield return Attacker.Animator.RotateTowards( FR.Target.BOTH, Defender ).Coroutine;
		
		Attacker.Animator.CrossFade(FR.Target.NUMERATOR, this.AnimationType() ); 
		
		yield return new WaitForSeconds(0.57f);

		Prop football = PropFactory.use.CreateProp( FR.PropType.Football );
		football.transform.parent = Attacker.Numerator.interactionCharacter.RightHand;
		football.transform.localPosition = new Vector3(0.4113141f, 0.6171437f, -1.154315f);
		football.transform.localEulerAngles = new Vector3(318.2309f, 159.6911f, 14.3963f);

		yield return new WaitForSeconds(1.084f);

		football.transform.parent = null;
		football.transform.localEulerAngles = new Vector3(318.2309f, 159.6911f, 14.3963f);
		football.gameObject.MoveTo( Defender.Numerator.interactionCharacter.Head ).Time( 0.5f ).Execute();

		yield return new WaitForSeconds( 0.4f );

		PropFactory.use.FreeProp( football );
		
		Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
		hit.transform.position = Defender.Numerator.interactionCharacter.Head.transform.position + new Vector3(0,0.0f,-1.0f);
		
		yield return new WaitForSeconds(0.2f);
		
		
		Defender.Numerator.Number.Value -= 1;
		Defender.Numerator.NumberValueChanged(); // will automatically freeRenderer on Numerator on smallest on the last attack

		if( Defender.Numerator.Number.Value < 1 )
		{ 
			RendererFactory.use.FreeRenderer( Defender.Denominator );
		}

	}
}
