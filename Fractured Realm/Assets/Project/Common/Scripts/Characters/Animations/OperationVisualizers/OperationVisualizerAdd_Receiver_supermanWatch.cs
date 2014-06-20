
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Receiver_supermanWatch : OperationVisualizerAdd_Receiver
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.supermanWatch;
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
