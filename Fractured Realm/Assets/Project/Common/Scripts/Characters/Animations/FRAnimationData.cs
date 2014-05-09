using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FRAnimationData
{
	public string name = "";
	public int hash = -1;
	public int layer = 0;

	public FR.Animation type = FR.Animation.NONE;

	public FR.OperationType operation = FR.OperationType.NONE;
	public FR.AnimationStage stage = FR.AnimationStage.NONE;
	
	public string parentName = ""; // to assist in auto-creating the classes through script generation
	public string originalOperationName = ""; // to assist in auto-creating the classes through script generation
	public string originalStageName = ""; // to assist in auto-creating the classes through script generation


	public string movieClipName = "";
	public bool loop = false;

	public delegate IOperationVisualizer GetVisualizer();

	public GetVisualizer visualizer = null;

	public string VisualizerClassName()
	{	
		if( operation == FR.OperationType.NONE )
			return "";
		
		string stageName = ( !string.IsNullOrEmpty(originalStageName) ) ? "_" + originalStageName : "";
		return "OperationVisualizer" + originalOperationName + stageName + "_" + this.name;
	}
}
