using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(InteractionGroup))]
public class InteractionGroupEditor : Editor 
{
	public void OnSceneGUI() 
	{
		InteractionGroup subject = (InteractionGroup) target;
		
		//Handles.color = Color.red;
		//Handles.Label(subject.transform.position + Vector3.up*45, subject.transform.position.ToString() + " ROBIN" );
		
		//GUIStyle style = new GUIStyle();
		
		// style.normal.textColor = Color.red;
		
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.red;
		GUIStyle style2 = new GUIStyle();
		style2.normal.textColor = Color.black;
		style2.fontSize = 14;
		
		GUIStyle style3 = new GUIStyle();
		style3.normal.textColor = Color.blue; 
		style3.fontSize = 15;
		
		// http://forum.unity3d.com/threads/107333-very-small-request-Handle-Label
		Handles.BeginGUI();

		foreach( Transform child in subject.transform )
		{
			Handles.Label(child.transform.position, child.name, style2 );
		}
	
		Handles.EndGUI();
	}
}