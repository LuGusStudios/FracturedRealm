
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSimplify_starFishJump : OperationVisualizerSimplify
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.starFishJump;
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
        yield return new WaitForSeconds(1.5f);
	}
}
