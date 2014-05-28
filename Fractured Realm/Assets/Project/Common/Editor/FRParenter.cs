using UnityEngine;
using System.Collections;
using UnityEditor;

public class Parenter : EditorWindow 
{
    [MenuItem("FR/Parenter")]

    static void Init()
    {
        Parenter parenterWindow = (Parenter)EditorWindow.GetWindow(typeof(Parenter));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Parent under Geometry", GUILayout.Height(100)))
        {
            GameObject geometry = GameObject.Find("Geometry");

            if (geometry == null)
            {
                Debug.LogError("Can't find the geometry gameobject in the scene");
                return;
            }

            foreach (GameObject go in Selection.gameObjects)
            {
                go.transform.parent = geometry.transform;
            }
        }

        if (GUILayout.Button("Parent under numerator geometry", GUILayout.Height(100)))
        {
            GameObject numeratorGeometry = GameObject.Find("GeometryNumerator");

            if (numeratorGeometry == null)
            {
                Debug.LogError("Can't find the numerator geometry gameobject in the scene");
                return;
            }

            foreach (GameObject go in Selection.gameObjects)
            {
                go.transform.parent = numeratorGeometry.transform;
            }
        }

        if (GUILayout.Button("Parent under denominator geometry", GUILayout.Height(100)))
        {
            GameObject denominatorGeometry = GameObject.Find("GeometryDenominator");

            if (denominatorGeometry == null)
            {
                Debug.LogError("Can't find the denominator geometry gameobject in the scene");
                return;
            }

            foreach (GameObject go in Selection.gameObjects)
            {
                go.transform.parent = denominatorGeometry.transform;
            }
        }
    }
}
