
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Descend_fishingPulledIn : OperationVisualizerDivide_Descend
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.fishingPulledIn;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.NONE;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Attacker, NumberRenderer Defender )
	{
		return base.VisualizeAnimationPart( part, Attacker, Defender );
	}
}
