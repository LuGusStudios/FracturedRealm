
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Descend : OperationVisualizerDivide
{
	protected override void PrepareNextAnimations() 
	{ 
		_nextAnimations = new List<FR.Animation>(); 
		
		_nextAnimations.Add(FR.Animation.flapWings);
		_nextAnimations.Add(FR.Animation.jetFart);
		_nextAnimations.Add(FR.Animation.lotus);
		_nextAnimations.Add(FR.Animation.tractorBeam);
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		return base.Visualize( current, target );
	}

}

