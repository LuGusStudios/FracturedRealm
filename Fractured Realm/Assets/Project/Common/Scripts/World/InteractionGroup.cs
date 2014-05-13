using UnityEngine;
using System.Collections;

public class InteractionGroup : MonoBehaviour 
{
	public Transform Spawn1
	{
		get
		{
			return transform.FindChild("Spawn1");
		}
	}
	
	public Transform Spawn2
	{
		get
		{
			return transform.FindChild("Spawn2");
		}
	}

	public Transform Camera
	{
		get
		{
			return transform.FindChild("Camera");
		}
	}
	
	public Transform PortalEntry
	{
		get
		{
			return transform.FindChild("PortalEntry");
		}
	}
	
	public Vector3 PortalEntryCamera
	{
		get
		{
			Transform t = transform.FindChild("PortalEntryCamera");
			if( t != null )
				return t.position;
			else
				return PortalEntry.position.y(Camera.position.y).z( Camera.position.z );
		}
	}
	
	public Transform PortalExit
	{
		get
		{
			return transform.FindChild("PortalExit");
		}
	}
	
	public Vector3 PortalExitCamera
	{
		get
		{
			Transform t = transform.FindChild("PortalExitCamera");
			if( t != null )
				return t.position;
			else
				return PortalExit.position.y(Camera.position.y).z( Camera.position.z );
		}
	}
	
	void OnDrawGizmosSelected()
	{	
		/*
		if( this.defaultSceneStartPosition )
		{
			Gizmos.color = new Color (0,1,0,0.5f); 
			Gizmos.DrawCube ( transform.position.y( transform.position.y + 40 * scaleMultiplier), new Vector3 (40 * scaleMultiplier, 40 * scaleMultiplier, 40 * scaleMultiplier) );
		}
		
		if( this.exitToScene != null )
		{
		
			Gizmos.color = new Color (0,0,1,0.5f); 
			Gizmos.DrawCube ( transform.position, new Vector3 (40 * scaleMultiplier, 40 * scaleMultiplier, 40 * scaleMultiplier) );
			
			/*
			// we are an end node
			foreach( Waypoint neighbour in this.neighbours )
			{
				if( neighbour.exitToScene == this.exitToScene )
				{
					Gizmos.DrawLine( this.transform.position, neighbour.transform.position );
					
					foreach( Waypoint neighbour2 in neighbour.neighbours )
					{ 
						if( neighbour2.exitToScene == neighbour.exitToScene )
						{
							Gizmos.DrawLine( neighbour2.transform.position, neighbour.transform.position );
						}
					} 
				}
				
			}
			
		}
		else 
		{ */
		//if( !debug )
		//{
			Color[] colors = new Color[8];
			colors[0] = Color.black;
			colors[1] = Color.blue;
			colors[2] = Color.green;
			colors[3] = Color.red;
			colors[4] = Color.white;
			colors[5] = Color.cyan;
			colors[6] = Color.magenta;
			colors[7] = Color.yellow;
			
			DataRange zRange = new DataRange(500, -500);
			DataRange colorIndexRange = new DataRange(0, 7);
			
			float percentageZ = zRange.PercentageInInterval( transform.position.z /*layerOrder*/ );
			
			Gizmos.color = Color.red;//colors[ (int) colorIndexRange.ValueFromPercentage(percentageZ) ].a (0.5f); //new Color (layerOrder / 5.0f, 0, 0, 0.5f); 

		//}
		//else
		//	Gizmos.color = new Color(0,1,0,1.0f);

		foreach( Transform child in this.transform )
		{
			Gizmos.color = Color.red;

			if( child.name.Contains("Portal") )
				Gizmos.color = Color.cyan;
			if( child.name.Contains("Camera") )
				Gizmos.color = Color.blue;

			Gizmos.DrawCube ( child.position, new Vector3 (0.5f, 0.5f, 0.5f) );
		}
		
		//}
		
		//Gizmos.color = new Color (1,0,0,0.5f); 
		/*
		foreach( Waypoint neighbour in this.neighbours )
		{
			Gizmos.DrawLine( this.transform.position, neighbour.transform.position );
		}
		*/
	}
}
