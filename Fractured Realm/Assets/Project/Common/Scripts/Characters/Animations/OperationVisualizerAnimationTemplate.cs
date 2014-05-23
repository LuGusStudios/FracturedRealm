/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerOPERATIONSTAGEANIMATION : OperationVisualizerOPERATIONSTAGE
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.TYPE;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.NONE;
	}

	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 
		NEXTANIMATION
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}
}
*/