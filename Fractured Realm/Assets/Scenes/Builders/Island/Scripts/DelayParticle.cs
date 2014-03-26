using UnityEngine;
using System.Collections;

public class DelayParticle : MonoBehaviour 
{
    public float delay;
    
	void Start () 
    {
        StartCoroutine(Wait());
	}

    private IEnumerator Wait()
    {
        this.particleSystem.enableEmission = false;

        yield return new WaitForSeconds(delay);

        this.particleSystem.enableEmission = true;

    }
	
}
