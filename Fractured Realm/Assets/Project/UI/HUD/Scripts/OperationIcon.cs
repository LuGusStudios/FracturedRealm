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

	public void Clear()
	{
		// remove existing renderers (if any)
		for( int i = 0; i < Renderers.Count; ++i )
		{
			Destroy ( Renderers[i].gameObject );
		}
		
		Renderers = new List<Transform>();
		_operationAmount = 0;
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

			iconRendererTemplate.name = "RendererTemplate";
			iconRendererTemplate.renderer.enabled = false;
		}

		//if( Renderers.Count == 0 )
		//{
		//	Renderers.Add( iconRendererTemplate );
		//}

		//Debug.Log ("OperationIcon : UpdateRenderers : " + this.type + " -> " + _operationAmount + " // " + Renderers.Count );


		if( _operationAmount == 0 )
		{
			this.collider2D.enabled = false;
		}
		else
		{
			this.collider2D.enabled = true;
		}


		int animationStartIndex = -1;

		if( _operationAmount == 0 )
		{
			// if previous was 1, and now we're 0, we just removed the last peg from the pile
			// it's probably still animating and will be removed by the UIManager, so we shouldn't remove it here
			// NOTE: this is custom logic for when you work with 1 peg/action at a time. Should there be more than 1 at a time, this should be revised
			if( previousAmount != 1 )
			{
				// remove existing renderers (if any)
				for( int i = 0; i < Renderers.Count; ++i )
				{
					Destroy ( Renderers[i].gameObject );
				}
			}
			
			Renderers = new List<Transform>();
			animationStartIndex = 666; // no animation needed
		}
		else if( _operationAmount == -1 )
		{
			// infinite uses
			// just 1 renderer needed that is re-used constantly

			// remove existing renderers (if any)
			for( int i = 1; i < Renderers.Count; ++i )
			{
				Destroy ( Renderers[i].gameObject );
			}

			Transform renderer = null;
			if( Renderers.Count == 0 )
			{
				Renderers = new List<Transform>();

				renderer = (Transform) GameObject.Instantiate( iconRendererTemplate );
				renderer.renderer.enabled = true;
				renderer.transform.position = GetNextRendererPosition();
				renderer.transform.parent = this.transform;
				
				
				renderer.transform.name = "Renderer" + (Renderers.Count);
			}
			else
			{
				renderer = Renderers[0]; // we've left this one "alive" above

				Renderers = new List<Transform>(); // remove others from the list, later add the correct one
			}

			Renderers.Add ( renderer );

			animationStartIndex = 0; // animate the one
		}
		else
		{
			int leftOver = _operationAmount - Renderers.Count;

			if( leftOver > 0 ) 
			{
				// one or more operations were added
				//Debug.LogWarning("Icon " + this.name + " : add extra operations : " + leftOver + " = " + _operationAmount + " - " + Renderers.Count + ". Prev : " + previousAmount);

				for( int i = 0; i < leftOver; ++i )
				{
					Transform renderer = (Transform) GameObject.Instantiate( iconRendererTemplate );
					renderer.renderer.enabled = true;
					renderer.transform.position = GetNextRendererPosition();
					renderer.transform.parent = this.transform;


					renderer.transform.name = "Renderer" + (Renderers.Count);

					Renderers.Add( renderer );
				}

				animationStartIndex = Mathf.Max ( 0, previousAmount ); // only animate the new ones
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
			int count = 0;
			for( int j = animationStartIndex; j < Renderers.Count; ++j )
			{
				Transform renderer = Renderers[j];

				renderer.transform.localScale = Vector3.zero;

				renderer.transform.eulerAngles = new Vector3(0,0,0);
				renderer.transform.Rotate( new Vector3(0, 0, UnityEngine.Random.Range(-20.0f, 20.0f)) );

				renderer.gameObject.ScaleTo( new Vector3(1,1,1) ).Time (1.0f).Delay(count * 0.3f).EaseType( iTween.EaseType.easeOutBounce ).Execute();

				count++;
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
