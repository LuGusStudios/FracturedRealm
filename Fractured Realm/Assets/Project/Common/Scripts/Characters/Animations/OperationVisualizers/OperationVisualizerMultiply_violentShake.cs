
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerMultiply_violentShake : OperationVisualizerMultiply
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.violentShake;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}
	
	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
		//yield return LugusCoroutines.use.StartRoutine( VisualizeAnimationPartDefault(part, Starter, Receiver, AnimationType(), 1.14f) ).Coroutine;
		return VisualizeAnimationPartDefault(part, Starter, Receiver, AnimationType(), 1.14f);
		
	}
}
