
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter_magnetAtrractRight : OperationVisualizerAdd_Starter
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.magnetAtrractRight;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 

		_nextAnimations.Add(FR.Animation.magnetAttractedSideLeft);
		_nextAnimations.Add(FR.Animation.magnetAttractedSideRight);
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
		yield return Starter.Animator.RotateTowards( Receiver ).Coroutine;

		Starter.Animator.CrossFade( FR.Animation.magnetAtrractRight );
		
		yield return new WaitForSeconds( 0.54f );

		Prop magnet = PropFactory.use.CreateProp( FR.PropType.Magnet ); 
		magnet.transform.parent = Starter.interactionCharacter.RightHand;
		magnet.transform.localPosition = new Vector3(0.4113141f, 0.6171437f, -1.154315f);
		magnet.transform.localEulerAngles = new Vector3(318.2309f, 159.6911f, 14.3963f);

		if( Starter.transform.position.x < Receiver.transform.position.x ) // starter is to the LEFT of the receiver on screen
			Receiver.Animator.CrossFade( FR.Animation.magnetAttractedSideRight ); // this animation only really starts at 1.0f 
		else // starter is to the RIGHT of the receiver on screen
			Receiver.Animator.CrossFade( FR.Animation.magnetAttractedSideLeft ); // this animation only really starts at 1.0f
		
		yield return new WaitForSeconds( 0.5f ); 
		
		
		yield return new WaitForSeconds( 0.8f );

		// DONT DO THIS : Receiver.Animator.MoveTo( Starter ); // this also triggers running animation, which we don't want here


		foreach( CharacterRenderer rend in Receiver.Characters )
		{
			rend.gameObject.MoveTo( Starter.interactionCharacter.Body ).Time (0.5f).Execute();
		} 

		
		// poof effect should come when we hit the magnet
		yield return new WaitForSeconds(0.33f);
		
		Starter.Animator.SpawnEffect( FR.EffectType.JOIN_HIT );
	}
}
