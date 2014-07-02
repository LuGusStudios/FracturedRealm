
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Ascend_lotus : OperationVisualizerDivide_Ascend
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.lotus;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

    public override float TimeToTransition()
    {
        return 3.0f;
    }

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer original, NumberRenderer second )
	{
        if (part.HasNumerator())
        {
            Debug.LogError("Ascend_lotus:VisualizeAnimationPart: shouldn't be called with Target part having Numerator!");
            yield break;
        }

        if (animationPartDelay > 0.0f)
            yield return new WaitForSeconds(animationPartDelay);

        original.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(2f);

        BlackHole blackHole = this.blackHoleController.RequestBlackHole(animationPartTarget);

        original.gameObject.MoveTo(original.transform.position + new Vector3(0, 8, 0)).Time(1.8f).EaseType(iTween.EaseType.easeInSine).Execute();

        yield return new WaitForSeconds(2f);


        // we can use the same lotus animation to come out of the hole
        // it will crossfade to an animation to start the multiply, that animation is always on the ground. The root position
        // of the mutliply animation will bring the character back down, this looks pretty OK actually
        second.Animator.CrossFade(this.AnimationType()); 
        second.gameObject.MoveTo(second.transform.position + new Vector3(0, 6, 0)).EaseType(iTween.EaseType.easeOutSine).Time(1f).Execute();
	}
}
