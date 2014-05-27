
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Ascend_flapWings : OperationVisualizerDivide_Ascend
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.flapWings;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 

	}

	public override float TimeToTransition()
	{
		return 3.0f;
	}

	public override IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer original, NumberRenderer second)
	{
		if( part.HasNumerator() )
		{
			Debug.LogError("Ascend_flapWings:VisualizeAnimationPart : shouldn't be called with Target part having Numerator!!!");
			yield break;
		}
		
		if( animationPartDelay > 0.0f )
			yield return new WaitForSeconds( animationPartDelay );

		
		original.Animator.CrossFade( this.AnimationType() );
		
		yield return new WaitForSeconds( 1.4985f );

		original.gameObject.MoveTo( original.transform.position + new Vector3(0,8,0)).Time (2.0f).Execute();


		
		yield return new WaitForSeconds(1.0f); // wait for 1 second with both offscreen to prevent clipping

		//second.transform.position = second.transform.position.yAdd( -3.7243f ); // shift a little down to compensate for root motion
		second.transform.position = second.transform.position.yAdd( 2.25f ); // shift a little down to compensate for root motion
		second.Animator.CrossFade( this.AnimationType() );


		yield return new WaitForSeconds( 0.8348f );

		
		// move original off screen down (through the black hole)
		//original.gameObject.MoveTo( original.transform.position.yAdd( -6.0f ) ).Time (0.5f).Execute();
		
		//second.Animator.CrossFade( FR.Animation.ironManFall );
		//second.gameObject.MoveTo( animationPartTarget ).Time (1.0f).Execute();
		
		yield return new WaitForSeconds(2.0f);


		// now, the character has shifted through root motion
		// but the NumberRenderer itself is still too low
		// shift numberRenderer to the correct position and move the character(S)s down again
		second.transform.position = second.interactionCharacter.transform.position;//animationPartTarget;
		second.interactionCharacter.transform.localPosition = Vector3.zero;

	}
}
