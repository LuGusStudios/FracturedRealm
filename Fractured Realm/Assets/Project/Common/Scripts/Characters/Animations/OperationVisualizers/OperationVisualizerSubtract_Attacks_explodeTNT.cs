using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_explodeTNT : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.explodeTNT;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer attacker, NumberRenderer defender )
	{
        yield return attacker.Animator.RotateTowards(defender).Coroutine;

        attacker.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(.7f);

        Prop dynamite = PropFactory.use.CreateProp(FR.PropType.Dynamite01);
        Prop detonator = PropFactory.use.CreateProp(FR.PropType.Detonator01);
        GameObject plunger = null;

        foreach (Transform child in detonator.GetComponentInChildren<Transform>())
        {
            if (child.name.ToLower().Contains("plunger"))
                plunger = child.gameObject;
        }

        if (plunger == null)
            Debug.LogError("Can't find the detonator plunger!");

        if (attacker.transform.position.x > defender.transform.position.x) 
        {
            dynamite.transform.position = defender.interactionCharacter.transform.position + new Vector3(2, 0, 0);
            detonator.transform.position = attacker.interactionCharacter.transform.position + new Vector3(-1.8f, .345f, 0);

            detonator.transform.localEulerAngles = new Vector3(0, 90, 0); 
        }
        else
        {
            dynamite.transform.position = defender.interactionCharacter.transform.position + new Vector3(-2, 0, 0);
            detonator.transform.position = attacker.interactionCharacter.transform.position + new Vector3(1.8f, .345f, 0);

            detonator.transform.localEulerAngles = new Vector3(0, -90, 0);
            dynamite.transform.localEulerAngles = new Vector3(0, 90, 0); 
        }
        
        yield return new WaitForSeconds(1.5f);
         
        plunger.gameObject.MoveTo(plunger.transform.position.yAdd(-0.7f)).Time(0.1f).Execute();

        yield return new WaitForSeconds(.8f);

        defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);
        LugusCamera.game.Shake(LugusCameraExtensions.ShakeAmount.HUGE, 1.1f);

        PropFactory.use.FreeProp(dynamite);
        PropFactory.use.FreeProp(detonator);

        yield return new WaitForSeconds(.3f);
	}
}
