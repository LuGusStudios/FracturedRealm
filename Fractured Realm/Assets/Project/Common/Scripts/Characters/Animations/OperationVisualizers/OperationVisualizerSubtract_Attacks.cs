
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks : OperationVisualizerSubtract
{
	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 
		
		_nextAnimations.Add(FR.Animation.hitFront);
		_nextAnimations.Add(FR.Animation.hitSideLeft);
		_nextAnimations.Add(FR.Animation.hitSideRight);
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}
}

