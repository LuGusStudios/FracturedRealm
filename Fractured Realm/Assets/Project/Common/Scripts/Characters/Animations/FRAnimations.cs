
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum FRAnimation
{
	NONE = -1, 
	idle = -178524345,
	lowFiveReceiveLefttHand = 123265927,
	lowFiveReceiveRightHand = -426334593,
	magnetAttractedFront = 2093565510,
	magnetAttractedSideLeft = -1676399453,
	magnetAttractedSideRight = -1197859353,
	supermanWatch = 1312703362,
	chestBump = -576715917,
	forcePull = -1764252714,
	freshPrince = 730543127,
	highFiveLeftHand = 1101443251,
	highFiveRightHand = 1667387702,
	lowFiveSlapLeftHand = -1215737478,
	lowFiveSlapRightHand = -2127821629,
	magnetAtrractLeft = -1106126365,
	magnetAtrractRight = -832284286,
	mrWillieBam = 851302718,
	supermanJump = 1319978637,
	cheering = 34390176,
	crazySignLookLeft = 293460485,
	crazySignLookRight = -2019396916,
	headScratchLookLeft = -1662210992,
	headScratchLookRight = -1674688831,
	flapWings = -804928702,
	jetFart = -372105823,
	lotus = 833697178,
	tractorBeam = -242418756,
	fallInLeft = 1219030451,
	fallInRight = -1510362814,
	fishingPulledIn = -1623192120,
	ironManFall = 1797527838,
	maryPoppins = -1154752180,
	maryPoppinsFall = 138656333,
	shrugAndDrop = -1734119106,
	waveAndDrop = -248037782,
	bigMaskLookAngry = 1761717563,
	bigMaskLookNeutral = 1115670771,
	mediumMaskLookAngry = -1496809486,
	mediumMaskLookNeutral = 815454405,
	smallMaskLookAngry = -1859231287,
	smallMaskLookNeutral = -165200520,
	floating = -1029462549,
	running = -1961299755,
	turnLeft10 = 2124953321,
	turnLeft20 = 1434805546,
	turnLeft30 = 1285444715,
	turnLeft40 = 64996012,
	turnLeft50 = 449115117,
	turnLeft60 = 837394478,
	turnLeft70 = 686985583,
	turnLeft80 = -1351971424,
	turnLeft90 = -1234060063,
	turnRight10 = 632665566,
	turnRight20 = 244901405,
	turnRight30 = 394516316,
	turnRight40 = 1489125787,
	turnRight50 = 1104769242,
	turnRight60 = 1794384665,
	turnRight70 = 1945047640,
	turnRight80 = -193524073,
	turnRight90 = -311672874,
	featherPlop = -1930819229,
	headbang = -529993720,
	violentShake = 269461105,
	shakeNoAndPlantSign = 484019339,
	drEvil = 1567165407,
	backFlip = 2020844167,
	starFishJump = 458244747,
	twoHandsBoom = -381782375,
	blowpipe = 144376158,
	boxingGlove = -908690230,
	callAirstrike = -1889655412,
	castFireball = -1744296412,
	dropAnvil = -1268919018,
	explodeTNT = -1948904948,
	kickSoccerball = 69401085,
	shootMortar = -126765314,
	summon = -1387642857,
	summonLightningRightHand = -130191979,
	summonMeteor = -959445676,
	summon_02 = -265714369,
	throwAmericanFootballLeftHand = -1400141880,
	throwAmericanFootballRightHand = -1537682251,
	throwBaseball = 1445084204,
	throwBaseballRightHand = 1101773290,
	throwBasketballLeftHand = 609256038,
	throwBasketballRightHand = -1782677153,
	throwBoomerangLeftHand = -489268668,
	throwBoomerangRightHand = 1075332707,
	throwGrenade = 2007250806,
	tornado = -336647795,
	volleyballHitLeftHand = 2116695232,
	volleyballHitRightHand = -1437467340,
	hitFront = -1914119007,
	hitSideLeft = -142698311,
	hitSideRight = 1167298089,
};
	
public class FRAnimations
{

	public static FRAnimation TypeFromString( string animationName )
	{
		try
		{
			return (FRAnimation) Enum.Parse( typeof(FRAnimation), animationName ); 

		}
		catch( ArgumentException e )
		{
			return FRAnimation.NONE;
		}
	}
	
	protected static Dictionary<int, FRAnimationData> _animations = null;
	public static Dictionary<int, FRAnimationData> animations
	{
		get
		{
			if( _animations == null )
			{
				_animations = new Dictionary<int, FRAnimationData>();
				FillDictionary();
			}

			return _animations;
		}
	}

