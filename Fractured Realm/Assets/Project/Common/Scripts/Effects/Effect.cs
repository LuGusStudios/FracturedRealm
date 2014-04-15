using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {
	
	public void Free()
	{
		EffectFactory.use.FreeEffect( this );
	}

	public float Duration()
	{
		ParticleSystem ps = GetComponent<ParticleSystem>();
		if( ps == null )
		{
			ps = GetComponentInChildren<ParticleSystem>();
		}

		if( ps == null )
		{
			return 1.0f;
		}
		else
		{
			return ps.duration;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
