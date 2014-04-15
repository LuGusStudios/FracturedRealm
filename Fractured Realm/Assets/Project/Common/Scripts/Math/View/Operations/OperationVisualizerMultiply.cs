using UnityEngine;
using System.Collections;

public class OperationVisualizerMultiply : IOperationVisualizer 
{
	public OperationVisualizerMultiply() 
	{
		Reset();
	}
	
	public void Reset()
	{
		this.type = FR.OperationType.MULTIPLY; 
	}
	
	protected bool numeratorDone = false;
	protected bool denominatorDone = false;
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerMultiply : Visualize : " + current + " TO " + target);

		
		FractionRenderer Starter = current.StopFraction.Renderer;
		FractionRenderer Receiver = current.StartFraction.Renderer;

		// can't do this in 1 coroutine, because one of both might run longer than other (too complex state keeping...)
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();

		if( Starter.Fraction.Numerator.Value != 0 )
		{
			waiter.Add( LugusCoroutines.use.StartRoutine( VisualizeSide(FR.Target.NUMERATOR, Starter.Numerator, Receiver.Numerator) ));
		}

		if( Starter.Fraction.Denominator.Value != 0 )
		{
			waiter.Add ( LugusCoroutines.use.StartRoutine( VisualizeSide(FR.Target.DENOMINATOR, Starter.Denominator, Receiver.Denominator) ));
		}
		
		yield return waiter.Start().Coroutine;
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerMultiply : finished");
		yield break; 
	}

	protected IEnumerator VisualizeSide( FR.Target side, NumberRenderer Starter, NumberRenderer Receiver )
	{
		int originalReceiverValue = Receiver.Number.Value;
		
		while( Starter.Number.Value > 0 )
		{
			Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Starter.transform.position + new Vector3(0,0.5f,-1.0f);
			
			Starter.Number.Value -= 1;
			if( Starter.Number.Value > 0 )
			{
				Starter = Starter.NumberValueChanged();
			}
			else
			{
				// if we are at 1, we just disappear... sad, isn't it :(
				CharacterFactory.use.FreeRenderer(Starter);
				break;
			}
			
			// spawn new element

			// TODO: make sure we don't have to wait for 1 to arrive before spawning the other... (booooring otherwhise :))


			NumberRenderer Runner = CharacterFactory.use.CreateRenderer( new Number(originalReceiverValue, null, side.HasNumerator() ) );
			Runner.transform.position = Starter.transform.position;
			Runner.transform.rotation = Starter.transform.rotation;

			// TODO: replace this part with a call to an appropriate VisualizerAdd()?

			Runner.Animator.RotateTowardsDirect( Receiver.transform.position );
			
			yield return Runner.Animator.MoveTo( Receiver.transform.position ).Coroutine;


			Receiver.Number.Fraction.Renderer.Animator.SpawnEffect( side, FR.EffectType.FIRE_HIT );
			
			//hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			//hit.transform.position = Receiver.transform.position + new Vector3(0,0.5f,-1.0f);
			
			// wait untill the height of the hit effect (covering all)
			yield return new WaitForSeconds(0.2f);
			
			Receiver.Number.Value += originalReceiverValue;
			Receiver = Receiver.NumberValueChanged();


			CharacterFactory.use.FreeRenderer( Runner );
			
			// just some animation breathing time
			yield return new WaitForSeconds(1.0f);
		}

		if( side.HasNumerator() )
			numeratorDone = true;
		else if( side.HasDenominator() )
			denominatorDone = true;
		
		yield break;	
	}
}
