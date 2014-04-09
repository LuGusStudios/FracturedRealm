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
		
		
		Runner.AnimationFireBool(FR.Target.BOTH, "turnLeft");
		
		yield return WaitForAnimationState.New( Runner.Numerator.interactionCharacter, "Base Layer.idle");
		
		Runner.Numerator.transform.eulerAngles = new Vector3(0, 30, 0);
		Runner.Denominator.transform.eulerAngles = new Vector3(0, 30, 0);
			
		Runner.AnimationSetBool(FR.Target.BOTH, "running", true);
		
		Runner.Numerator.gameObject.MoveTo( Receiver.Numerator.transform.position ).Time (2.0f).Execute();
		Runner.Denominator.gameObject.MoveTo( Receiver.Denominator.transform.position ).Time (2.0f).Execute();
		
		
		// wait until they arrive at the target
		yield return new WaitForSeconds(1.8f);
		
		Effect[] hits = EffectFactory.use.CreateEffects( FR.EffectType.FIRE_HIT );
		hits[0].transform.position = Receiver.Numerator.transform.position + new Vector3(0,50,-100);
		hits[1].transform.position = Receiver.Denominator.transform.position + new Vector3(0,50,-100);
		
		// wait untill the height of the hit effect (covering all)
		yield return new WaitForSeconds(0.2f);
		
		current.StartFraction.Numerator.Value = target.StartFraction.Numerator.Value;
		current.StartFraction.Denominator.Value = target.StartFraction.Denominator.Value;
		
		
		Runner.Numerator.NumberValueChanged();
		Runner.Denominator.NumberValueChanged();
		
		//CharacterFactory.use.ReplaceRenderer( Runner.Numerator,   current.StartFraction.Numerator );
		//CharacterFactory.use.ReplaceRenderer( Runner.Denominator, current.StartFraction.Denominator );
		
		Runner.Numerator.interactionCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		Runner.Denominator.interactionCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		
		CharacterFactory.use.FreeRenderer( Receiver );
		//CharacterFactory.use.FreeRenderer( Receiver.Numerator );
		//CharacterFactory.use.FreeRenderer( Receiver.Denominator );
		
		
		
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
