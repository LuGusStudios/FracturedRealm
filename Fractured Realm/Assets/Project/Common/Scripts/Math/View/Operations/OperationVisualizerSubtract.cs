using UnityEngine;
using System.Collections;

public class OperationVisualizerSubtract : IOperationVisualizer 
{
	public OperationVisualizerSubtract() 
	{
		Reset();
	}
	
	public override void Reset()
	{
		this.type = FR.OperationType.SUBTRACT; 
	}

	protected virtual IEnumerator VisualizeDenominatorsOnly(OperationState current, OperationState target )
	{
		IOperationVisualizer addViz = MathManager.use.GetVisualizer( FR.OperationType.ADD );
		
		yield return LugusCoroutines.use.StartRoutine( addViz.Visualize(current, target) ).Coroutine;
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerSubtract : finished version for just Denominators");
	}


	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Attacker, NumberRenderer Defender )
	{
		yield return Attacker.Animator.RotateTowards( Defender ).Coroutine;

		Attacker.Animator.CrossFade( FR.Animation.castFireball );
		
		yield return new WaitForSeconds(1.0f);
		
		Effect fireball = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_BALL );

		fireball.transform.position = Attacker.interactionCharacter.LeftHand.transform.position;
		fireball.gameObject.MoveTo( Defender.interactionCharacter.Head.transform.position).Time(1.0f).Execute();
		
		yield return new WaitForSeconds(1.0f);

		fireball.Free ();

		Defender.Animator.SpawnEffect( FR.EffectType.FIRE_HIT );

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

		IOperationVisualizer visualizer = null;

		while( Biggest.Numerator.Number.Value > target.StartFraction.Numerator.Value )
		{
			
			visualizer = MathManager.use.GetVisualizer( FR.OperationType.SUBTRACT ); // new random visualizer

			yield return LugusCoroutines.use.StartRoutine( visualizer.VisualizeAnimationPart(FR.Target.NUMERATOR, Smallest.Numerator, Biggest.Numerator) ).Coroutine;

			Biggest.Numerator.Number.Value -= 1;
			Biggest.Numerator.NumberValueChanged(); // will automatically freeRenderer on Numerator on smallest on the last attack

			if( Biggest.Numerator.Number.Value < 1 )
			{ 
				Biggest.Denominator.Animator.SpawnEffect( FR.EffectType.FIRE_HIT );
				yield return new WaitForSeconds(0.2f);
				RendererFactory.use.FreeRenderer( Biggest.Denominator );
			}
			else
				yield return new WaitForSeconds(0.2f);
			




			visualizer = MathManager.use.GetVisualizer( FR.OperationType.SUBTRACT ); // new random visualizer
			
			yield return LugusCoroutines.use.StartRoutine( visualizer.VisualizeAnimationPart(FR.Target.NUMERATOR, Biggest.Numerator, Smallest.Numerator) ).Coroutine;

			Smallest.Numerator.Number.Value -= 1;
			Smallest.Numerator.NumberValueChanged(); // will automatically freeRenderer on Numerator on smallest on the last attack

			if( Smallest.Numerator.Number.Value < 1 )
			{ 
				Smallest.Denominator.Animator.SpawnEffect( FR.EffectType.FIRE_HIT );
				yield return new WaitForSeconds(0.2f);
				RendererFactory.use.FreeRenderer( Smallest.Denominator );
			}
			else
				yield return new WaitForSeconds(0.2f);



			//yield return LugusCoroutines.use.StartRoutine( VisualizeSingleAttack(Smallest, Biggest) ).Coroutine;

			//yield return LugusCoroutines.use.StartRoutine( VisualizeSingleAttack(Biggest, Smallest) ).Coroutine;

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
}
