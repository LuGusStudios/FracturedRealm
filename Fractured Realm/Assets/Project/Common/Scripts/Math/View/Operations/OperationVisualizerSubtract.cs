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

	protected IEnumerator VisualizeDenominatorsOnly(OperationState current, OperationState target )
	{
		IOperationVisualizer addViz = MathManager.use.GetVisualizer( FR.OperationType.ADD );
		
		yield return LugusCoroutines.use.StartRoutine( addViz.Visualize(current, target) ).Coroutine;
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSubtract : finished version for just Denominators");
	}

	protected IEnumerator VisualizeSingleAttack( FractionRenderer Attacker, FractionRenderer Defender )
	{
		yield return Attacker.Animator.RotateTowards( FR.Target.BOTH, Defender ).Coroutine;
		
		Attacker.Animator.CrossFade(FR.Target.NUMERATOR, FR.Animation.castFireball); 
		
		yield return new WaitForSeconds(1.0f);
		
		Effect fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );

		fireball.transform.position = Attacker.Numerator.interactionCharacter.LeftHand.transform.position;
		fireball.gameObject.MoveTo( Defender.Numerator.interactionCharacter.Head.transform.position).Time(1.0f).Execute();
		
		yield return new WaitForSeconds(1.0f);

		fireball.Free ();

		Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
		hit.transform.position = Defender.Numerator.interactionCharacter.Head.transform.position + new Vector3(0,0.0f,-1.0f);
		
		yield return new WaitForSeconds(0.2f);
		
		
		Defender.Numerator.Number.Value -= 1;
		Defender.Numerator.NumberValueChanged(); // will automatically freeRenderer on Numerator on smallest on the last attack

		
		if( Defender.Numerator.Number.Value < 1 )
		{ 
			RendererFactory.use.FreeRenderer( Defender.Denominator );
		}
	}

	public override IEnumerator Visualize(OperationState current, OperationState target)
	{
		Debug.Log ("OperationVisualizerSubtract : Visualize : " + current + " TO " + target);
		
		
		FractionRenderer Smallest = current.StartFraction.Renderer;
		FractionRenderer Biggest = current.StopFraction.Renderer;
		
		// in the case where we only have denominators (both are automatically the same)
		// we have to do an alternative animation, since they won't fight each other
		// for now, we just use the default Add behaviour in this case
		// TODO: change this to a specific subtract animation to make the difference clear
		
		// note: no need to check for Biggest to have Numerator -> both are of the same type or wouldn't pass IOperation validation in the first place
		if( !FRTargetExtensions.TargetFromFraction(Smallest.Fraction).HasNumerator() )
		{
			yield return LugusCoroutines.use.StartRoutine( VisualizeDenominatorsOnly(current, target) ).Coroutine;
			yield break;
		}
		
		
		
		
		if( Biggest.Numerator.Number.Value < Smallest.Numerator.Number.Value )
		{
			FractionRenderer temp = Smallest;
			Smallest = Biggest;
			Biggest = temp;
		}

		while( Biggest.Numerator.Number.Value > target.StartFraction.Numerator.Value )
		{
			yield return LugusCoroutines.use.StartRoutine( VisualizeSingleAttack(Smallest, Biggest) ).Coroutine;

			yield return LugusCoroutines.use.StartRoutine( VisualizeSingleAttack(Biggest, Smallest) ).Coroutine;

			if( Smallest.Fraction.Numerator.Value < 1 )
			{
				// we're done!
				// should break automatically... biggest.Value = startFraction.Value
			}

		}

		
		//yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
		
		yield return Biggest.Animator.RotateTowardsCamera().Coroutine;

		
		current.StartFraction.Numerator.Value = Biggest.Fraction.Numerator.Value;
		current.StartFraction.Denominator.Value = Biggest.Fraction.Denominator.Value;
		
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSubtract : finished");
		yield break; 
	}


	public IEnumerator VisualizeOLD(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerSubtract : Visualize : " + current + " TO " + target);


		FractionRenderer Smallest = current.StartFraction.Renderer;
		FractionRenderer Biggest = current.StopFraction.Renderer;

		// in the case where we only have denominators (both are automatically the same)
		// we have to do an alternative animation, since they won't fight each other
		// for now, we just use the default Add behaviour in this case
		// TODO: change this to a specific subtract animation to make the difference clear

		// note: no need to check for Biggest to have Numerator -> both are of the same type or wouldn't pass IOperation validation in the first place
		if( !FRTargetExtensions.TargetFromFraction(Smallest.Fraction).HasNumerator() )
		{
			yield return LugusCoroutines.use.StartRoutine( VisualizeDenominatorsOnly(current, target) ).Coroutine;
			yield break;
		}



		
		if( Biggest.Numerator.Number.Value < Smallest.Numerator.Number.Value )
		{
			FractionRenderer temp = Smallest;
			Smallest = Biggest;
			Biggest = temp;
		}
		
		// TODO: deselect characters if need be 
		

		yield return Smallest.Animator.RotateTowards( FR.Target.BOTH, Biggest ).Coroutine;

		Smallest.Animator.CrossFade(FR.Target.NUMERATOR, FR.Animation.castFireball); 

		yield return new WaitForSeconds(1.0f);
		
		Effect fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );
		// TODO: make start at hand-palm
		fireball.transform.position = Smallest.Numerator.transform.position + new Vector3(0, 0.5f, 0);
		fireball.gameObject.MoveTo( Biggest.Numerator.transform.position + new Vector3(0, 0.5f, 0)).Time(1.0f).Execute();
		
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
			hit.transform.position = Biggest.Numerator.transform.position + new Vector3(0,0.5f,-1.0f);
			
			yield return new WaitForSeconds(0.2f);
			
			
			Biggest.Numerator.Number.Value -= 1;
			Biggest.Numerator.NumberValueChanged();
			//CharacterFactory.use.ReplaceRenderer( Biggest.Numerator, Biggest.Numerator.Number);
			Debug.LogError("Subtract : New Biggest = " + Biggest.Numerator.Number.Value + " ?= " + target.StartFraction.Numerator.Value);
			
			if( firstTime )
			{
				firstTime = false;
				
				//Biggest.AnimationFireBool(FR.Target.BOTH, "turnLeft");

				yield return Biggest.Animator.RotateTowards( FR.Target.BOTH, Smallest ).Coroutine;

				//WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
				
				//Biggest.Numerator.transform.eulerAngles = new Vector3(0, 30, 0);
				//Biggest.Denominator.transform.eulerAngles = new Vector3(0, 30, 0);
			}
			
			
			// 1. Biggest attacks : sends fireball to smallest
			//Biggest.AnimationFireBool(FR.Target.NUMERATOR, "CastFireball");
			
			Biggest.Animator.CrossFade(FR.Target.NUMERATOR, FR.Animation.castFireball); 

			yield return new WaitForSeconds(1.0f);
			
			fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );
			// TODO: make start at hand-palm
			fireball.transform.position = Biggest.Numerator.transform.position + new Vector3(0, 0.5f, 0);
			fireball.transform.eulerAngles = new Vector3(0, 90, 0);
			fireball.gameObject.MoveTo( Smallest.Numerator.transform.position + new Vector3(0, 0.5f, 0)).Time(1.0f).Execute();
			
			yield return new WaitForSeconds(1.0f);
			
			
			// 2. fireball arrives at smallest : he becomes smaller
			fireball.Free ();
		
			hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Smallest.Numerator.transform.position + new Vector3(0, 0.5f, -1.0f);
			
			yield return new WaitForSeconds(0.2f);
			
			Smallest.Numerator.Number.Value -= 1; 

			if( Smallest.Numerator.Number.Value < 1 )
			{
				RendererFactory.use.FreeRenderer( Smallest );
			}
			else
			{ 
				Smallest.Numerator.NumberValueChanged();
				//CharacterFactory.use.ReplaceRenderer( Smallest.Numerator, Smallest.Numerator.Number);
				Debug.LogError("Subtract : New Smallest = " + Smallest.Numerator.Number.Value);
				
				// 3. Smallest attacks : sends fireball to biggest
				//Smallest.AnimationFireBool(FR.Target.NUMERATOR, "CastFireball");
			 
				Smallest.Animator.CrossFade(FR.Target.NUMERATOR, FR.Animation.castFireball); 

				yield return new WaitForSeconds(1.0f); 
				
				fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );
				// TODO: make start at hand-palm
				fireball.transform.position = Smallest.Numerator.transform.position + new Vector3(0, 60, 0);
				fireball.transform.eulerAngles = new Vector3(0, 270, 0);
				fireball.gameObject.MoveTo( Biggest.Numerator.transform.position + new Vector3(0, 60, 0)).Time(1.0f).Execute();
				
				yield return new WaitForSeconds(1.0f);
			} 
		}
		 
		//yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");

		yield return Biggest.Animator.RotateTowardsCamera().Coroutine;

		Debug.Log ("OperationVisualizerSubtract : finished 1  :turning right");
		//Biggest.AnimationFireBool(FR.Target.BOTH, "turnRight");
		//yield return WaitForAnimationState.New(Biggest.Numerator.interactionCharacter, "Base Layer.Idle");
	
		current.StartFraction.Numerator.Value = Biggest.Fraction.Numerator.Value;
		current.StartFraction.Denominator.Value = Biggest.Fraction.Denominator.Value;


		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSubtract : finished");
		yield break; 
	}
}
