using UnityEngine;
using System.Collections;
using UnityEditor;

public class SelectCamera : EditorWindow
{
    [MenuItem("Lugus/Select Camera")]

    static void Init()
    {
        SelectCamera selectCameraWindow = (SelectCamera)EditorWindow.GetWindow(typeof(SelectCamera));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Select FRCameras", GUILayout.Height(100)))
        {

            GameObject FRCameras = GameObject.Find("FRCameras");

            if (FRCameras == null)
            {
                Debug.Log("SelectCamera: There is no FRCameras in the scene");
                return;
            }

            Selection.activeTransform = FRCameras.transform;
        }
    }
}
