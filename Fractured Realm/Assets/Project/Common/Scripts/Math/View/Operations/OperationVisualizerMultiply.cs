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

		FR.Animation addAnimation = addAnimations[ Random.Range(0, addAnimations.Count) ];

		Debug.Log ("VisualizerMultiply:RandomAddAnimation : chosen " + addAnimation );

		return addAnimation;
	}


	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerMultiply : Visualize : " + current + " TO " + target);


		FractionRenderer Starter = current.StartFraction.Renderer;
		FractionRenderer Receiver = current.StopFraction.Renderer;

		yield return LugusCoroutines.use.StartRoutine( VisualizeAnimation(Starter, Receiver) ).Coroutine;

		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerMultiply : finished");
		yield break; 
	}

	public override IEnumerator VisualizeAnimation( FractionRenderer Starter, FractionRenderer Receiver )
	{
		
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
	}


	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
		return VisualizeAnimationPartDefault(part, Starter, Receiver, FR.Animation.NONE, 0.0f);

	}

	public virtual IEnumerator VisualizeAnimationPartDefault( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver, FR.Animation animation, float delay = 1.0f )
	{
		
		int originalStarterValue = Starter.Number.Value;
		
		yield return Starter.Animator.RotateTowards( Receiver ).Coroutine;
		
		while( Receiver.Number.Value > 0 )
		{
			if( animation != FR.Animation.NONE )
			{
				Receiver.Animator.CrossFade(animation);
			}
			
			if( delay > 0.0f )
				yield return new WaitForSeconds(delay);
			
			//Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			//hit.transform.position = Starter.transform.position + new Vector3(0,0.5f,-1.0f);
			
			Receiver.Animator.SpawnEffect( FR.EffectType.FIRE_HIT );
			yield return new WaitForSeconds(0.2f);
			
			
			Receiver.Number.Value -= 1;
			if( Receiver.Number.Value > 0 )
			{
				Receiver = Receiver.NumberValueChanged();
			}
			else
			{
				// if we are at 1, we just disappear... sad, isn't it :(
				RendererFactory.use.FreeRenderer(Receiver);
				break;
			}

			//Debug.LogError("Receiver multiply turning to camera");
			Receiver.Animator.RotateTowardsCamera();
			
			// spawn new element
			
			// TODO: make sure we don't have to wait for 1 to arrive before spawning the other... (booooring otherwhise :))
			
			//Debug.LogError("Multiply : runner is numerator? " + Receiver.VisualizedAsNumerator + " for part " + part);
			NumberRenderer Runner = RendererFactory.use.CreateRenderer( new Number(originalStarterValue, null, Receiver.VisualizedAsNumerator) );//part.HasNumerator() ) );
			Runner.transform.position = Receiver.transform.position;
			Runner.transform.rotation = Receiver.transform.rotation;
			
			Vector3 direction = Receiver.transform.position - Starter.transform.position;
			direction.Normalize();
			
			Runner.transform.position -= ( direction * 1.0f );
			
			Runner.Animator.RotateTowardsDirect( Starter.transform.position );
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
			
			yield return LugusCoroutines.use.StartRoutine( visualizer.VisualizeAnimationPart( part, Starter, Runner ) ).Coroutine;
			
			Starter.Number.Value += originalStarterValue;
			Starter = Starter.NumberValueChanged();
			
			
			RendererFactory.use.FreeRenderer( Runner );
			
			// just some animation breathing time
			yield return new WaitForSeconds(1.0f);
		}
		
		yield return Starter.Animator.RotateTowardsCamera().Coroutine;
		
		//Debug.LogError(Time.frameCount + " VisualizeAnimationPart multiply DONE ");
		
		yield break;	
	}

}
