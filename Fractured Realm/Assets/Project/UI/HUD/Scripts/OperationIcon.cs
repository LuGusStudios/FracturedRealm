using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Button))]
public class OperationIcon : MonoBehaviour 
{
	protected Button button = null;
	public FR.OperationType type = FR.OperationType.NONE;
	
	// Use this for initialization
	void Start () 
	{
		button = GetComponent<Button>();
		if( button == null )
		{
			Debug.LogError("OperationIcon:Start : Button is null!");
		}

		if( type == FR.OperationType.NONE )
		{
			if ( !Enum.IsDefined(typeof(FR.OperationType), this.name) )
			{
				Debug.LogError("OperationIcon:Start : OperationType unknown : " + this.name);
			}
			else
			{
				type = (FR.OperationType) Enum.Parse( typeof(FR.OperationType), this.name );
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( button.pressed ) 
		{
			MathInputManager.use.SelectOperation( this.type );
		}
	}
}
