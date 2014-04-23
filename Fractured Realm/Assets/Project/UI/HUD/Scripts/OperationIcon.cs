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
		set{ int prevAmount = _operationAmount; _operationAmount = value; UpdateRenderers(prevAmount); }
	}

	protected Transform iconRendererTemplate = null;
	protected List<Transform> Renderers = new List<Transform>(); 

	public void UpdateRenderers(int previousAmount)
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


		int animationStartIndex = -1;

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

			animationStartIndex = 0; // normal animation, show the one icon
		}
		else
		{
			int leftOver = _operationAmount - Renderers.Count;

			if( leftOver > 0 )
			{
				// one or more operations were added
				for( int i = 0; i < leftOver; ++i )
				{
					Transform renderer = (Transform) GameObject.Instantiate( iconRendererTemplate );
					renderer.transform.position = GetNextRendererPosition();
					renderer.transform.parent = this.transform;


					renderer.transform.name = "Renderer" + (Renderers.Count);

					Renderers.Add( renderer );
				}

				animationStartIndex = Mathf.Max ( 0, previousAmount - 1 ); // only animate the new ones
			}
			else if( leftOver < 0 )
			{
				// one or more were removed
				// destroying/hiding the renderers themselves should be done by the user of this class
				// this only updates the internal structure
				Renderers = new List<Transform>();
				for( int i = 0; i < _operationAmount; ++i )
				{
					Transform renderer = transform.FindChild( "Renderer" + i );
					if( renderer == null )
					{
						Debug.LogError ("OperationIcon:UpdateRenderers : could not find renderer " + i + " after lowering operationAmount");
					}
					else
					{
						Renderers.Add( renderer );
					}
				}
				
				animationStartIndex = Renderers.Count; // no animation needed
			}
			else
			{
				Debug.LogError("OperationIcon:UpdateRenderers called without change in operationAmount! " + previousAmount + " // " + _operationAmount + " - " + Renderers.Count + " = " + leftOver);
				animationStartIndex = Mathf.Max ( 0, previousAmount - 1 ); //Renderers.Count; // no animation needed
			}
		}

		if( _operationAmount != 0 )
		{
			int j = 0;
			for( j = animationStartIndex; j < Renderers.Count; ++j )
			{
				Transform renderer = Renderers[j];

				renderer.transform.localScale = Vector3.zero;

				renderer.transform.eulerAngles = new Vector3(0,0,0);
				renderer.transform.Rotate( new Vector3(0, 0, UnityEngine.Random.Range(-20.0f, 20.0f)) );

				renderer.gameObject.ScaleTo( new Vector3(1,1,1) ).Time (1.0f).Delay(j * 0.3f).EaseType( iTween.EaseType.easeOutBounce ).Execute();
			}

			// resize collider
			BoxCollider2D box = ((BoxCollider2D) collider2D);
			box.size = new Vector2( 1.8f, 1.8f + (0.32f * (Renderers.Count - 1)) ); // 0.32f is magical number
			box.center = new Vector2( 0.0f, 0.15f * (Renderers.Count - 1) ); // 0.15f is magical number
		}
	}

	public Vector3 GetNextRendererPosition()
	{
		return this.transform.position.xAdd(UnityEngine.Random.Range(-0.1f, 0.1f)).yAdd( Renderers.Count * rendererYOffset ).zAdd( Renderers.Count * rendererZOffset);
	}

	public Vector3 GetTopRendererPosition()
	{
		return this.transform.position.xAdd(UnityEngine.Random.Range(-0.1f, 0.1f)).yAdd( (Renderers.Count - 1) * rendererYOffset ).zAdd( (Renderers.Count - 1) * rendererZOffset);
	}

	public Transform GetTopRenderer()
	{
		if( Renderers != null && Renderers.Count > 0 )
			return Renderers[ Renderers.Count - 1 ];
		else
			return null;
	}

	void Awake()
	{
		_operationAmount = -1;

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
		//if( button.pressed ) 
		//{
		//	MathManager.use.SelectOperation( this.type );
		//}
	}
}
