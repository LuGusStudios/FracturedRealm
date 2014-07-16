using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IMenu : MonoBehaviour 
{
	public bool activated = false;

	public abstract bool Activate(bool force = false);
	public abstract bool Deactivate(bool force = false);

	/*
	public override bool Activate(bool force = false)
	{
		if( activated && !force )
			return false;

		activated = true;

		return true;
	}

	public override bool Deactivate(bool force = false)
	{
		if( !activated && !force )
			return false;

		activated = false;

		return true;
	}
	*/
}
