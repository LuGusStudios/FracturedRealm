using UnityEngine;
using System.Collections;

public class OperationVisualizerAdd : IOperationVisualizer 
{
	public OperationVisualizerAdd() 
	{
		Reset();
	}
	
	public override void Reset()
	{
		this.type = FR.OperationType.ADD; 
	}
	
	public override IEnumerator Visualize(OperationState current, OperationState target)
	{ 
		Debug.Log ("OperationVisualizerAdd : Visualize : " + current + " TO " + target);
		
		
		FractionRenderer Starter = current.StartFraction.Renderer;
		FractionRenderer Receiver = current.StopFraction.Renderer;

		yield return LugusCoroutines.use.StartRoutine( VisualizeAnimation(Starter, Receiver) ).Coroutine;
		
		// wait until the height of the hit effect (covering all)
		yield return new WaitForSeconds(0.2f);

		Starter.Fraction.Numerator.Value = target.StartFraction.Numerator.Value;
		Starter.Fraction.Denominator.Value = target.StartFraction.Denominator.Value;

		if( Starter.Fraction.Numerator.Value != 0 )
			Starter.Numerator.NumberValueChanged();
		
		if( Starter.Fraction.Denominator.Value != 0 )
			Starter.Denominator.NumberValueChanged();

		RendererFactory.use.FreeRenderer( Receiver );

		yield return Starter.Animator.RotateTowardsCamera().Coroutine;

		CheckOutcome(current, target);
		
		Debug.Log ("OperationVisualizerAdd : finished");
		yield break; 
		
	}


	public override IEnumerator VisualizeAnimation(FractionRenderer Starter, FractionRenderer Receiver)
	{
		LugusCoroutineWaiter waiter = new LugusCoroutineWaiter();
		
		if( Starter.Fraction.HasNumerator() )
		{
			waiter.Add( LugusCoroutines.use.StartRoutine( VisualizeAnimationPart(FR.Target.NUMERATOR, Starter.Numerator, Receiver.Numerator) ));
		}
		
		if( Starter.Fraction.HasDenominator() )
		{
			waiter.Add ( LugusCoroutines.use.StartRoutine( VisualizeAnimationPart(FR.Target.DENOMINATOR, Starter.Denominator, Receiver.Denominator) ));
		}
		
		yield return waiter.Start().Coroutine;
	}


	public override IEnumerator VisualizeAnimationPart( FR.Target part, NumberRenderer Starter, NumberRenderer Receiver )
	{
		yield return LugusCoroutineUtil.WaitForFinish( 
                      Starter.Animator.RotateTowards( Receiver ),
		              Receiver.Animator.RotateTowards( Starter ) 
                      ).Coroutine;

		Starter.Animator.RunTo( Receiver );
		
		// wait until they arrive at the target (takes 2 seconds)
		yield return new WaitForSeconds(1.8f);

		Receiver.Animator.SpawnEffect( FR.EffectType.JOIN_HIT );
	}
}
