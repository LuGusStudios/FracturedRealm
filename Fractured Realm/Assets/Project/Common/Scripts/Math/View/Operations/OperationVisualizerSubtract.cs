using UnityEngine;
using System.Collections;

public class OperationVisualizerSubtract : IOperationVisualizer 
{
	public OperationVisualizerSubtract() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.SUBTRACT; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerSubtract : Visualize : " + current + " TO " + target);
		
		FractionRenderer Smallest = current.StartFraction.Renderer;
		FractionRenderer Biggest = current.StopFraction.Renderer;
		
		if( Biggest.Numerator.Number.Value < Smallest.Numerator.Number.Value )
		{
			FractionRenderer temp = Smallest;
			Smallest = Biggest;
			Biggest = temp;
		}
		
		// TODO: deselect characters if need be 
		
		
		//OLD: current.StopFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("TurnRight");
		//OLD: current.StopFraction.Denominator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("TurnRight");
		
		Smallest.AnimationFireBool(FR.Target.BOTH, "turnRight");
		
		//OLD: yield return Stop.Numerator.StartCoroutine( Stop.AnimationWaitForState(Stop.Numerator.interactionCharacter, "Base Layer.Idle") );
		yield return WaitForAnimationState.New(Smallest.Numerator.interactionCharacter, "Base Layer.idle");
		
		Smallest.Numerator.transform.eulerAngles -= new Vector3(0, 30, 0);
		Smallest.Denominator.transform.eulerAngles -= new Vector3(0, 30, 0);
		
		//OLD: current.StopFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("CastFireball");
		Smallest.AnimationFireBool(FR.Target.NUMERATOR, "castFireball");
		
		yield return new WaitForSeconds(1.0f);
		
		Effect fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );
		// TODO: make start at hand-palm
		fireball.transform.position = Smallest.Numerator.transform.position + new Vector3(0, 60, 0);
		fireball.transform.eulerAngles = new Vector3(0, 270, 0);
		fireball.gameObject.MoveTo( Biggest.Numerator.transform.position + new Vector3(0, 60, 0)).Time(1.0f).Execute();
		
		yield return new WaitForSeconds(1.0f);
		
		
		
		/*
		fireball.Free ();
		
		Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
		hit.transform.position = Biggest.Numerator.transform.position + new Vector3(0,50,-100);
		
		yield return new WaitForSeconds(0.2f);
		
		
		
		// TODO: play hitleft ?
		//current.StartFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool(current.StartFraction.Numerator.Renderer, "TurnLeft");
		
		
		
		Biggest.Numerator.Number.Value -= 1;
		
		CharacterFactory.use.ReplaceRenderer( Biggest.Numerator, Biggest.Numerator.Number);
		
		
		// TODO: look angry (make function for this on Character... layer depends on character type)
		//current.StartFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("LookAngry");
		
		//OLD: current.StartFraction.Numerator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("TurnLeft");
		//OLD: current.StartFraction.Denominator.Renderer.interactionCharacter.GetComponent<Animator>().FireBool("TurnLeft");
		
		Biggest.AnimationFireBool(FR.Target.BOTH, "TurnLeft");
		yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
		*/
		
		bool firstTime = true; 
		while( Biggest.Numerator.Number.Value > target.StartFraction.Numerator.Value )
		{
			// 1. biggest attacks
			// 2. smallest value changes
			// 3. smallest attacks back
			// 4. biggest value changes
			
			
			// 4. fireball arrives at biggest : he becomes smaller	
			fireball.Free ();
		 
			Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Biggest.Numerator.transform.position + new Vector3(0,50,-100);
			
			yield return new WaitForSeconds(0.2f);
			
			
			Biggest.Numerator.Number.Value -= 1;
			Biggest.Numerator.NumberValueChanged();
			//CharacterFactory.use.ReplaceRenderer( Biggest.Numerator, Biggest.Numerator.Number);
			Debug.LogError("Subtract : New Biggest = " + Biggest.Numerator.Number.Value + " ?= " + target.StartFraction.Numerator.Value);
			
			if( firstTime )
			{
				firstTime = false;
				
				Biggest.AnimationFireBool(FR.Target.BOTH, "turnLeft");
				
				yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
				
				Biggest.Numerator.transform.eulerAngles = new Vector3(0, 30, 0);
				Biggest.Denominator.transform.eulerAngles = new Vector3(0, 30, 0);
			}
			
			
			// 1. Biggest attacks : sends fireball to smallest
			Biggest.AnimationFireBool(FR.Target.NUMERATOR, "CastFireball");
		
			yield return new WaitForSeconds(1.0f);
			
			fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );
			// TODO: make start at hand-palm
			fireball.transform.position = Biggest.Numerator.transform.position + new Vector3(0, 60, 0);
			fireball.transform.eulerAngles = new Vector3(0, 90, 0);
			fireball.gameObject.MoveTo( Smallest.Numerator.transform.position + new Vector3(0, 60, 0)).Time(1.0f).Execute();
			
			yield return new WaitForSeconds(1.0f);
			
			
			// 2. fireball arrives at smallest : he becomes smaller
			fireball.Free ();
		
			hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Smallest.Numerator.transform.position + new Vector3(0,50,-100);
			
			yield return new WaitForSeconds(0.2f);
			
			Smallest.Numerator.Number.Value -= 1; 
			
			if( Smallest.Numerator.Number.Value < 1 )
			{
				CharacterFactory.use.FreeRenderer( Smallest );
			}
			else
			{
				Smallest.Numerator.NumberValueChanged();
				//CharacterFactory.use.ReplaceRenderer( Smallest.Numerator, Smallest.Numerator.Number);
				Debug.LogError("Subtract : New Smallest = " + Smallest.Numerator.Number.Value);
				
				// 3. Smallest attacks : sends fireball to biggest
				Smallest.AnimationFireBool(FR.Target.NUMERATOR, "CastFireball");
			 
				yield return new WaitForSeconds(1.0f); 
				
				fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );
				// TODO: make start at hand-palm
				fireball.transform.position = Smallest.Numerator.transform.position + new Vector3(0, 60, 0);
				fireball.transform.eulerAngles = new Vector3(0, 270, 0);
				fireball.gameObject.MoveTo( Biggest.Numerator.transform.position + new Vector3(0, 60, 0)).Time(1.0f).Execute();
				
				yield return new WaitForSeconds(1.0f);
			} 
		}
		 
		yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
		
		Debug.Log ("OperationVisualizerSubtract : finished 1  :turning right");
		Biggest.AnimationFireBool(FR.Target.BOTH, "turnRight");
		yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
	
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSubtract : finished");
		yield break; 
	}
}
