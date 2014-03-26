using UnityEngine;
using System.Collections;

public class GraphicsManager : MonoBehaviour 
{
	public static void PositionElement(Transform element, int positionIndex)
	{
		Vector3 target = Vector3.zero;
		/*
		if( positionIndex == 0 )
			target = new Vector3(-13.62886f, -3.432834f, -0.7510824f);
		else if( positionIndex == 1 )
			target = new Vector3(-13.62886f, -3.432834f + -106.1185f, -0.7510824f);
		if( positionIndex == 2 )
			target = new Vector3(16.40057f, -3.432834f, -0.7510824f);
		else if( positionIndex == 3 )
			target = new Vector3(16.40057f, -3.432834f + -106.1185f, -0.7510824f);
		*/
		
		if( positionIndex == 0 )
			target = new Vector3(-10.06708f, -56.77174f, 0.0f);
		else if( positionIndex == 1 )
			target = new Vector3(10.06708f, -56.77174f, 0.0f);
		
		element.position = target;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
