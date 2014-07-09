    
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_throwGrenade : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.throwGrenade;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Attacker, NumberRenderer Defender )
	{
        yield return Attacker.Animator.RotateTowards(Defender).Coroutine;

        Attacker.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(.5f); // hand behind back


        Prop grenade = PropFactory.use.CreateProp(FR.PropType.Grenade); 
        grenade.transform.parent = Attacker.interactionCharacter.LeftHand;
        grenade.transform.localPosition = new Vector3(-0.9188284f, -0.3741895f, 0.5828773f);
        grenade.transform.localEulerAngles = new Vector3(76.15785f, 344.165f, 64.68163f);


        yield return new WaitForSeconds(1.3f); // hand releases grenade

        Vector3 storedPosition = grenade.transform.position;
        grenade.transform.parent = null;
        grenade.transform.position = storedPosition;




        Vector3[] totalPath = new Vector3[3];
        totalPath[0] = storedPosition;
        totalPath[2] = Defender.interactionCharacter.Head.transform.position;
        totalPath[1] = Vector3.Lerp(totalPath[0], totalPath[2], 0.5f) + new Vector3(0, 2.5f, 0);



        // FIXME figure out a way to do a lob
        //Vector3[] beginPath = new Vector3[3];
        //beginPath[0] = totalPath[0];
        //beginPath[2] = totalPath[1];
        //beginPath[1] = Vector3.Lerp(beginPath[0], beginPath[2], .5f) + new Vector3(0, 1, 0);


        //Vector3[] endPath = new Vector3[3];
        //endPath[0] = beginPath[2];
        //endPath[2] = totalPath[2];
        //endPath[1] = Vector3.Lerp(endPath[0], endPath[2], .5f) + new Vector3(0, 1, 0);
        
        
        grenade.gameObject.MoveTo(totalPath).MoveToPath(false).Time(.6f).Execute();
        grenade.gameObject.RotateTo(new Vector3(180, 80, 0)).Time(.5f).Execute();





        yield return new WaitForSeconds(0.45f);

        PropFactory.use.FreeProp(grenade);
        Defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);

        yield return new WaitForSeconds(0.2f);


	}
}
