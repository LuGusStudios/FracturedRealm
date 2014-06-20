using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropFactory : LugusSingletonExisting<PropFactory> 
{	
	public List<Prop> props = new List<Prop>();

	public Prop CreateProp( FR.PropType propType )
	{
		Prop prefab = FindPropPrefab( propType );

		if( prefab == null )
		{
			Debug.LogError("PropFactory:CreateProp : no prop found for type " + propType );
			return null;
		}

		Prop prop = (Prop) GameObject.Instantiate( prefab );
		return prop;
	}

	public Prop FindPropPrefab( FR.PropType propType )
	{
		Prop output = null;

		foreach( Prop prop in props )
		{
			//Debug.LogWarning("Looking for prop " + propType.ToString() + " in prefab " + prop.name );
			if( prop.name == propType.ToString() )
			{
				//Debug.LogError("PROP FOUND" + prop.name );

				output = prop;
				break;
			}
		}

		return output;
	}

	public void FreeProp(Prop prop)
	{
		GameObject.Destroy( prop.gameObject );
	}
}