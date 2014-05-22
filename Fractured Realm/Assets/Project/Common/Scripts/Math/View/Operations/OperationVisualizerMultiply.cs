using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationVisualizerMultiply : IOperationVisualizer 
{
	public OperationVisualizerMultiply() 
	{
		Reset();
	}
	
	public override void Reset()
	{
		this.type = FR.OperationType.MULTIPLY; 
	}

	public List<FR.Animation> addAnimations = new List<FR.Animation>();
	public virtual FR.Animation GetRandomAddAnimation()
	{
		if( addAnimations.Count == 0 )
		{
			//addAnimations.Add( FR.Animation.highFiveLeftHand );
			addAnimations.Add( FR.Animation.highFiveRightHand );

			// TODO: FIXME: add extra options here!
		}

		return addAnimations[ Random.Range(0, addAnimations.Count) ];
	}


	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerMultiply : Visualize : " + current + " TO " + target);

		// NOTE : REVERSE LOGIC HERE!
		// TODO: SHould probably be refactored... but too error-prone to do right now...
		FractionRenderer Starter = current.StopFraction.Renderer;
		FractionRenderer Receiver = current.StartFraction.Renderer;

		// can't do this in 1 coroutine, because one of both might run longer than other (too complex state keeping...)
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();

		if( Starter.Fraction.Numerator.Value != 0 )
		{
			waiter.Add( LugusCoroutines.use.StartRoutine( VisualizeAnimationPart(FR.Target.NUMERATOR, Starter.Numerator, Receiver.Numerator) ));
		}

		if( Starter.Fraction.Denominator.Value != 0 )
		{
			waiter.Add ( LugusCoroutines.use.StartRoutine( VisualizeAnimationPart(FR.Target.DENOMINATOR, Starter.Denominator, Receiver.Denominator) ));
		}
		
		yield return waiter.Start().Coroutine;
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerMultiply : finished");
		yield break; 
	}

	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
		//Debug.LogError(Time.frameCount + " VisualizeAnimationPart multiply ");

		int originalReceiverValue = Receiver.Number.Value;

		yield return Receiver.Animator.RotateTowards( Starter ).Coroutine;
		
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
				RendererFactory.use.FreeRenderer(Starter);
				break;
			}
			
			// spawn new element

			// TODO: make sure we don't have to wait for 1 to arrive before spawning the other... (booooring otherwhise :))


			NumberRenderer Runner = RendererFactory.use.CreateRenderer( new Number(originalReceiverValue, null, part.HasNumerator() ) );
			Runner.transform.position = Starter.transform.position;
			Runner.transform.rotation = Starter.transform.rotation;

			Vector3 direction = Starter.transform.position - Receiver.transform.position;
			direction.Normalize();

			Runner.transform.position -= ( direction * 1.0f );
			
			Runner.Animator.RotateTowardsDirect( Receiver.transform.position );
			Runner.Animator.CrossFade( FR.Animation.running );


			// TODO: replace this part with a call to an appropriate VisualizerAdd()?
			/*

			Runner.Animator.RotateTowardsDirect( Receiver.transform.position );
			
			yield return Runner.Animator.RunTo( Receiver.transform.position ).Coroutine;


			Receiver.Number.Fraction.Renderer.Animator.SpawnEffect( side, FR.EffectType.FIRE_HIT );
			
			//hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			//hit.transform.position = Receiver.transform.position + new Vector3(0,0.5f,-1.0f);
			
			// wait untill the height of the hit effect (covering all)
			yield return new WaitForSeconds(0.2f);
			*/

			FR.Animation addAnim = GetRandomAddAnimation();
			FRAnimationData addAnimation = FRAnimations.use.animations[ (int) addAnim ];

			IOperationVisualizer visualizer = addAnimation.visualizer;

			yield return LugusCoroutines.use.StartRoutine( visualizer.VisualizeAnimationPart( part, Receiver, Runner ) ).Coroutine;



			Receiver.Number.Value += originalReceiverValue;
			Receiver = Receiver.NumberValueChanged();


			RendererFactory.use.FreeRenderer( Runner );
			
			// just some animation breathing time
			yield return new WaitForSeconds(1.0f);
		}

		Camera cam = null;
		if( part.HasNumerator() )
			cam = LugusCamera.numerator;
		else
			cam = LugusCamera.denominator;

		yield return Receiver.Animator.RotateTowards( cam.transform.position ).Coroutine;
		
		//Debug.LogError(Time.frameCount + " VisualizeAnimationPart multiply DONE ");
		
		yield break;	
	}

}
