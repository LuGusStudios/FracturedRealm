using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorldPart : MonoBehaviour 
{
	protected InteractionGroup[] _interactionGroups;
	public InteractionGroup[] InteractionGroups
	{
		get
		{
			if( _interactionGroups == null || _interactionGroups.Length == 0 )
			{
				_interactionGroups = transform.gameObject.GetComponentsInChildren<InteractionGroup>();
				if( _interactionGroups.Length == 0 )
				{
					Debug.LogError("WorldPart:InteractionGroups : no InteractionGroups found! Make sure there's at least 1! " + transform.name);
				}
			}

			return _interactionGroups; 
		}
	}
	
	public Transform NumeratorSpecificObjects()
	{
		return transform.FindChild("GeometryNumerator");
	}

	public Transform DenominatorSpecificObjects()
	{
		return transform.FindChild("GeometryDenominator");
	}

	/*
	public Transform SpawnLeft
	{
		get
		{
			return InteractionGroups[0].FindChild("1/Spawn1");
		}
	}
	
	public Transform SpawnRight
	{
		get
		{
			return InteractionGroups[0].FindChild("1/Spawn2");
		}
	}
	*/
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
