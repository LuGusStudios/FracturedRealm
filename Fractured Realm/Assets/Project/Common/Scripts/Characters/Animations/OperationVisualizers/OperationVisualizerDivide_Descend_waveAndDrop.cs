
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Descend_waveAndDrop : OperationVisualizerDivide_Descend
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.waveAndDrop;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	public override float TimeToTransition()
	{
		return 3.89436f;
	}


	public override IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer original, NumberRenderer second)
	{
		if( part.HasDenominator() )
		{
			Debug.LogError("Descend_waveAndDrop:VisualizeAnimationPart : shouldn't be called with Target part having Denominator!!!");
			yield break;
		}


		if( animationPartDelay > 0.0f )
			yield return new WaitForSeconds( animationPartDelay );

		BlackHole hole = this.blackHoleController.RequestBlackHole(original.interactionCharacter.transform.position);

		yield return new WaitForSeconds(0.1f);

		original.Animator.CrossFade( this.AnimationType() );

		yield return new WaitForSeconds( TimeToTransition() );

		// move original off screen down (through the black hole)
		original.gameObject.MoveTo( original.transform.position.yAdd( -6.0f ) ).Time (0.5f).Execute();
		
		yield return new WaitForSeconds(1.0f);

		hole.Free();

		second.Animator.CrossFade( FR.Animation.ironManFall );
		second.gameObject.MoveTo( animationPartTarget ).Time (0.45f).Execute();
		
		yield return new WaitForSeconds(1.6f);
	}

}





