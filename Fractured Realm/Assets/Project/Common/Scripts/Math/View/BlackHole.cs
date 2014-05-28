using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHole 
{
	public BlackHole(Vector3 position)
	{
		Create (position);
	}

	public GameObject renderer = null;
	public BlackHoleController controller = null;

	protected int _useCount = 0;
	public int useCount
	{
		get{ return _useCount; }
		set
		{
			SetUseCount( value );
		}
	}

	public void Free()
	{
		useCount--;
	}

	public void Create(Vector3 position)
	{
		Prop holeRenderer = PropFactory.use.CreateProp( FR.PropType.BlackHole );
		holeRenderer.transform.position = position;
		Vector3 originalScale = holeRenderer.transform.localScale;
		holeRenderer.transform.localScale = new Vector3(0,0,0);
		
		holeRenderer.gameObject.ScaleTo( originalScale ).Time (0.3f).Execute();

		this.renderer = holeRenderer.gameObject;
	}

	protected void SetUseCount(int useCount)
	{
		_useCount = useCount;

		if( _useCount == 0 )
		{
			if( controller != null )
			{
				controller.FreeHole(this);
			}

			renderer.gameObject.ScaleTo( Vector3.zero ).Time ( 0.3f ).Execute();

			GameObject.Destroy( renderer, 1.0f );
		}
	}
}
