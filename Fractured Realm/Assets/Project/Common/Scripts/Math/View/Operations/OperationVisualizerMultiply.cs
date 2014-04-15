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
		
		// TODO: deselect characters if need be
		
		FractionRenderer Starter = current.StopFraction.Renderer;
		FractionRenderer Receiver = current.StartFraction.Renderer;

		// can't do this in 1 coroutine, because one of both might run longer than other (too complex state keeping...)
		if( Starter.Fraction.Numerator.Value != 0 )
		{
			//LugusCoroutines.use.StartRoutine( VisualizeNumerator(Starter, Receiver) );
			LugusCoroutines.use.StartRoutine( VisualizeSide(FR.Target.NUMERATOR, Starter.Numerator, Receiver.Numerator) );
		}

		if( Starter.Fraction.Denominator.Value != 0 )
		{
			//LugusCoroutines.use.StartRoutine( VisualizeDenominator(Starter, Receiver) );
			LugusCoroutines.use.StartRoutine( VisualizeSide(FR.Target.DENOMINATOR, Starter.Denominator, Receiver.Denominator) );
		}
		
		while( !numeratorDone || !denominatorDone )
		{
			yield return null;
		}
		
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
			
			// TODO: this should be NumberRenderer instead of Character... 
			/*
			Character Runner = CharacterFactory.use.CreateCharacter( new Number(originalReceiverValue), originalReceiverValue );
			Runner.transform.eulerAngles = new Vector3(0, 230, 0);
			Runner.transform.position = Starter.Numerator.transform.position;
			
			Runner.GetComponent<Animator>().SetBool("running", true); 
			Runner.gameObject.MoveTo( Receiver.Numerator.transform.position ).Time (2.0f).Execute();
			*/
			
			NumberRenderer Runner = CharacterFactory.use.CreateRenderer( new Number(originalReceiverValue, null, side.HasNumerator() ) );
			Runner.transform.position = Starter.transform.position;
			Runner.transform.rotation = Starter.transform.rotation;
			
			// face towards the Receiver
			// TODO: best replaced by for ex. Runner.Animator.RotateTowardsDirect( Receiver.transform.position )
			//CharacterOrientationInfo info = new CharacterOrientationInfo();
			//info.Fill( Runner.interactionCharacter.transform, Receiver.Numerator.interactionCharacter.transform.position );
			//Runner.transform.rotation = info.lookRotation;
			
			//Debug.LogError("OrInfo : " + info.angle + " // " + info.lookRotation.eulerAngles + " // " + info.targetDirection + " // " + info.animationDegrees + " // " + info.xPosition + " // " + Runner.interactionCharacter.transform.up );
			
			//yield return Runner.Animator.RotateTowards( Receiver.Numerator.transform.position );


			// TODO: replace this part with a call to an appropriate VisualizerAdd()?

			Runner.Animator.RotateTowardsDirect( Receiver.transform.position );
			
			yield return Runner.Animator.MoveTo( Receiver.transform.position ).Coroutine;
			
			
			// wait until they arrive at the target
			//yield return new WaitForSeconds(1.8f);
			
			
			hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Receiver.transform.position + new Vector3(0,0.5f,-1.0f);
			
			// wait untill the height of the hit effect (covering all)
			yield return new WaitForSeconds(0.2f);
			
			Receiver.Number.Value += originalReceiverValue;//Runner.Number.Value;
			Receiver = Receiver.NumberValueChanged();
			
			//CharacterFactory.use.FreeCharacter( Runner );
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

	/*
	protected IEnumerator VisualizeNumerator(FractionRenderer Starter, FractionRenderer Receiver)
	{
		int originalReceiverValue = Receiver.Numerator.Number.Value;
		
		while( Starter.Numerator.Number.Value > 0 )
		{
			Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Starter.Numerator.transform.position + new Vector3(0,0.5f,-1.0f);
			
			Starter.Numerator.Number.Value -= 1;
			if( Starter.Numerator.Number.Value > 0 )
			{
				Starter.Numerator.NumberValueChanged();
			}
			else
			{
				// if we are at 1, we just disappear... sad, isn't it :(
				CharacterFactory.use.FreeRenderer(Starter.Numerator);
				break;
			}
			
			// spawn new element
			// TODO: make sure we don't have to wait for 1 to arrive before spawning the other... (booooring otherwhise :))
			
			// TODO: this should be NumberRenderer instead of Character... 

			//Character Runner = CharacterFactory.use.CreateCharacter( new Number(originalReceiverValue), originalReceiverValue );
			//Runner.transform.eulerAngles = new Vector3(0, 230, 0);
			//Runner.transform.position = Starter.Numerator.transform.position;
			
			//Runner.GetComponent<Animator>().SetBool("running", true); 
			//Runner.gameObject.MoveTo( Receiver.Numerator.transform.position ).Time (2.0f).Execute();


			NumberRenderer Runner = CharacterFactory.use.CreateRenderer( new Number(originalReceiverValue) );
			Runner.transform.position = Starter.Numerator.transform.position;
			Runner.transform.rotation = Starter.Numerator.transform.rotation;

			// face towards the Receiver
			// TODO: best replaced by for ex. Runner.Animator.RotateTowardsDirect( Receiver.transform.position )
			//CharacterOrientationInfo info = new CharacterOrientationInfo();
			//info.Fill( Runner.interactionCharacter.transform, Receiver.Numerator.interactionCharacter.transform.position );
			//Runner.transform.rotation = info.lookRotation;

			//Debug.LogError("OrInfo : " + info.angle + " // " + info.lookRotation.eulerAngles + " // " + info.targetDirection + " // " + info.animationDegrees + " // " + info.xPosition + " // " + Runner.interactionCharacter.transform.up );

			//yield return Runner.Animator.RotateTowards( Receiver.Numerator.transform.position );

			Runner.Animator.RotateTowardsDirect( Receiver.Numerator.transform.position );

			yield return Runner.Animator.MoveTo( Receiver.Numerator.transform.position ).Coroutine;
		
		
			// wait until they arrive at the target
			//yield return new WaitForSeconds(1.8f);
			
			
			hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Receiver.Numerator.transform.position + new Vector3(0,50,-100);
		
			// wait untill the height of the hit effect (covering all)
			yield return new WaitForSeconds(0.2f);
			
			Receiver.Numerator.Number.Value += originalReceiverValue;//Runner.Number.Value;
			Receiver.Numerator.NumberValueChanged();
		
			//CharacterFactory.use.FreeCharacter( Runner );
			CharacterFactory.use.FreeRenderer( Runner );
			
			// just some animation breathing time
			yield return new WaitForSeconds(1.0f);
		}
		
		numeratorDone = true;
		
		yield break;		
	}
	*/

	/*
	protected IEnumerator VisualizeDenominator(FractionRenderer Starter, FractionRenderer Receiver)
	{
		int originalReceiverValue = Receiver.Denominator.Number.Value;
		
		while( Starter.Denominator.Number.Value > 0 )
		{
			Effect hit = EffectFactory.use.CreateEffectSpirit( FR.EffectType.FIRE_HIT );
			hit.transform.position = Starter.Denominator.transform.position + new Vector3(0,0.5f,-1.0f);
			
			Starter.Denominator.Number.Value -= 1;
			if( Starter.Denominator.Number.Value > 0 )
			{
				Starter.Denominator.NumberValueChanged();
			}
			else
			{
				// if we are at 1, we just disappear... sad, isn't it :(
				CharacterFactory.use.FreeRenderer(Starter.Denominator);
				break;
			}
			
			// spawn new element
			// TODO: make sure we don't have to wait for 1 to arrive before spawning the other... (booooring otherwhise :))
			
			// TODO: this should be NumberRenderer instead of Character... 
			Character Runner = CharacterFactory.use.CreateCharacter( new Number(originalReceiverValue, null, false), originalReceiverValue );
			Runner.transform.eulerAngles = new Vector3(0, 230, 0);
			Runner.transform.position = Starter.Denominator.transform.position;
			
			Runner.GetComponent<Animator>().SetBool("running", true);
			Runner.gameObject.MoveTo( Receiver.Denominator.transform.position ).Time (2.0f).Execute();
		
		
			// wait until they arrive at the target
			yield return new WaitForSeconds(1.8f);
			
			
			hit = EffectFactory.use.CreateEffectSpirit( FR.EffectType.FIRE_HIT );
			hit.transform.position = Receiver.Denominator.transform.position + new Vector3(0,0.5f,-1.0f);
		
			// wait untill the height of the hit effect (covering all)
			yield return new WaitForSeconds(0.2f);
			
			Receiver.Denominator.Number.Value += originalReceiverValue;//Runner.Number.Value;
			Receiver.Denominator.NumberValueChanged();
		
			CharacterFactory.use.FreeCharacter( Runner );
			
			// just some animation breathing time
			yield return new WaitForSeconds(1.0f);
		}
		
		denominatorDone = true;
		
		yield break;	
	}
	*/
}
