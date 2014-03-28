using UnityEngine;
using System.Collections;

public class shadowPosition : MonoBehaviour {

    public GameObject character;

	// Update is called once per frame
	void Update () 
    {
        transform.position = character.transform.position;
	}
}
