
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FR
{
	public enum AnimationStage
	{
		NONE = -1,
		Stage1 = 1,
		Stage2 = 2
	}

	public enum Animation
{
	NONE = -1, 
	idle = -178524345,
	lowFiveReceiveLefttHand = 1394721566,
	lowFiveReceiveRightHand = -1292925210,
	magnetAttractedFront = 807729372,
	magnetAttractedSideLeft = -932377542,
	magnetAttractedSideRight = 824004391,
	supermanWatch = -865762447,
	chestBump = -1881519238,
	forcePull = -995234849,
	freshPrince = -2108812154,
	highFiveLeftHand = -447528410,
	highFiveRightHand = -475806961,
	lowFiveSlapLeftHand = -1492341143,
	lowFiveSlapRightHand = 92455138,
	magnetAtrractLeft = 1053838298,
	magnetAtrractRight = -1847521256,
	mrWillieBam = -1686570577,
	supermanJump = -921952129,
	cheering = -1598974891,
	crazySignLookLeft = 151404393,
	crazySignLookRight = -1010949030,
	headScratchLookLeft = -2050718089,
	headScratchLookRight = 960355200,
	flapWings = -1694428202,
	jetFart = -318687071,
	lotus = 1603903191,
	tractorBeam = 1651649906,
	fallInLeft = -615105667,
	fallInRight = -642342877,
	fishingPulledIn = 795621940,
	ironManFall = 392890495,
	maryPoppins = -949686227,
	maryPoppinsFall = -1200648783,
	shrugAndDrop = -1570082847,
	waveAndDrop = -1921503477,
	bigMaskLookAngry = -1515997460,
	bigMaskLookNeutral = 107191909,
	mediumMaskLookAngry = -1075817003,
	mediumMaskLookNeutral = 837073502,
	smallMaskLookAngry = -718129313,
	smallMaskLookNeutral = 1395957817,
	floating = 447431443,
	running = -1898589571,
	turnLeft10 = -341523943,
	turnLeft20 = -1064726054,
	turnLeft30 = -644702053,
	turnLeft40 = -1764542884,
	turnLeft50 = -1882716387,
	turnLeft60 = -1528474402,
	turnLeft70 = -1107401313,
	turnLeft80 = 979794256,
	turnLeft90 = 595413009,
	turnRight10 = -1734522058,
	turnRight20 = -1280304907,
	turnRight30 = -1431623244,
	turnRight40 = -437600397,
	turnRight50 = -51278286,
	turnRight60 = -673391119,
	turnRight70 = -825758544,
	turnRight80 = 1231024255,
	turnRight90 = 1346683198,
	featherPlop = 434568595,
	headbang = -442117472,
	violentShake = -1389930343,
	shakeNoAndPlantSign = 1489361437,
	drEvil = 1505373919,
	backFlip = 2109636655,
	starFishJump = -1502036893,
	twoHandsBoom = 1410762865,
	blowpipe = -886989685,
	boxingGlove = -1378933481,
	callAirstrike = -1451289705,
	castFireball = 1611567380,
	dropAnvil = 1109871056,
	explodeTNT = -114880747,
	kickSoccerball = -1906241241,
	shootMortar = -1670844637,
	summon = -669810648,
	summonLightningRightHand = -1164697260,
	summonMeteor = 1054257764,
	summon_02 = 106109433,
	throwAmericanFootballLeftHand = -1109726642,
	throwAmericanFootballRightHand = -1601002547,
	throwBaseball = 1879247415,
	throwBaseballRightHand = -1801196609,
	throwBasketballLeftHand = 1065627471,
	throwBasketballRightHand = -686305890,
	throwBoomerangLeftHand = 937297937,
	throwBoomerangRightHand = 1540156234,
	throwGrenade = -1885175738,
	tornado = 1576820080,
	volleyballHitLeftHand = 372868578,
	volleyballHitRightHand = 2136584033,
	hitFront = -1098154141,
	hitSideLeft = 885261676,
	hitSideRight = -1276570897,
};
}

