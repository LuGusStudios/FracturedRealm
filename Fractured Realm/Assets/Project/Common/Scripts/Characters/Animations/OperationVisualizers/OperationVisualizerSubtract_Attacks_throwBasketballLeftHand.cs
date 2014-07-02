
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerSubtract_Attacks_throwBasketballLeftHand : OperationVisualizerSubtract_Attacks
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.throwBasketballLeftHand; 
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.TESTING;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer attacker, NumberRenderer defender )
	{
        yield return attacker.Animator.RotateTowards(defender).Coroutine;

        attacker.Animator.CrossFade(this.AnimationType());

        yield return new WaitForSeconds(0.6756f); // spawn ball

        Prop basketball = PropFactory.use.CreateProp(FR.PropType.Basketball);
        basketball.transform.parent = attacker.interactionCharacter.LeftHand;
        basketball.transform.localScale = new Vector3(0.9f, 0.7924063f, 0.9f);
        basketball.transform.localPosition = new Vector3(-0.5963913f, -1.190002f, 0.4890688f);

        yield return new WaitForSeconds(0.59f); // hand position first dribble 


        Debug.Log("parenting to world"); 
        basketball.transform.parent = null;  

        Vector3 upPosition = basketball.transform.position;
        Vector3 downPosition = new Vector3(basketball.transform.position.x, (basketball.transform.position.y - 1.1f), basketball.transform.position.z);

        float waitTime = 0.1f;
        float upTime = .1f;

        basketball.gameObject.RotateTo(new Vector3(-270, 23, 367)).Time(0.6f).Execute();

        basketball.gameObject.MoveTo(downPosition).Time(waitTime).Execute();
        yield return new WaitForSeconds(waitTime);
        basketball.gameObject.MoveTo(upPosition).Time(waitTime + upTime).Execute();

        yield return new WaitForSeconds(waitTime + upTime);
        basketball.gameObject.MoveTo(downPosition).Time(waitTime).Execute();
        yield return new WaitForSeconds(waitTime);
        basketball.gameObject.MoveTo(upPosition).Time(waitTime + upTime).Execute(); 

        yield return new WaitForSeconds(waitTime + upTime);
        basketball.gameObject.MoveTo(downPosition).Time(waitTime).Execute();
        yield return new WaitForSeconds(waitTime);
        basketball.gameObject.MoveTo(upPosition).Time(waitTime + upTime).Execute();

        yield return new WaitForSeconds(waitTime + upTime);

        basketball.transform.parent = attacker.interactionCharacter.LeftHand;

        yield return new WaitForSeconds(0.6f);

        basketball.transform.parent = null;

        Vector3[] pathDown = new Vector3[2];
        pathDown[0] = attacker.interactionCharacter.LeftHand.transform.position;
        pathDown[1] = Vector3.Lerp(pathDown[0], defender.interactionCharacter.Body.position, .61f) + new Vector3(0, -1.8f, 0);

        Vector3[] pathUp = new Vector3[2];
        pathUp[0] = pathDown[1];
        pathUp[1] = defender.interactionCharacter.Head.position;

        basketball.gameObject.RotateTo(new Vector3(-540, 23, 367)).Time(0.4f).Execute();

        basketball.gameObject.MoveTo(pathDown).MoveToPath(false).Time(.2f).EaseType(iTween.EaseType.linear).Execute();
        yield return new WaitForSeconds(.18f);
        basketball.gameObject.MoveTo(pathUp).MoveToPath(false).Time(.11f).EaseType(iTween.EaseType.linear).Execute();

        yield return new WaitForSeconds(.11f);


        defender.Animator.SpawnEffect(FR.EffectType.FIRE_HIT);
        PropFactory.use.FreeProp(basketball);



        yield return new WaitForSeconds(.3f);
	}
}