	public static void FillDictionary()
	{
		FRAnimationData animation = null;

		animation = new FRAnimationData();
		animation.name = "idle";
		animation.hash = -178524345;
		animation.parentName = "Base Layer";
		animation.layer = 0;
		animations.Add(-178524345, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveReceiveLefttHand";
		animation.hash = 123265927;
		animation.parentName = "/Add/Receiver";
		animation.layer = 0;
		animations.Add(123265927, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveReceiveRightHand";
		animation.hash = -426334593;
		animation.parentName = "/Add/Receiver";
		animation.layer = 0;
		animations.Add(-426334593, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAttractedFront";
		animation.hash = 2093565510;
		animation.parentName = "/Add/Receiver";
		animation.layer = 0;
		animations.Add(2093565510, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAttractedSideLeft";
		animation.hash = -1676399453;
		animation.parentName = "/Add/Receiver";
		animation.layer = 0;
		animations.Add(-1676399453, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAttractedSideRight";
		animation.hash = -1197859353;
		animation.parentName = "/Add/Receiver";
		animation.layer = 0;
		animations.Add(-1197859353, animation);

		animation = new FRAnimationData();
		animation.name = "supermanWatch";
		animation.hash = 1312703362;
		animation.parentName = "/Add/Receiver";
		animation.layer = 0;
		animations.Add(1312703362, animation);

		animation = new FRAnimationData();
		animation.name = "chestBump";
		animation.hash = -576715917;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(-576715917, animation);

		animation = new FRAnimationData();
		animation.name = "forcePull";
		animation.hash = -1764252714;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(-1764252714, animation);

		animation = new FRAnimationData();
		animation.name = "freshPrince";
		animation.hash = 730543127;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(730543127, animation);

		animation = new FRAnimationData();
		animation.name = "highFiveLeftHand";
		animation.hash = 1101443251;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(1101443251, animation);

		animation = new FRAnimationData();
		animation.name = "highFiveRightHand";
		animation.hash = 1667387702;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(1667387702, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveSlapLeftHand";
		animation.hash = -1215737478;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(-1215737478, animation);

		animation = new FRAnimationData();
		animation.name = "lowFiveSlapRightHand";
		animation.hash = -2127821629;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(-2127821629, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAtrractLeft";
		animation.hash = -1106126365;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(-1106126365, animation);

		animation = new FRAnimationData();
		animation.name = "magnetAtrractRight";
		animation.hash = -832284286;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(-832284286, animation);

		animation = new FRAnimationData();
		animation.name = "mrWillieBam";
		animation.hash = 851302718;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(851302718, animation);

		animation = new FRAnimationData();
		animation.name = "supermanJump";
		animation.hash = 1319978637;
		animation.parentName = "/Add/Starter";
		animation.layer = 0;
		animations.Add(1319978637, animation);

		animation = new FRAnimationData();
		animation.name = "cheering";
		animation.hash = 34390176;
		animation.parentName = "/Cheers";
		animation.layer = 0;
		animations.Add(34390176, animation);

		animation = new FRAnimationData();
		animation.name = "crazySignLookLeft";
		animation.hash = 293460485;
		animation.parentName = "/Cheers";
		animation.layer = 0;
		animations.Add(293460485, animation);

		animation = new FRAnimationData();
		animation.name = "crazySignLookRight";
		animation.hash = -2019396916;
		animation.parentName = "/Cheers";
		animation.layer = 0;
		animations.Add(-2019396916, animation);

		animation = new FRAnimationData();
		animation.name = "headScratchLookLeft";
		animation.hash = -1662210992;
		animation.parentName = "/Cheers";
		animation.layer = 0;
		animations.Add(-1662210992, animation);

		animation = new FRAnimationData();
		animation.name = "headScratchLookRight";
		animation.hash = -1674688831;
		animation.parentName = "/Cheers";
		animation.layer = 0;
		animations.Add(-1674688831, animation);

		animation = new FRAnimationData();
		animation.name = "flapWings";
		animation.hash = -804928702;
		animation.parentName = "/Divide/Ascend";
		animation.layer = 0;
		animations.Add(-804928702, animation);

		animation = new FRAnimationData();
		animation.name = "jetFart";
		animation.hash = -372105823;
		animation.parentName = "/Divide/Ascend";
		animation.layer = 0;
		animations.Add(-372105823, animation);

		animation = new FRAnimationData();
		animation.name = "lotus";
		animation.hash = 833697178;
		animation.parentName = "/Divide/Ascend";
		animation.layer = 0;
		animations.Add(833697178, animation);

		animation = new FRAnimationData();
		animation.name = "tractorBeam";
		animation.hash = -242418756;
		animation.parentName = "/Divide/Ascend";
		animation.layer = 0;
		animations.Add(-242418756, animation);

		animation = new FRAnimationData();
		animation.name = "fallInLeft";
		animation.hash = 1219030451;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(1219030451, animation);

		animation = new FRAnimationData();
		animation.name = "fallInRight";
		animation.hash = -1510362814;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(-1510362814, animation);

		animation = new FRAnimationData();
		animation.name = "fishingPulledIn";
		animation.hash = -1623192120;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(-1623192120, animation);

		animation = new FRAnimationData();
		animation.name = "ironManFall";
		animation.hash = 1797527838;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(1797527838, animation);

		animation = new FRAnimationData();
		animation.name = "maryPoppins";
		animation.hash = -1154752180;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(-1154752180, animation);

		animation = new FRAnimationData();
		animation.name = "maryPoppinsFall";
		animation.hash = 138656333;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(138656333, animation);

		animation = new FRAnimationData();
		animation.name = "shrugAndDrop";
		animation.hash = -1734119106;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(-1734119106, animation);

		animation = new FRAnimationData();
		animation.name = "waveAndDrop";
		animation.hash = -248037782;
		animation.parentName = "/Divide/Descend";
		animation.layer = 0;
		animations.Add(-248037782, animation);

		animation = new FRAnimationData();
		animation.name = "bigMaskLookAngry";
		animation.hash = 1761717563;
		animation.parentName = "/Facial";
		animation.layer = 0;
		animations.Add(1761717563, animation);

		animation = new FRAnimationData();
		animation.name = "bigMaskLookNeutral";
		animation.hash = 1115670771;
		animation.parentName = "/Facial";
		animation.layer = 0;
		animations.Add(1115670771, animation);

		animation = new FRAnimationData();
		animation.name = "mediumMaskLookAngry";
		animation.hash = -1496809486;
		animation.parentName = "/Facial";
		animation.layer = 0;
		animations.Add(-1496809486, animation);

		animation = new FRAnimationData();
		animation.name = "mediumMaskLookNeutral";
		animation.hash = 815454405;
		animation.parentName = "/Facial";
		animation.layer = 0;
		animations.Add(815454405, animation);

		animation = new FRAnimationData();
		animation.name = "smallMaskLookAngry";
		animation.hash = -1859231287;
		animation.parentName = "/Facial";
		animation.layer = 0;
		animations.Add(-1859231287, animation);

		animation = new FRAnimationData();
		animation.name = "smallMaskLookNeutral";
		animation.hash = -165200520;
		animation.parentName = "/Facial";
		animation.layer = 0;
		animations.Add(-165200520, animation);

		animation = new FRAnimationData();
		animation.name = "floating";
		animation.hash = -1029462549;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(-1029462549, animation);

		animation = new FRAnimationData();
		animation.name = "running";
		animation.hash = -1961299755;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(-1961299755, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft10";
		animation.hash = 2124953321;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(2124953321, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft20";
		animation.hash = 1434805546;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(1434805546, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft30";
		animation.hash = 1285444715;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(1285444715, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft40";
		animation.hash = 64996012;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(64996012, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft50";
		animation.hash = 449115117;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(449115117, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft60";
		animation.hash = 837394478;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(837394478, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft70";
		animation.hash = 686985583;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(686985583, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft80";
		animation.hash = -1351971424;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(-1351971424, animation);

		animation = new FRAnimationData();
		animation.name = "turnLeft90";
		animation.hash = -1234060063;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(-1234060063, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight10";
		animation.hash = 632665566;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(632665566, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight20";
		animation.hash = 244901405;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(244901405, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight30";
		animation.hash = 394516316;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(394516316, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight40";
		animation.hash = 1489125787;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(1489125787, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight50";
		animation.hash = 1104769242;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(1104769242, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight60";
		animation.hash = 1794384665;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(1794384665, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight70";
		animation.hash = 1945047640;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(1945047640, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight80";
		animation.hash = -193524073;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(-193524073, animation);

		animation = new FRAnimationData();
		animation.name = "turnRight90";
		animation.hash = -311672874;
		animation.parentName = "/Movements";
		animation.layer = 0;
		animations.Add(-311672874, animation);

		animation = new FRAnimationData();
		animation.name = "featherPlop";
		animation.hash = -1930819229;
		animation.parentName = "/Multiply";
		animation.layer = 0;
		animations.Add(-1930819229, animation);

		animation = new FRAnimationData();
		animation.name = "headbang";
		animation.hash = -529993720;
		animation.parentName = "/Multiply";
		animation.layer = 0;
		animations.Add(-529993720, animation);

		animation = new FRAnimationData();
		animation.name = "violentShake";
		animation.hash = 269461105;
		animation.parentName = "/Multiply";
		animation.layer = 0;
		animations.Add(269461105, animation);

		animation = new FRAnimationData();
		animation.name = "shakeNoAndPlantSign";
		animation.hash = 484019339;
		animation.parentName = "/Other";
		animation.layer = 0;
		animations.Add(484019339, animation);

		animation = new FRAnimationData();
		animation.name = "drEvil";
		animation.hash = 1567165407;
		animation.parentName = "/Portal/Success";
		animation.layer = 0;
		animations.Add(1567165407, animation);

		animation = new FRAnimationData();
		animation.name = "backFlip";
		animation.hash = 2020844167;
		animation.parentName = "/Simplify";
		animation.layer = 0;
		animations.Add(2020844167, animation);

		animation = new FRAnimationData();
		animation.name = "starFishJump";
		animation.hash = 458244747;
		animation.parentName = "/Simplify";
		animation.layer = 0;
		animations.Add(458244747, animation);

		animation = new FRAnimationData();
		animation.name = "twoHandsBoom";
		animation.hash = -381782375;
		animation.parentName = "/Simplify";
		animation.layer = 0;
		animations.Add(-381782375, animation);

		animation = new FRAnimationData();
		animation.name = "blowpipe";
		animation.hash = 144376158;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(144376158, animation);

		animation = new FRAnimationData();
		animation.name = "boxingGlove";
		animation.hash = -908690230;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-908690230, animation);

		animation = new FRAnimationData();
		animation.name = "callAirstrike";
		animation.hash = -1889655412;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1889655412, animation);

		animation = new FRAnimationData();
		animation.name = "castFireball";
		animation.hash = -1744296412;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1744296412, animation);

		animation = new FRAnimationData();
		animation.name = "dropAnvil";
		animation.hash = -1268919018;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1268919018, animation);

		animation = new FRAnimationData();
		animation.name = "explodeTNT";
		animation.hash = -1948904948;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1948904948, animation);

		animation = new FRAnimationData();
		animation.name = "kickSoccerball";
		animation.hash = 69401085;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(69401085, animation);

		animation = new FRAnimationData();
		animation.name = "shootMortar";
		animation.hash = -126765314;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-126765314, animation);

		animation = new FRAnimationData();
		animation.name = "summon";
		animation.hash = -1387642857;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1387642857, animation);

		animation = new FRAnimationData();
		animation.name = "summonLightningRightHand";
		animation.hash = -130191979;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-130191979, animation);

		animation = new FRAnimationData();
		animation.name = "summonMeteor";
		animation.hash = -959445676;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-959445676, animation);

		animation = new FRAnimationData();
		animation.name = "summon_02";
		animation.hash = -265714369;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-265714369, animation);

		animation = new FRAnimationData();
		animation.name = "throwAmericanFootballLeftHand";
		animation.hash = -1400141880;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1400141880, animation);

		animation = new FRAnimationData();
		animation.name = "throwAmericanFootballRightHand";
		animation.hash = -1537682251;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1537682251, animation);

		animation = new FRAnimationData();
		animation.name = "throwBaseball";
		animation.hash = 1445084204;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(1445084204, animation);

		animation = new FRAnimationData();
		animation.name = "throwBaseballRightHand";
		animation.hash = 1101773290;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(1101773290, animation);

		animation = new FRAnimationData();
		animation.name = "throwBasketballLeftHand";
		animation.hash = 609256038;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(609256038, animation);

		animation = new FRAnimationData();
		animation.name = "throwBasketballRightHand";
		animation.hash = -1782677153;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1782677153, animation);

		animation = new FRAnimationData();
		animation.name = "throwBoomerangLeftHand";
		animation.hash = -489268668;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-489268668, animation);

		animation = new FRAnimationData();
		animation.name = "throwBoomerangRightHand";
		animation.hash = 1075332707;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(1075332707, animation);

		animation = new FRAnimationData();
		animation.name = "throwGrenade";
		animation.hash = 2007250806;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(2007250806, animation);

		animation = new FRAnimationData();
		animation.name = "tornado";
		animation.hash = -336647795;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-336647795, animation);

		animation = new FRAnimationData();
		animation.name = "volleyballHitLeftHand";
		animation.hash = 2116695232;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(2116695232, animation);

		animation = new FRAnimationData();
		animation.name = "volleyballHitRightHand";
		animation.hash = -1437467340;
		animation.parentName = "/Subtract/Attacks";
		animation.layer = 0;
		animations.Add(-1437467340, animation);

		animation = new FRAnimationData();
		animation.name = "hitFront";
		animation.hash = -1914119007;
		animation.parentName = "/Subtract/Hits";
		animation.layer = 0;
		animations.Add(-1914119007, animation);

		animation = new FRAnimationData();
		animation.name = "hitSideLeft";
		animation.hash = -142698311;
		animation.parentName = "/Subtract/Hits";
		animation.layer = 0;
		animations.Add(-142698311, animation);

		animation = new FRAnimationData();
		animation.name = "hitSideRight";
		animation.hash = 1167298089;
		animation.parentName = "/Subtract/Hits";
		animation.layer = 0;
		animations.Add(1167298089, animation);


	}

	public FRAnimations()
	{
		FillDictionary();
	}
}

