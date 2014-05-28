
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerAdd_Starter : OperationVisualizerAdd
{
	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 

		_nextAnimations.Add(FR.Animation.lowFiveReceiveLefttHand);
		_nextAnimations.Add(FR.Animation.lowFiveReceiveRightHand);
		_nextAnimations.Add(FR.Animation.magnetAttractedFront);
		_nextAnimations.Add(FR.Animation.magnetAttractedSideLeft);
		_nextAnimations.Add(FR.Animation.magnetAttractedSideRight);
		_nextAnimations.Add(FR.Animation.supermanWatch);
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}
}