public partial class FRAnimations : LugusSingletonRuntime<FRAnimations>
{

	public FR.Animation TypeFromString( string animationName )
	{
		try
		{
			return (FR.Animation) Enum.Parse( typeof(FR.Animation), animationName ); 

		}
		catch( ArgumentException e )
		{
			return FR.Animation.NONE;
		}
	}
	
	protected Dictionary<int, FRAnimationData> _animations = null;
	public Dictionary<int, FRAnimationData> animations
	{
		get
		{
			if( _animations == null )
			{
				FillDictionary();
			}

			return _animations;
		}
	}

	public FRAnimationData GetAnimationData( FR.Animation animation )
	{
		return _animations[ (int) animation];
	}

	public void FillDictionary()
	{
		_animations = new Dictionary<int, FRAnimationData>();
				
		FRAnimationData animation = null;

		animation = new FRAnimationData();
		animation.name = "idle";
		animation.type = FR.Animation.idle;
		animation.hash = -178524345;
		animation.parentName = "Base Layer";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Base Layer";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-178524345, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveReceiveLefttHand";
		animation.type = FR.Animation.lowFiveReceiveLefttHand;
		animation.hash = 1394721566;
		animation.parentName = "Add.Receiver";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Receiver";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Receiver_lowFiveReceiveLefttHand(); };
		animations.Add(1394721566, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveReceiveRightHand";
		animation.type = FR.Animation.lowFiveReceiveRightHand;
		animation.hash = -1292925210;
		animation.parentName = "Add.Receiver";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Receiver";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Receiver_lowFiveReceiveRightHand(); };
		animations.Add(-1292925210, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAttractedFront";
		animation.type = FR.Animation.magnetAttractedFront;
		animation.hash = 807729372;
		animation.parentName = "Add.Receiver";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Receiver";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Receiver_magnetAttractedFront(); };
		animations.Add(807729372, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAttractedSideLeft";
		animation.type = FR.Animation.magnetAttractedSideLeft;
		animation.hash = -932377542;
		animation.parentName = "Add.Receiver";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Receiver";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Receiver_magnetAttractedSideLeft(); };
		animations.Add(-932377542, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAttractedSideRight";
		animation.type = FR.Animation.magnetAttractedSideRight;
		animation.hash = 824004391;
		animation.parentName = "Add.Receiver";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Receiver";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Receiver_magnetAttractedSideRight(); };
		animations.Add(824004391, animation);

		animation = new FRAnimationData();
		animation.name = "supermanWatch";
		animation.type = FR.Animation.supermanWatch;
		animation.hash = -865762447;
		animation.parentName = "Add.Receiver";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Receiver";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Receiver_supermanWatch(); };
		animations.Add(-865762447, animation);

		animation = new FRAnimationData();
		animation.name = "chestBump";
		animation.type = FR.Animation.chestBump;
		animation.hash = -1881519238;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_chestBump(); };
		animations.Add(-1881519238, animation);

		animation = new FRAnimationData();
		animation.name = "forcePull";
		animation.type = FR.Animation.forcePull;
		animation.hash = -995234849;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_forcePull(); };
		animations.Add(-995234849, animation);

		animation = new FRAnimationData();
		animation.name = "freshPrince";
		animation.type = FR.Animation.freshPrince;
		animation.hash = -2108812154;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_freshPrince(); };
		animations.Add(-2108812154, animation);

		animation = new FRAnimationData();
		animation.name = "highFiveLeftHand";
		animation.type = FR.Animation.highFiveLeftHand;
		animation.hash = -447528410;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_highFiveLeftHand(); };
		animations.Add(-447528410, animation);

		animation = new FRAnimationData();
		animation.name = "highFiveRightHand";
		animation.type = FR.Animation.highFiveRightHand;
		animation.hash = -475806961;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_highFiveRightHand(); };
		animations.Add(-475806961, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveSlapLeftHand";
		animation.type = FR.Animation.lowFiveSlapLeftHand;
		animation.hash = -1492341143;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_lowFiveSlapLeftHand(); };
		animations.Add(-1492341143, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveSlapRightHand";
		animation.type = FR.Animation.lowFiveSlapRightHand;
		animation.hash = 92455138;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_lowFiveSlapRightHand(); };
		animations.Add(92455138, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAtrractLeft";
		animation.type = FR.Animation.magnetAtrractLeft;
		animation.hash = 1053838298;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_magnetAtrractLeft(); };
		animations.Add(1053838298, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAtrractRight";
		animation.type = FR.Animation.magnetAtrractRight;
		animation.hash = -1847521256;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_magnetAtrractRight(); };
		animations.Add(-1847521256, animation);

