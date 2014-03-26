using UnityEngine;
using System.Collections;

public class WorldPart : MonoBehaviour 
{
	public Transform InteractionPointsContainer
	{
		get
		{
			return transform.FindChild ("InteractionPoints");
		}
	}
	
	public Transform SpawnLeft
	{
		get
		{
			return InteractionPointsContainer.FindChild("SpawnLeft");
		}
	}
	
	public Transform SpawnRight
	{
		get
		{
			return InteractionPointsContainer.FindChild("SpawnRight");
		}
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
