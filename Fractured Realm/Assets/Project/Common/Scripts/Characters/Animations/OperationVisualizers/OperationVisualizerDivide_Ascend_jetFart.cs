
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Ascend_jetFart : OperationVisualizerDivide_Ascend
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.jetFart;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override float TimeToTransition()
	{
		return 5.0f;
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer original, NumberRenderer second )
	{
		if( part.HasNumerator() )
		{
			Debug.LogError("Ascend_jetFart:VisualizeAnimationPart : shouldn't be called with Target part having Numerator!!!");
			yield break;
		}
		
		if( animationPartDelay > 0.0f )
			yield return new WaitForSeconds( animationPartDelay );
		
		
		original.Animator.CrossFade( this.AnimationType() );

		// TODO: add lighter prop
		// TODO: add fire particles

		yield return new WaitForSeconds( 3.5f );

		BlackHole hole = this.blackHoleController.RequestBlackHole(animationPartTarget);


		original.gameObject.MoveTo( original.transform.position + new Vector3(0,8,0)).Time (2.0f).Execute();

		//original.gameObject.MoveTo( original.transform.position + new Vector3(0,8,0)).Time (2.0f).Execute();
		
		
		
		yield return new WaitForSeconds(2.0f); // wait for some time before rising on the numerator side to prevent clipping

		// TODO: add proper "landing" animation
		 
		//second.transform.position = second.transform.position.yAdd( -3.7243f ); // shift a little down to compensate for root motion
		second.Animator.CrossFade( FR.Animation.idle );

		second.gameObject.MoveTo( animationPartTarget ).Time(1.5f).Execute ();
		
		
		yield return new WaitForSeconds( 1.8f );

		hole.Free();
		
		// move original off screen down (through the black hole)
		//original.gameObject.MoveTo( original.transform.position.yAdd( -6.0f ) ).Time (0.5f).Execute();
		
		//second.Animator.CrossFade( FR.Animation.ironManFall );
		//second.gameObject.MoveTo( animationPartTarget ).Time (1.0f).Execute();
		
		yield return new WaitForSeconds(1.7f);
		
		
		// now, the character has shifted through root motion
		// but the NumberRenderer itself is still too low
		// shift numberRenderer to the correct position and move the character(S)s down again
	}
}
