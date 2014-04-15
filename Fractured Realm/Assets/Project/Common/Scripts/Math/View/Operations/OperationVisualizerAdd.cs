using UnityEngine;
using System.Collections;

public class OperationVisualizerAdd : IOperationVisualizer 
{
	public OperationVisualizerAdd() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.ADD; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerAdd : Visualize : " + current + " TO " + target);
		
		
		FractionRenderer Runner = current.StartFraction.Renderer;
		FractionRenderer Receiver = current.StopFraction.Renderer;


		//Runner.AnimationFireBool(FR.Target.BOTH, "turnLeft");
		//yield return WaitForAnimationState.New( Runner.Numerator.interactionCharacter, "Base Layer.idle");


		yield return LugusCoroutineUtil.WaitForFinish( 
                      	Runner.Animator.RotateTowards( FR.Target.BOTH, Receiver ),
		                Receiver.Animator.RotateTowards( FR.Target.BOTH,  Runner ) 
              		);


			
		//Runner.AnimationSetBool(FR.Target.BOTH, "running", true);
		
		//Runner.Numerator.gameObject.MoveTo( Receiver.Numerator.transform.position ).Time (2.0f).Execute();
		//Runner.Denominator.gameObject.MoveTo( Receiver.Denominator.transform.position ).Time (2.0f).Execute();

		Runner.Animator.MoveTo( FR.Target.BOTH,  Receiver );
		
		// wait until they arrive at the target
		yield return new WaitForSeconds(1.8f);

		Receiver.Animator.SpawnEffect( FR.Target.BOTH, FR.EffectType.JOIN_HIT );
		
		// wait untill the height of the hit effect (covering all)
		yield return new WaitForSeconds(0.2f);
		
		current.StartFraction.Numerator.Value = target.StartFraction.Numerator.Value;
		current.StartFraction.Denominator.Value = target.StartFraction.Denominator.Value;
		
		if( Runner.Numerator.Number.Value != 0 )
			Runner.Numerator.NumberValueChanged();
		
		if( Runner.Denominator.Number.Value != 0 )
			Runner.Denominator.NumberValueChanged();
		
		//CharacterFactory.use.ReplaceRenderer( Runner.Numerator,   current.StartFraction.Numerator );
		//CharacterFactory.use.ReplaceRenderer( Runner.Denominator, current.StartFraction.Denominator );
		
		//Runner.Numerator.interactionCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		//Runner.Denominator.interactionCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		
		CharacterFactory.use.FreeRenderer( Receiver );
		//CharacterFactory.use.FreeRenderer( Receiver.Numerator );
		//CharacterFactory.use.FreeRenderer( Receiver.Denominator );
		
		Runner.Animator.RotateTowardsCamera();

		
		/* OLD VERSION : just for reference!

		// TODO: deselect characters if need be
		
		current.StartFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool(current.StartFraction.Numerator.Renderer, "TurnLeft");
		current.StartFraction.Denominator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool(current.StartFraction.Denominator.Renderer, "TurnLeft");
		
		yield return new WaitForSeconds(0.3f);
		
		bool waiting = true;
		while( waiting )
		{
			if( current.StartFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.Idle") )
				waiting = false;
			
			yield return null;
		}
		
		//yield return new WaitForSeconds(1.1f); 
		
		current.StartFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().SetBool("Running", true);
		current.StartFraction.Denominator.Renderer.interactionCharacter.GetComponent<Animator>().SetBool("Running", true);
		
		//yield return new WaitForSeconds(0.5f);
		
		current.StartFraction.Numerator.Renderer.gameObject.MoveTo( current.StopFraction.Numerator.Renderer.transform.position ).Time(2.0f).Execute();
		current.StartFraction.Denominator.Renderer.gameObject.MoveTo( current.StopFraction.Denominator.Renderer.transform.position ).Time(2.0f).Execute();
		
		yield return new WaitForSeconds(1.8f);
		
		
		Effect[] effects = EffectFactory.use.CreateEffects( FR.EffectType.FIRE_HIT );
		effects[0].transform.position = current.StopFraction.Numerator.Renderer.transform.position + new Vector3(0,50,-100);
		effects[1].transform.position = current.StopFraction.Denominator.Renderer.transform.position + new Vector3(0,50,-100);
		
		yield return new WaitForSeconds(0.2f);
		
		current.StartFraction.Numerator.Value = target.StartFraction.Numerator.Value;
		current.StartFraction.Denominator.Value = target.StartFraction.Denominator.Value;
		
		CharacterFactory.use.ReplaceRenderer( current.StartFraction.Numerator.Renderer, current.StartFraction.Numerator );
		CharacterFactory.use.ReplaceRenderer( current.StartFraction.Denominator.Renderer, current.StartFraction.Denominator );
		
		current.StartFraction.Numerator.Renderer.interactionCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		current.StartFraction.Denominator.Renderer.interactionCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		
		CharacterFactory.use.FreeRenderer( current.StopFraction.Numerator.Renderer );
		CharacterFactory.use.FreeRenderer( current.StopFraction.Denominator.Renderer );
		*/
		
		CheckOutcome(current, target);
		
		Debug.Log ("OperationVisualizerAdd : finished");
		yield break; 
		
	}
}
