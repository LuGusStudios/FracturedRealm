using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
    GameObject FRCamera = null;
    public iTween.EaseType easeType;
    public float startingXposition = -40;
    public float endingXposition = 0;
    public float seconds = 15;
    public bool allowMoving = true;


	void Start () 
    {
        FRCamera = GameObject.Find("FRCameras");

        if (FRCamera == null)
            Debug.LogError("CameraMover: There is no FRCameras in this scene!");

        else
        {
            FRCamera.transform.position = new Vector3(startingXposition, 0, 0);
            StartCoroutine(MoveCamera());
        }
	}

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 100, 50), "Restart"))
    //    {
    //        FRCamera.transform.position = new Vector3(startingXposition, 0, 0);
    //        StartCoroutine(MoveCamera());
    //    }
    //}

    IEnumerator MoveCamera()
    {
        if (allowMoving)
        {
            yield return new WaitForSeconds(.8f);

            FRCamera.MoveTo(new Vector3(endingXposition, 0, 0)).Time(seconds).EaseType(easeType).Execute();
        }
    }
}
