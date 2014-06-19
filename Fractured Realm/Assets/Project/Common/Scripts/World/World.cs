using UnityEngine;
using System.Collections;

public class World : MonoBehaviour 
{
	public WorldPart numerator = null;
	public WorldPart denominator = null;

	public bool HasNumerator(){ return (numerator != null); }
	public bool HasDenominator(){ return (denominator != null); }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
