using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FractionAnimator
{
	protected FractionRenderer _renderer = null;
	public FractionRenderer Renderer
	{
		get{ return _renderer; }
		set{ _renderer = value; }
	}
}
