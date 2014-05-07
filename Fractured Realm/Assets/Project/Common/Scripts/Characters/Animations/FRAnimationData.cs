using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FRAnimationData
{
	public string name = "";
	public int hash = -1;
	public int layer = 0;

	public string parentName = "";

	public string movieClipName = "";
	public bool loop = false;

	public delegate IOperationVisualizer GetVisualizer();

	public GetVisualizer visualizer = null;

	public string VisualizerClassName()
	{	
		
		List<string> operations = new List<string>();
		operations.Add ("Add");
		operations.Add ("Subtract");
		operations.Add ("Multiply");
		operations.Add ("Divide");
		operations.Add ("Simplify");
		operations.Add ("Double");


		string[] splits = parentName.Split('.');
		string parentOperation = splits[0];
		string parentStage = "";
		if( splits.Length > 1 )
		{
			parentStage = splits[1];
		}

		if( !operations.Contains( parentOperation ) )
			return "";
		
		string stageName = ( !string.IsNullOrEmpty(parentStage) ) ? "_" + parentStage : "";
		return "OperationVisualizer" + parentOperation + stageName + "_" + this.name;
	}
}
