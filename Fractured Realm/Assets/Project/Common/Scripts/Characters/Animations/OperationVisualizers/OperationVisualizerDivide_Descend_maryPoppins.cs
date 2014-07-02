
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerDivide_Descend_maryPoppins : OperationVisualizerDivide_Descend
{
	public override FR.Animation AnimationType()
	{
		return FR.Animation.maryPoppins;
	}

	public override FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.IN_PROGRESS;
	}

	public override float ApproximateDuration()
	{
		return -1.0f; // number < 0 means no length is known
	}
	
	public override float TimeToTransition()
	{
		return 3.95751f + 1.0f /*rotate duration estimate*/ + 3.3f; 
	}
	
	
	public override IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer original, NumberRenderer second)
	{
		if( part.HasDenominator() )
		{
			Debug.LogError("Descend_waveAndDrop:VisualizeAnimationPart : shouldn't be called with Target part having Denominator!!!");
			yield break;
		}

		if( animationPartDelay > 0.0f )
			yield return new WaitForSeconds( animationPartDelay );

		// FIXME: current setup doesn't work if number > 6!! (floating 6 stays on screen and moves weirdly)
		// suggestion: don't play animation on floater, but parent it to head of interactionCharacter (or custom animation moving down the hole)

		Vector3 holeOffset = new Vector3(3.5f, 0, 0); 

		// spawn hole a bit to the right
		BlackHole hole = this.blackHoleController.RequestBlackHole(original.interactionCharacter.transform.position + holeOffset );

		//yield return original.Animator.RotateInDirection( Vector3.right ).Coroutine; // maryPoppins animation starts facing the hole
		yield return original.Animator.RotateTowards( hole.renderer.transform.position ).Coroutine;
	
		original.Animator.CrossFade( this.AnimationType() );


        yield return new WaitForSeconds(0.735f); 

        Prop umbrella = PropFactory.use.CreateProp(FR.PropType.Umbrella);  
        umbrella.transform.parent = original.interactionCharacter.LeftHand;
        umbrella.transform.localPosition = new Vector3(-0.3172049f, -0.1107331f, 0.0134073f);
        umbrella.transform.localEulerAngles = new Vector3(342.0453f, 166.0374f, 268.3725f);

        
        foreach (Transform child in umbrella.transform)
        {
            if (child.gameObject.name.Contains("rotator"))
            {
                child.localEulerAngles += new Vector3(0, 0, 40);
            }
        }

        yield return new WaitForSeconds(1f);

        foreach (Transform child in umbrella.transform)
        {
            if (child.gameObject.name.Contains("rotator"))
            {
                child.gameObject.RotateTo(new Vector3(child.localEulerAngles.x, child.localEulerAngles.y, 0)).Time(.854f).EaseType(iTween.EaseType.easeOutElastic).Execute();
            }
        }



		
		yield return new WaitForSeconds( 7.467f ); //TimeToTransition() );
		
		// move original off screen down (through the black hole)
		//original.gameObject.MoveTo( original.transform.position.yAdd( -6.0f ) ).Time (0.5f).Execute();
		
		yield return new WaitForSeconds(1.0f);
		
		hole.Free();
		second.gameObject.transform.position += holeOffset;

        // this will eventually be a random next animation -> public virtual FR.Animation GetRandomNextAnimation()
		second.Animator.CrossFade( FR.Animation.ironManFall );



		second.gameObject.MoveTo( animationPartTarget + holeOffset ).Time (0.45f).Execute();
		
		yield return new WaitForSeconds(1.6f);
	}
}
