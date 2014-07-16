using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoldoutMenu : MonoBehaviour 
{
	public bool open = false;
	public List<Transform> children = new List<Transform>();

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		Transform childrenRoot = transform.FindChild("Children");
		foreach( Transform child in childrenRoot )
		{
			children.Add( child );
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		Close (false);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void Open(bool animate = true)
	{
		open = true;

		float yPadding = 1.0f;
		float currentY = -0.5f;

		int count = 0;
		foreach( Transform child in children )
		{
			child.gameObject.StopTweens();
			child.collider2D.enabled = true;
			child.transform.localPosition = Vector3.zero.z(1);
			
			currentY -= yPadding + child.renderer.bounds.extents.y;
			Vector3 target = child.transform.localPosition.yAdd( currentY );

			if( animate )
				child.gameObject.MoveTo( target ).IsLocal(true).Delay(count * 0.35f).Time (0.65f).EaseType(iTween.EaseType.easeOutBack).Execute();
			else
				child.transform.localPosition = target;

			count++;
		}
	}

	public void Close(bool animate = true)
	{
		open = false;

		int count = 0;
		foreach( Transform child in children )
		{
			child.gameObject.StopTweens();
			child.collider2D.enabled = false;

			if( animate )
				child.gameObject.MoveTo( Vector3.zero.z(1) ).IsLocal(true).Time (0.4f).Execute();
			else
				child.transform.localPosition = Vector3.zero.z(1);
			
			count++;
		}
	}
	
	protected void Update () 
	{
		Transform hit = LugusInput.use.RayCastFromMouseUp2D();
		if( hit == null )
			return;

		if( hit == this.transform )
		{
			open = !open;

			if( open )
				Open ();
			else
				Close ();
		}
	}
}
