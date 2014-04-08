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
		
		// FIXME: TODO: find a decent object to start coroutines on!! LuGusInput is just temporary
		
		// can't do this in 1 coroutine, because one of both might run longer than other (too complex state keeping...)
		LugusInput.use.StartCoroutine( VisualizeNumerator(Starter, Receiver) );
		LugusInput.use.StartCoroutine( VisualizeDenominator(Starter, Receiver) );
		
		while( !numeratorDone || !denominatorDone )
		{
			yield return null;
		}
		
		CheckOutcome(current, target);
		Debug.Log ("OperationVisualizerMultiply : finished");
		yield break; 
	}
				
	protected IEnumerator VisualizeNumerator(FractionRenderer Starter, FractionRenderer Receiver)
	{
		int originalReceiverValue = Receiver.Numerator.Number.Value;
		
		while( Starter.Numerator.Number.Value > 0 )
		{
			Effect hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Starter.Numerator.transform.position + new Vector3(0,50,-100);
			
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
			Character Runner = CharacterFactory.use.CreateCharacter( new Number(originalReceiverValue), originalReceiverValue );
			Runner.transform.eulerAngles = new Vector3(0, 230, 0);
			Runner.transform.position = Starter.Numerator.transform.position;
			
			Runner.GetComponent<Animator>().SetBool("running", true); 
			Runner.gameObject.MoveTo( Receiver.Numerator.transform.position ).Time (2.0f).Execute();
		
		
			// wait until they arrive at the target
			yield return new WaitForSeconds(1.8f);
			
			
			hit = EffectFactory.use.CreateEffectNormal( FR.EffectType.FIRE_HIT );
			hit.transform.position = Receiver.Numerator.transform.position + new Vector3(0,50,-100);
		
			// wait untill the height of the hit effect (covering all)
			yield return new WaitForSeconds(0.2f);
			
			Receiver.Numerator.Number.Value += originalReceiverValue;//Runner.Number.Value;
			Receiver.Numerator.NumberValueChanged();
		
			CharacterFactory.use.FreeCharacter( Runner );
			
			// just some animation breathing time
			yield return new WaitForSeconds(1.0f);
		}
		
		numeratorDone = true;
		
		yield break;		
	}
			
	protected IEnumerator VisualizeDenominator(FractionRenderer Starter, FractionRenderer Receiver)
	{
		int originalReceiverValue = Receiver.Denominator.Number.Value;
		
		while( Starter.Denominator.Number.Value > 0 )
		{
			Effect hit = EffectFactory.use.CreateEffectSpirit( FR.EffectType.FIRE_HIT );
			hit.transform.position = Starter.Denominator.transform.position + new Vector3(0,50,-100);
			
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
			hit.transform.position = Receiver.Denominator.transform.position + new Vector3(0,50,-100);
		
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
}
