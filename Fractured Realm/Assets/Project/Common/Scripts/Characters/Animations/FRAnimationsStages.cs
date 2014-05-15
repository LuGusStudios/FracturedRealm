
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
		_operationAnimations = new Dictionary<FR.OperationType, List<FR.Animation>>();
		
		
		_operationAnimations[ FR.OperationType.ADD ] = new List<FR.Animation>();
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.chestBump );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.forcePull );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.freshPrince );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.highFiveLeftHand );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.highFiveRightHand );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.lowFiveSlapLeftHand );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.lowFiveSlapRightHand );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.magnetAtrractLeft );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.magnetAtrractRight );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.mrWillieBam );
		_operationAnimations[ FR.OperationType.ADD ].Add( FR.Animation.supermanJump );

		_operationAnimations[ FR.OperationType.DIVIDE ] = new List<FR.Animation>();
		_operationAnimations[ FR.OperationType.DIVIDE ].Add( FR.Animation.flapWings );
		_operationAnimations[ FR.OperationType.DIVIDE ].Add( FR.Animation.jetFart );
		_operationAnimations[ FR.OperationType.DIVIDE ].Add( FR.Animation.lotus );
		_operationAnimations[ FR.OperationType.DIVIDE ].Add( FR.Animation.tractorBeam );

		_operationAnimations[ FR.OperationType.MULTIPLY ] = new List<FR.Animation>();
		_operationAnimations[ FR.OperationType.MULTIPLY ].Add( FR.Animation.featherPlop );
		_operationAnimations[ FR.OperationType.MULTIPLY ].Add( FR.Animation.headbang );
		_operationAnimations[ FR.OperationType.MULTIPLY ].Add( FR.Animation.violentShake );

		_operationAnimations[ FR.OperationType.SIMPLIFY ] = new List<FR.Animation>();
		_operationAnimations[ FR.OperationType.SIMPLIFY ].Add( FR.Animation.backFlip );
		_operationAnimations[ FR.OperationType.SIMPLIFY ].Add( FR.Animation.starFishJump );
		_operationAnimations[ FR.OperationType.SIMPLIFY ].Add( FR.Animation.twoHandsBoom );

		_operationAnimations[ FR.OperationType.SUBTRACT ] = new List<FR.Animation>();
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.blowpipe );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.boxingGlove );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.callAirstrike );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.castFireball );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.dropAnvil );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.explodeTNT );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.kickSoccerball );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.shootMortar );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.summon );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.summonLightningRightHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.summonMeteor );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.summon_02 );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwAmericanFootballLeftHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwAmericanFootballRightHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwBaseball );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwBaseballRightHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwBasketballLeftHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwBasketballRightHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwBoomerangLeftHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwBoomerangRightHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.throwGrenade );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.tornado );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.volleyballHitLeftHand );
		_operationAnimations[ FR.OperationType.SUBTRACT ].Add( FR.Animation.volleyballHitRightHand );


	}
}



