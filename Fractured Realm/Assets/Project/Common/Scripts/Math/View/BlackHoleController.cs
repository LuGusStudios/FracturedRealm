using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHoleController 
{
	public List<BlackHole> holes = new List<BlackHole>();

	public BlackHole RequestBlackHole(Vector3 position)
	{
		// 2 options:
		// - position is close to an already open hole : re-use hole
		// - position is not close: create new hole

		BlackHole closestHole = null;
		float minDistance = float.MaxValue;

		foreach( BlackHole hole in holes )
		{
			float distance = Vector3.Distance( hole.renderer.transform.position, position );
			if( distance < minDistance )
			{
				minDistance = distance;
				closestHole = hole;
			}
		}

		if( minDistance < 1.0f )
		{
			//Debug.LogError("Hole position is too close to existing : RE-USE!");

			closestHole.useCount++;

			// new position is too close to an existing hole: just re-use that
			return closestHole;
		}
		else
		{
			//Debug.LogError("Hole position is far out! create new one");

			// position is far from existing holes (or the first hole) : create new one
			BlackHole hole = new BlackHole(position);
			hole.controller = this;

			hole.useCount++;

			holes.Add( hole );

			return hole;
		}
	}

	protected bool freeingAll = false;
	public void FreeHole(BlackHole hole)
	{
		//Debug.LogError("BlackHoleController : freeing hole! " + hole.renderer.transform.position);

		if( !freeingAll ) // shouldn't remove stuff from collection if we're looping over it
			holes.Remove( hole ); 
	}

	public void FreeAll()
	{
		freeingAll = true;

		foreach( BlackHole hole in holes )
		{
			hole.useCount = 0;
		}

		holes.Clear();

		freeingAll = false;
	}
}
