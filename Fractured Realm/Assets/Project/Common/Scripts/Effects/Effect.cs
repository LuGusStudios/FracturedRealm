using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {
	
	public void Free()
	{
		EffectFactory.use.FreeEffect( this );
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
