using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class OperationIcon : MonoBehaviour 
{
	protected Button button = null;
	public FR.OperationType type = FR.OperationType.NONE;

	protected float rendererYOffset = 0.3f;
	protected float rendererZOffset = -0.1f;

	protected int _operationAmount = -1; // -1 is infinite
	public int OperationAmount
	{
		get{ return _operationAmount; }
		set{ _operationAmount = value; UpdateRenderers(); }
	}

	protected Transform iconRendererTemplate = null;
	protected List<Transform> Renderers = new List<Transform>(); 

	public void UpdateRenderers()
	{
		if( iconRendererTemplate == null )
		{
			iconRendererTemplate = transform.GetChild(0);
			if( iconRendererTemplate == null )
			{
				Debug.LogError("OperationIcon:UpdateRenderers : iconRendererTemplate not found. Should be child of icon!");
				return;
			}
		}

		if( Renderers.Count == 0 )
		{
			Renderers.Add( iconRendererTemplate );
		}

		Debug.Log ("OperationIcon : UpdateRenderers : " + this.type + " -> " + _operationAmount + " // " + Renderers.Count );


		if( _operationAmount == 0 )
		{
			this.collider2D.enabled = false;
		}
		else
		{
			this.collider2D.enabled = true;
		}


		if( _operationAmount == -1 || _operationAmount == 0 )
		{
			// infinite uses
			// just 1 renderer needed that is re-used constantly

			// remove all excess renderers (if any)
			for( int i = 1; i < Renderers.Count; ++i )
			{
				Destroy ( Renderers[i].gameObject );
			}

			Renderers = new List<Transform>();
			Renderers.Add( iconRendererTemplate );

			if( _operationAmount == 0 )
			{
				iconRendererTemplate.localScale = new Vector3(0,0,0);
			}
		}
		else
		{
			int leftOver = _operationAmount - Renderers.Count;
			for( int i = 0; i < leftOver; ++i )
			{
				Transform renderer = (Transform) GameObject.Instantiate( iconRendererTemplate );
				renderer.transform.position = GetNextRendererPosition();
				renderer.transform.parent = this.transform;

				renderer.transform.name = "Renderer" + (Renderers.Count);

				Renderers.Add( renderer );
			}
		}

		if( _operationAmount != 0 )
		{
			int j = 0;
			foreach( Transform renderer in Renderers )
			{
				renderer.transform.localScale = Vector3.zero;
				renderer.gameObject.ScaleTo( new Vector3(1,1,1) ).Time (1.0f).Delay(j * 0.3f).EaseType( iTween.EaseType.easeOutBounce ).Execute();
				j++;
			}

			// resize collider
			BoxCollider2D box = ((BoxCollider2D) collider2D);
			box.size = new Vector2( 1.8f, 1.8f + (0.32f * (Renderers.Count - 1)) ); // 0.32f is magical number
			box.center = new Vector2( 0.0f, 0.15f * (Renderers.Count - 1) ); // 0.15f is magical number
		}
	}

	protected Vector3 GetNextRendererPosition()
	{
		return this.transform.position.yAdd( Renderers.Count * rendererYOffset ).zAdd( Renderers.Count * rendererZOffset);
	}

	void Awake()
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

		foreach( Transform child in this.transform )
		{
			child.localScale = new Vector3(0,0,0); // hide buttons for now
		}
	}

	// Use this for initialization
	void Start () 
	{
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
