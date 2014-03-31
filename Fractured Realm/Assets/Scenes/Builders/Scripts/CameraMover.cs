using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
    GameObject FRCamera = null;
    public iTween.EaseType easeType;


	void Start () 
    {
        FRCamera = GameObject.Find("FRCameras");

        if (FRCamera == null)
            Debug.LogError("CameraMover: There is no FRCameras in this scene!");

        else
        {
            FRCamera.transform.position = new Vector3(-40, 0, 0);
            StartCoroutine(MoveCamera());
        }
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "Restart"))
        {
            FRCamera.transform.position = new Vector3(-40, 0, 0);
            StartCoroutine(MoveCamera());
        }
    }

    IEnumerator MoveCamera()
    {
        yield return new WaitForSeconds(.8f);

        FRCamera.MoveTo(new Vector3(0, 0, 0)).Time(15f).EaseType(easeType).Execute();
    }
}
