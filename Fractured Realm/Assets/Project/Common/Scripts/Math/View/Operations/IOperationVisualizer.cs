using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR 
{
	public enum VisualizerImplementationStatus
	{
		NONE = -1,

		IN_PROGRESS = 0,
		TESTING = 1,
		IMPLEMENTED = 2
	}
}

public class IOperationVisualizer
{
	public IOperationVisualizer()
	{
		
	}

	public virtual void Reset()
	{

	}

	public virtual FR.VisualizerImplementationStatus GetImplementationStatus()
	{
		return FR.VisualizerImplementationStatus.NONE;
	}

	public virtual FR.Animation AnimationType()
	{
		return FR.Animation.NONE;
	}

	// returns a number < 0 if length is not known / should not be used
	public virtual float ApproximateDuration()
	{
		return -1.0f; 
	}
	
	protected List<FR.Animation> _nextAnimations = null;
	public List<FR.Animation> NextAnimations
	{
		get
		{
			if( _nextAnimations == null )
			{
				PrepareNextAnimations();
			}
			
			return _nextAnimations;
		}
	}
	
	public virtual FR.Animation GetRandomNextAnimation()
	{
		if( NextAnimations == null || NextAnimations.Count == 0 )
		{
			return FR.Animation.NONE;
		}
		
		return NextAnimations[ UnityEngine.Random.Range(0, NextAnimations.Count) ];
	}


	protected virtual void PrepareNextAnimations(){ _nextAnimations = new List<FR.Animation>(); }

	
	public FR.OperationType type = FR.OperationType.NONE;


	public virtual IEnumerator Visualize(OperationState current, OperationState target){ yield break; }
	public virtual IEnumerator VisualizeAnimation(FractionRenderer starter, FractionRenderer receiver){ yield break; }
	public virtual IEnumerator VisualizeAnimationPart(FR.Target part, NumberRenderer starter, NumberRenderer receiver){ yield break; }
	
	protected virtual void CheckOutcome(OperationState outcome, OperationState target)
	{
		if( outcome.StartFraction.Numerator.Value == target.StartFraction.Numerator.Value &&
			outcome.StartFraction.Denominator.Value == target.StartFraction.Denominator.Value )
		{
			Debug.Log ("IOPerationVisualiser:CheckOutcome : " + this.type + " : outcome validated :)");
		}
		else
		{
			Debug.LogError("IOperationVisualizer:CheckOutcome : " + this.type + " : incorrect outcome! " + outcome + " != " + target);
		}
	}
}