		animation = new FRAnimationData();
		animation.name = "mrWillieBam";
		animation.type = FR.Animation.mrWillieBam;
		animation.hash = -1686570577;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_mrWillieBam(); };
		animations.Add(-1686570577, animation);

		animation = new FRAnimationData();
		animation.name = "supermanJump";
		animation.type = FR.Animation.supermanJump;
		animation.hash = -921952129;
		animation.parentName = "Add.Starter";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.ADD;
		animation.originalOperationName = "Add";
		animation.originalStageName = "Starter";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerAdd_Starter_supermanJump(); };
		animations.Add(-921952129, animation);

		animation = new FRAnimationData();
		animation.name = "cheering";
		animation.type = FR.Animation.cheering;
		animation.hash = -1598974891;
		animation.parentName = "Cheers";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Cheers";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1598974891, animation);

		animation = new FRAnimationData();
		animation.name = "crazySignLookLeft";
		animation.type = FR.Animation.crazySignLookLeft;
		animation.hash = 151404393;
		animation.parentName = "Cheers";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Cheers";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(151404393, animation);

		animation = new FRAnimationData();
		animation.name = "crazySignLookRight";
		animation.type = FR.Animation.crazySignLookRight;
		animation.hash = -1010949030;
		animation.parentName = "Cheers";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Cheers";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1010949030, animation);

		animation = new FRAnimationData();
		animation.name = "headScratchLookLeft";
		animation.type = FR.Animation.headScratchLookLeft;
		animation.hash = -2050718089;
		animation.parentName = "Cheers";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Cheers";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-2050718089, animation);

		animation = new FRAnimationData();
		animation.name = "headScratchLookRight";
		animation.type = FR.Animation.headScratchLookRight;
		animation.hash = 960355200;
		animation.parentName = "Cheers";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Cheers";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(960355200, animation);

		animation = new FRAnimationData();
		animation.name = "flapWings";
		animation.type = FR.Animation.flapWings;
		animation.hash = -1694428202;
		animation.parentName = "Divide.Ascend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Ascend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Ascend_flapWings(); };
		animations.Add(-1694428202, animation);

		animation = new FRAnimationData();
		animation.name = "jetFart";
		animation.type = FR.Animation.jetFart;
		animation.hash = -318687071;
		animation.parentName = "Divide.Ascend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Ascend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Ascend_jetFart(); };
		animations.Add(-318687071, animation);

		animation = new FRAnimationData();
		animation.name = "lotus";
		animation.type = FR.Animation.lotus;
		animation.hash = 1603903191;
		animation.parentName = "Divide.Ascend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Ascend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Ascend_lotus(); };
		animations.Add(1603903191, animation);

		animation = new FRAnimationData();
		animation.name = "tractorBeam";
		animation.type = FR.Animation.tractorBeam;
		animation.hash = 1651649906;
		animation.parentName = "Divide.Ascend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Ascend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Ascend_tractorBeam(); };
		animations.Add(1651649906, animation);

		animation = new FRAnimationData();
		animation.name = "fallInLeft";
		animation.type = FR.Animation.fallInLeft;
		animation.hash = -615105667;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_fallInLeft(); };
		animations.Add(-615105667, animation);

		animation = new FRAnimationData();
		animation.name = "fallInRight";
		animation.type = FR.Animation.fallInRight;
		animation.hash = -642342877;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_fallInRight(); };
		animations.Add(-642342877, animation);

		animation = new FRAnimationData();
		animation.name = "fishingPulledIn";
		animation.type = FR.Animation.fishingPulledIn;
		animation.hash = 795621940;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_fishingPulledIn(); };
		animations.Add(795621940, animation);

		animation = new FRAnimationData();
		animation.name = "ironManFall";
		animation.type = FR.Animation.ironManFall;
		animation.hash = 392890495;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_ironManFall(); };
		animations.Add(392890495, animation);

		animation = new FRAnimationData();
		animation.name = "maryPoppins";
		animation.type = FR.Animation.maryPoppins;
		animation.hash = -949686227;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_maryPoppins(); };
		animations.Add(-949686227, animation);

		animation = new FRAnimationData();
		animation.name = "maryPoppinsFall";
		animation.type = FR.Animation.maryPoppinsFall;
		animation.hash = -1200648783;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_maryPoppinsFall(); };
		animations.Add(-1200648783, animation);

		animation = new FRAnimationData();
		animation.name = "shrugAndDrop";
		animation.type = FR.Animation.shrugAndDrop;
		animation.hash = -1570082847;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_shrugAndDrop(); };
		animations.Add(-1570082847, animation);

		animation = new FRAnimationData();
		animation.name = "waveAndDrop";
		animation.type = FR.Animation.waveAndDrop;
		animation.hash = -1921503477;
		animation.parentName = "Divide.Descend";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.DIVIDE;
		animation.originalOperationName = "Divide";
		animation.originalStageName = "Descend";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerDivide_Descend_waveAndDrop(); };
		animations.Add(-1921503477, animation);

		animation = new FRAnimationData();
		animation.name = "bigMaskLookAngry";
		animation.type = FR.Animation.bigMaskLookAngry;
		animation.hash = -1515997460;
		animation.parentName = "Facial";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Facial";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1515997460, animation);

		animation = new FRAnimationData();
		animation.name = "bigMaskLookNeutral";
		animation.type = FR.Animation.bigMaskLookNeutral;
		animation.hash = 107191909;
		animation.parentName = "Facial";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Facial";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(107191909, animation);

		animation = new FRAnimationData();
		animation.name = "mediumMaskLookAngry";
		animation.type = FR.Animation.mediumMaskLookAngry;
		animation.hash = -1075817003;
		animation.parentName = "Facial";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Facial";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1075817003, animation);

		animation = new FRAnimationData();
		animation.name = "mediumMaskLookNeutral";
		animation.type = FR.Animation.mediumMaskLookNeutral;
		animation.hash = 837073502;
		animation.parentName = "Facial";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Facial";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(837073502, animation);

		animation = new FRAnimationData();
		animation.name = "smallMaskLookAngry";
		animation.type = FR.Animation.smallMaskLookAngry;
		animation.hash = -718129313;
		animation.parentName = "Facial";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Facial";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-718129313, animation);

		animation = new FRAnimationData();
		animation.name = "smallMaskLookNeutral";
		animation.type = FR.Animation.smallMaskLookNeutral;
		animation.hash = 1395957817;
		animation.parentName = "Facial";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Facial";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(1395957817, animation);

		animation = new FRAnimationData();
		animation.name = "floating";
		animation.type = FR.Animation.floating;
		animation.hash = 447431443;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(447431443, animation);

		animation = new FRAnimationData();
		animation.name = "running";
		animation.type = FR.Animation.running;
		animation.hash = -1898589571;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1898589571, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft10";
		animation.type = FR.Animation.turnLeft10;
		animation.hash = -341523943;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-341523943, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft20";
		animation.type = FR.Animation.turnLeft20;
		animation.hash = -1064726054;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1064726054, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft30";
		animation.type = FR.Animation.turnLeft30;
		animation.hash = -644702053;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-644702053, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft40";
		animation.type = FR.Animation.turnLeft40;
		animation.hash = -1764542884;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1764542884, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft50";
		animation.type = FR.Animation.turnLeft50;
		animation.hash = -1882716387;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1882716387, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft60";
		animation.type = FR.Animation.turnLeft60;
		animation.hash = -1528474402;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1528474402, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft70";
		animation.type = FR.Animation.turnLeft70;
		animation.hash = -1107401313;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1107401313, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft80";
		animation.type = FR.Animation.turnLeft80;
		animation.hash = 979794256;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(979794256, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft90";
		animation.type = FR.Animation.turnLeft90;
		animation.hash = 595413009;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(595413009, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight10";
		animation.type = FR.Animation.turnRight10;
		animation.hash = -1734522058;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1734522058, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight20";
		animation.type = FR.Animation.turnRight20;
		animation.hash = -1280304907;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1280304907, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight30";
		animation.type = FR.Animation.turnRight30;
		animation.hash = -1431623244;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-1431623244, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight40";
		animation.type = FR.Animation.turnRight40;
		animation.hash = -437600397;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-437600397, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight50";
		animation.type = FR.Animation.turnRight50;
		animation.hash = -51278286;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-51278286, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight60";
		animation.type = FR.Animation.turnRight60;
		animation.hash = -673391119;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-673391119, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight70";
		animation.type = FR.Animation.turnRight70;
		animation.hash = -825758544;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(-825758544, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight80";
		animation.type = FR.Animation.turnRight80;
		animation.hash = 1231024255;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(1231024255, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight90";
		animation.type = FR.Animation.turnRight90;
		animation.hash = 1346683198;
		animation.parentName = "Movements";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Movements";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(1346683198, animation);

		animation = new FRAnimationData();
		animation.name = "featherPlop";
		animation.type = FR.Animation.featherPlop;
		animation.hash = 434568595;
		animation.parentName = "Multiply";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.MULTIPLY;
		animation.originalOperationName = "Multiply";
		animation.originalStageName = "";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerMultiply_featherPlop(); };
		animations.Add(434568595, animation);

		animation = new FRAnimationData();
		animation.name = "headbang";
		animation.type = FR.Animation.headbang;
		animation.hash = -442117472;
		animation.parentName = "Multiply";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.MULTIPLY;
		animation.originalOperationName = "Multiply";
		animation.originalStageName = "";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerMultiply_headbang(); };
		animations.Add(-442117472, animation);

		animation = new FRAnimationData();
		animation.name = "violentShake";
		animation.type = FR.Animation.violentShake;
		animation.hash = -1389930343;
		animation.parentName = "Multiply";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.MULTIPLY;
		animation.originalOperationName = "Multiply";
		animation.originalStageName = "";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerMultiply_violentShake(); };
		animations.Add(-1389930343, animation);

		animation = new FRAnimationData();
		animation.name = "shakeNoAndPlantSign";
		animation.type = FR.Animation.shakeNoAndPlantSign;
		animation.hash = 1489361437;
		animation.parentName = "Other";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Other";
		animation.originalStageName = "";
		animation.visualizerCreate = null;
		animations.Add(1489361437, animation);

		animation = new FRAnimationData();
		animation.name = "drEvil";
		animation.type = FR.Animation.drEvil;
		animation.hash = 1505373919;
		animation.parentName = "Portal.Success";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.NONE;
		animation.originalOperationName = "Portal";
		animation.originalStageName = "Success";
		animation.visualizerCreate = null;
		animations.Add(1505373919, animation);

		animation = new FRAnimationData();
		animation.name = "backFlip";
		animation.type = FR.Animation.backFlip;
		animation.hash = 2109636655;
		animation.parentName = "Simplify";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.SIMPLIFY;
		animation.originalOperationName = "Simplify";
		animation.originalStageName = "";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSimplify_backFlip(); };
		animations.Add(2109636655, animation);

		animation = new FRAnimationData();
		animation.name = "starFishJump";
		animation.type = FR.Animation.starFishJump;
		animation.hash = -1502036893;
		animation.parentName = "Simplify";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.SIMPLIFY;
		animation.originalOperationName = "Simplify";
		animation.originalStageName = "";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSimplify_starFishJump(); };
		animations.Add(-1502036893, animation);

		animation = new FRAnimationData();
		animation.name = "twoHandsBoom";
		animation.type = FR.Animation.twoHandsBoom;
		animation.hash = 1410762865;
		animation.parentName = "Simplify";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.NONE;
		animation.operation = FR.OperationType.SIMPLIFY;
		animation.originalOperationName = "Simplify";
		animation.originalStageName = "";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSimplify_twoHandsBoom(); };
		animations.Add(1410762865, animation);

		animation = new FRAnimationData();
		animation.name = "blowpipe";
		animation.type = FR.Animation.blowpipe;
		animation.hash = -886989685;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_blowpipe(); };
		animations.Add(-886989685, animation);

		animation = new FRAnimationData();
		animation.name = "boxingGlove";
		animation.type = FR.Animation.boxingGlove;
		animation.hash = -1378933481;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_boxingGlove(); };
		animations.Add(-1378933481, animation);

		animation = new FRAnimationData();
		animation.name = "callAirstrike";
		animation.type = FR.Animation.callAirstrike;
		animation.hash = -1451289705;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_callAirstrike(); };
		animations.Add(-1451289705, animation);

		animation = new FRAnimationData();
		animation.name = "castFireball";
		animation.type = FR.Animation.castFireball;
		animation.hash = 1611567380;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_castFireball(); };
		animations.Add(1611567380, animation);

		animation = new FRAnimationData();
		animation.name = "dropAnvil";
		animation.type = FR.Animation.dropAnvil;
		animation.hash = 1109871056;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_dropAnvil(); };
		animations.Add(1109871056, animation);

		animation = new FRAnimationData();
		animation.name = "explodeTNT";
		animation.type = FR.Animation.explodeTNT;
		animation.hash = -114880747;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_explodeTNT(); };
		animations.Add(-114880747, animation);

		animation = new FRAnimationData();
		animation.name = "kickSoccerball";
		animation.type = FR.Animation.kickSoccerball;
		animation.hash = -1906241241;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_kickSoccerball(); };
		animations.Add(-1906241241, animation);

		animation = new FRAnimationData();
		animation.name = "shootMortar";
		animation.type = FR.Animation.shootMortar;
		animation.hash = -1670844637;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_shootMortar(); };
		animations.Add(-1670844637, animation);

		animation = new FRAnimationData();
		animation.name = "summon";
		animation.type = FR.Animation.summon;
		animation.hash = -669810648;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_summon(); };
		animations.Add(-669810648, animation);

		animation = new FRAnimationData();
		animation.name = "summonLightningRightHand";
		animation.type = FR.Animation.summonLightningRightHand;
		animation.hash = -1164697260;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_summonLightningRightHand(); };
		animations.Add(-1164697260, animation);

		animation = new FRAnimationData();
		animation.name = "summonMeteor";
		animation.type = FR.Animation.summonMeteor;
		animation.hash = 1054257764;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_summonMeteor(); };
		animations.Add(1054257764, animation);

		animation = new FRAnimationData();
		animation.name = "summon_02";
		animation.type = FR.Animation.summon_02;
		animation.hash = 106109433;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_summon_02(); };
		animations.Add(106109433, animation);

		animation = new FRAnimationData();
		animation.name = "throwAmericanFootballLeftHand";
		animation.type = FR.Animation.throwAmericanFootballLeftHand;
		animation.hash = -1109726642;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwAmericanFootballLeftHand(); };
		animations.Add(-1109726642, animation);

		animation = new FRAnimationData();
		animation.name = "throwAmericanFootballRightHand";
		animation.type = FR.Animation.throwAmericanFootballRightHand;
		animation.hash = -1601002547;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwAmericanFootballRightHand(); };
		animations.Add(-1601002547, animation);

		animation = new FRAnimationData();
		animation.name = "throwBaseball";
		animation.type = FR.Animation.throwBaseball;
		animation.hash = 1879247415;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwBaseball(); };
		animations.Add(1879247415, animation);

		animation = new FRAnimationData();
		animation.name = "throwBaseballRightHand";
		animation.type = FR.Animation.throwBaseballRightHand;
		animation.hash = -1801196609;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwBaseballRightHand(); };
		animations.Add(-1801196609, animation);

		animation = new FRAnimationData();
		animation.name = "throwBasketballLeftHand";
		animation.type = FR.Animation.throwBasketballLeftHand;
		animation.hash = 1065627471;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwBasketballLeftHand(); };
		animations.Add(1065627471, animation);

		animation = new FRAnimationData();
		animation.name = "throwBasketballRightHand";
		animation.type = FR.Animation.throwBasketballRightHand;
		animation.hash = -686305890;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwBasketballRightHand(); };
		animations.Add(-686305890, animation);

		animation = new FRAnimationData();
		animation.name = "throwBoomerangLeftHand";
		animation.type = FR.Animation.throwBoomerangLeftHand;
		animation.hash = 937297937;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwBoomerangLeftHand(); };
		animations.Add(937297937, animation);

		animation = new FRAnimationData();
		animation.name = "throwBoomerangRightHand";
		animation.type = FR.Animation.throwBoomerangRightHand;
		animation.hash = 1540156234;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwBoomerangRightHand(); };
		animations.Add(1540156234, animation);

		animation = new FRAnimationData();
		animation.name = "throwGrenade";
		animation.type = FR.Animation.throwGrenade;
		animation.hash = -1885175738;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_throwGrenade(); };
		animations.Add(-1885175738, animation);

		animation = new FRAnimationData();
		animation.name = "tornado";
		animation.type = FR.Animation.tornado;
		animation.hash = 1576820080;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_tornado(); };
		animations.Add(1576820080, animation);

		animation = new FRAnimationData();
		animation.name = "volleyballHitLeftHand";
		animation.type = FR.Animation.volleyballHitLeftHand;
		animation.hash = 372868578;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_volleyballHitLeftHand(); };
		animations.Add(372868578, animation);

		animation = new FRAnimationData();
		animation.name = "volleyballHitRightHand";
		animation.type = FR.Animation.volleyballHitRightHand;
		animation.hash = 2136584033;
		animation.parentName = "Subtract.Attacks";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage1;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Attacks";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Attacks_volleyballHitRightHand(); };
		animations.Add(2136584033, animation);

		animation = new FRAnimationData();
		animation.name = "hitFront";
		animation.type = FR.Animation.hitFront;
		animation.hash = -1098154141;
		animation.parentName = "Subtract.Hits";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Hits";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Hits_hitFront(); };
		animations.Add(-1098154141, animation);

		animation = new FRAnimationData();
		animation.name = "hitSideLeft";
		animation.type = FR.Animation.hitSideLeft;
		animation.hash = 885261676;
		animation.parentName = "Subtract.Hits";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Hits";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Hits_hitSideLeft(); };
		animations.Add(885261676, animation);

		animation = new FRAnimationData();
		animation.name = "hitSideRight";
		animation.type = FR.Animation.hitSideRight;
		animation.hash = -1276570897;
		animation.parentName = "Subtract.Hits";
		animation.layer = 0;
		animation.stage = FR.AnimationStage.Stage2;
		animation.operation = FR.OperationType.SUBTRACT;
		animation.originalOperationName = "Subtract";
		animation.originalStageName = "Hits";
		animation.visualizerCreate = delegate(){ return new OperationVisualizerSubtract_Hits_hitSideRight(); };
		animations.Add(-1276570897, animation);


	}

	public FRAnimations()
	{
		FillDictionary();
	}
}

