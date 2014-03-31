using UnityEngine;
using System.Collections;

public class ObjectBehavior : MonoBehaviour 
{
    public float scaleBy = 1.5f;

    private Vector3 startScale;
    private Vector3 newScale;

	void Start () 
    {
        startScale = this.gameObject.transform.localScale;
        newScale = startScale + new Vector3(scaleBy, scaleBy, scaleBy);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == this.gameObject.name)
                {
                    iTween.PunchScale(hit.collider.gameObject, new Vector3(scaleBy, scaleBy, scaleBy), .6f);
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
        }
	}
}
