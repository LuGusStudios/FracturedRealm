using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationIconBackgrounds : MonoBehaviour 
{
	public void OnWorldGenerated(FR.WorldType worldType, FR.Target composition)
	{
		Sprite background = LugusResources.use.Shared.GetSprite("OperationButtonBackground" + worldType );

		if( background == null || background == LugusResources.use.errorSprite )
		{
			background = LugusResources.use.Shared.GetSprite("OperationButtonBackground" + FR.WorldType.FOREST );
		}

		GetComponent<SpriteRenderer>().sprite = background;
	}

	public void SetupLocal()
	{
		WorldFactory.use.onWorldGenerated += OnWorldGenerated;
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}
}
