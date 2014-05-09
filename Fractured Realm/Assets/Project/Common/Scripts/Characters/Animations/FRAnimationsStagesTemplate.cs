/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class FRAnimations : LugusSingletonRuntime<FRAnimations>
{
	protected Dictionary<FR.OperationType, List<FR.Animation>> _operationAnimations = null;
	public Dictionary<FR.OperationType, List<FR.Animation>> OperationAnimations
	{
		get
		{
			if( _operationAnimations == null || _operationAnimations.Count == 0 )
			{
				FillOperationAnimations();
			}

			return _operationAnimations;
		}
	}


	public FRAnimationData GetRandomStarterAnimation(FR.OperationType operation)
	{
		List<FR.Animation> concreteAnimations = OperationAnimations[ operation ];
		if( concreteAnimations == null || concreteAnimations.Count == 0 )
		{
			Debug.LogError("FRAnimations:GetRandomStarterAnimation : no animations found for operation " + operation);
			return null;
		}

		FR.Animation animation = concreteAnimations[ UnityEngine.Random.Range(0, concreteAnimations.Count) ];
		return animations[ (int) animation ];
	}

	public void FillOperationAnimations()
	{
		INIT
	}
}
*/


