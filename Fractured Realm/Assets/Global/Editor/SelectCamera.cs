using UnityEngine;
using System.Collections;
using UnityEditor;

public class SelectCamera : EditorWindow
{
    [MenuItem("FR/Select Camera")]

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


        if (GUILayout.Button("Select CameraMover", GUILayout.Height(50)))
        {
            GameObject cameraMover = GameObject.Find("CameraMover");

            if (cameraMover == null)
            {
                Debug.Log("SelectCamera: There is no CameraMover in the scene");
                return;
            }

            Selection.activeTransform = cameraMover.transform;
        }

		GUILayout.BeginVertical();

		FRCamera.use.DrawInteractionGroupSelectionGUI();

		GUILayout.Space(40);
		
		if( GUILayout.Button("\nNumerator\n") )
		{
			FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
			HUDManager.use.SetMode( FR.Target.NUMERATOR );
		}
		else if( GUILayout.Button("\nDenominator\n") )
		{
			FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
			HUDManager.use.SetMode( FR.Target.DENOMINATOR );
		}
		else if( GUILayout.Button("\nBoth\n") )
		{
			FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
			HUDManager.use.SetMode( FR.Target.BOTH );
		}

		GUILayout.Space(10);
		
		if( GUILayout.Button("Default positions") )
		{
			FRCamera.use.MoveToDefaultPositions();
		}

		GUILayout.EndVertical();

    }
}
