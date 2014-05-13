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

		
		//Runner.Animator.RotateTowards( FR.Target.BOTH, Receiver );
		//yield return Receiver.Animator.RotateTowards( FR.Target.BOTH,  Runner );


		yield return LugusCoroutineUtil.WaitForFinish( 
                      	Runner.Animator.RotateTowards( FR.Target.BOTH, Receiver ),
		                Receiver.Animator.RotateTowards( FR.Target.BOTH,  Runner ) 
              		).Coroutine;




		Runner.Animator.MoveTo( FR.Target.BOTH,  Receiver );
		
		// wait until they arrive at the target
		yield return new WaitForSeconds(1.8f);

		Receiver.Animator.SpawnEffect( FR.Target.BOTH, FR.EffectType.JOIN_HIT );
		
		// wait untill the height of the hit effect (covering all)
		yield return new WaitForSeconds(0.2f);


		Runner.Fraction.Numerator.Value = target.StartFraction.Numerator.Value;
		Runner.Fraction.Denominator.Value = target.StartFraction.Denominator.Value;


		if( Runner.Fraction.Numerator.Value != 0 )
			Runner.Numerator.NumberValueChanged();
		
		if( Runner.Fraction.Denominator.Value != 0 )
			Runner.Denominator.NumberValueChanged();

		
		RendererFactory.use.FreeRenderer( Receiver );


		yield return Runner.Animator.RotateTowardsCamera().Coroutine;

		CheckOutcome(current, target);
		
		Debug.Log ("OperationVisualizerAdd : finished");
		yield break; 
		
	}
}
