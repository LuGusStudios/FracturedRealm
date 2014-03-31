using UnityEngine;
using System.Collections;
using UnityEditorInternal;

public class AnimationControllerEditorHelper
{
}

public static class AnimationControllerExtensions
{
	public static void Clear(this UnityEditorInternal.AnimatorController ac)
	{
		int layerCount = ac.layerCount;
		for( int i = layerCount - 1; i >= 0; --i )
		{
			UnityEditorInternal.AnimatorControllerLayer layer = ac.GetLayer(i);
			
			layer.stateMachine.Clear ();

			if( i > 0 ) // don't remove Base Layer
			{
				ac.RemoveLayer(i);
			}
		}

		int parameterCount = ac.parameterCount;
		for( int j = parameterCount - 1; j >= 0; --j )
		{
			ac.RemoveParameter( j );
		}

	}

	
	public static void RepositionRadially(this UnityEditorInternal.AnimatorController ac)
	{
		int layerCount = ac.layerCount;
		for( int i = layerCount - 1; i >= 0; --i )
		{
			ac.GetLayer(i).stateMachine.RepositionRadially();
		}
	}
	
	public static void RepositionVertically(this UnityEditorInternal.AnimatorController ac)
	{
		int layerCount = ac.layerCount;
		for( int i = layerCount - 1; i >= 0; --i )
		{
			ac.GetLayer(i).stateMachine.RepositionVertically();
		}
	}
	
	public static void RepositionRadially(this UnityEditorInternal.StateMachine sm)
	{
		int stateCount = sm.stateCount;
		int machineCount = sm.stateMachineCount;

		int totalCount = stateCount + machineCount;
		float angleIncrement = 360.0f / (float) totalCount;
		int currentCount = 0;
		float angle = 0.0f;

		//Debug.LogError(sm.name + " : Angle increment : " + angleIncrement + ", #" + totalCount);

		DataRange radiusRange = new DataRange( 100, 250 );
		float radius = radiusRange.from;
		bool radiusFrom = true;
		
		float itemWidth = 204.0f / 2.0f;

		// position is around center point (up is negative y, below is positve y, left is negative x, right is positive x)


		sm.anyStatePosition = new Vector3(0,0,0);

		int i = 0;
		for( i = 0; i < stateCount; ++i )
		{
			angle = currentCount * angleIncrement;
			if( radiusFrom )
				radius = radiusRange.from;
			else
				radius = radiusRange.to;

			float cos = Mathf.Cos( Mathf.Deg2Rad * angle ) ;
			sm.GetState(i).position = new Vector3( (radius * cos) + (cos * itemWidth), radius * Mathf.Sin( Mathf.Deg2Rad * angle ), 0.0f );


			radiusFrom = !radiusFrom;
			++currentCount;
		}
		
		for( i = 0; i < machineCount; ++i )
		{
			angle = currentCount * angleIncrement;
			if( radiusFrom )
				radius = radiusRange.from;
			else
				radius = radiusRange.to;
			
			float cos = Mathf.Cos( Mathf.Deg2Rad * angle ) ;
			sm.SetStateMachinePosition(i, new Vector3( (radius * cos) + (cos * itemWidth), radius * Mathf.Sin( Mathf.Deg2Rad * angle ), 0.0f ) );
			sm.GetStateMachine(i).RepositionRadially();

			radiusFrom = !radiusFrom;
			++currentCount;
		}
		


	}
	
	public static void RepositionVertically(this UnityEditorInternal.StateMachine sm)
	{
		int stateCount = sm.stateCount;
		int machineCount = sm.stateMachineCount;

		float itemHeight = 36.0f;
		float itemWidth = 204.0f;
		
		// position is around center point (up is negative y, below is positve y, left is negative x, right is positive x)
		int totalCount = stateCount + machineCount;
		float topStart = (totalCount * itemHeight) / 2.0f * -1.0f;

		int currentCount = 0;

		sm.anyStatePosition = new Vector3(0,0,0);
		
		int i = 0;
		for( i = 0; i < stateCount; ++i )
		{
			sm.GetState(i).position = new Vector3( itemWidth, topStart + (currentCount * itemHeight * 1.5f), 0.0f );

			++currentCount;
		}
		
		for( i = 0; i < machineCount; ++i )
		{
			sm.SetStateMachinePosition(i, new Vector3( itemWidth, topStart + (currentCount * itemHeight * 1.5f), 0.0f) );
			sm.GetStateMachine(i).RepositionVertically();
			
			++currentCount;
		}
	}

	public static UnityEditorInternal.StateMachine FindSubStateMachine(this UnityEditorInternal.StateMachine sm, string subMachineName)
	{
		int machineCount = sm.stateMachineCount;
		for( int i = 0; i < machineCount; ++i )
		{
			//Debug.LogWarning("Checking submachine name : " + sm.GetStateMachine(i).name + " == " + subMachineName);
			if( sm.GetStateMachine(i).name == subMachineName )
			{
				return sm.GetStateMachine(i);
			}
		}

		return null;
	}

	public static void Clear(this UnityEditorInternal.StateMachine sm)
	{
		int stateCount = sm.stateCount;
		for( int i = stateCount - 1; i >= 0; --i )
		{
			sm.SetTransitionsFromState( sm.GetState(i), new Transition[]{} );
			sm.RemoveState( sm.GetState(i) );
		}

		// remove the transitions from the Any State nodes
		Transition[] anyStateTransitions = sm.GetTransitionsFromState(null);
		foreach( Transition t in anyStateTransitions )
		{
			sm.RemoveTransition( t );
		}

		// NOTE: This doesn't work for clearing any state
		//sm.SetTransitionsFromState( null, new Transition[]{} );

		int stateMachineCount = sm.stateMachineCount;
		for( int j = stateMachineCount - 1; j >= 0; --j )
		{
			sm.GetStateMachine(j).Clear();
			sm.RemoveStateMachine( sm.GetStateMachine(j) );
		}

		//int transitionCount = sm.Remove
	}
}


