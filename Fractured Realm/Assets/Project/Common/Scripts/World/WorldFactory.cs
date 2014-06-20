using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FR
{
	public enum WorldType
	{
		NONE = 0,
		FOREST = 1,
		DESERT = 2,
		FIRE = 3,
		ICE = 4,
		JUNGLE = 5
	}
}

public class WorldFactory : LugusSingletonExisting<WorldFactory> 
{	
	public delegate void OnWorldGenerated(FR.WorldType worldType, FR.Target composition);
	public OnWorldGenerated onWorldGenerated = null;

	public void Start()
	{
		// TODO: CHANGE!
		//CreateWorld(FR.WorldType.DESERT, FR.Target.BOTH, true);
		//HUDManager.use.SetMode( FR.Target.BOTH );

		if( debug_initialFractions == null || debug_initialFractions.Length == 0 )
		{
			debug_initialFractions = new Fraction[2];
			debug_initialFractions[0] = new Fraction(4,2);
			debug_initialFractions[1] = new Fraction(7,4);
		}

		CreateDebugWorld( defaultWorldType, debug_initialFractions, FR.Target.BOTH, true );
	}
	
	// Dictionary<FR.WorldType, List<WorldPart>> is NOT SERIALIZABLE by unity!
	// so we need to use 2 lists and GetTemplateIndexForType() to get the same functionality
	//public List<FR.WorldType> worldTypes = new List<FR.WorldType>();
	//public List<WorldPart> worldTemplates = new List<WorldPart>();


	public List<WorldPart> worldParts_Fire = new List<WorldPart>();
	public List<WorldPart> worldParts_Forest = new List<WorldPart>();
	public List<WorldPart> worldParts_Jungle = new List<WorldPart>();
	public List<WorldPart> worldParts_Ice = new List<WorldPart>();
	public List<WorldPart> worldParts_Desert = new List<WorldPart>();

	public List<WorldPart> GetWorldParts( FR.WorldType worldType )
	{
		if( worldType == FR.WorldType.FIRE )
		{
			return worldParts_Fire;
		}
		else if(  worldType == FR.WorldType.FOREST )
		{
			return worldParts_Forest;
		}
		else if(  worldType == FR.WorldType.JUNGLE )
		{
			return worldParts_Jungle;
		}
		else if(  worldType == FR.WorldType.ICE )
		{
			return worldParts_Ice;
		}
		else if(  worldType == FR.WorldType.DESERT )
		{
			return worldParts_Desert;
		}

		return null; 
	}
	
	//public Dictionary<FR.WorldType, List<WorldPart>> worldTemplates = new Dictionary<FR.WorldType, List<WorldPart>>();
	/*
	public int GetTemplateIndexForType(FR.WorldType type)
	{
		if( !worldTypes.Contains(type) )
		{
			Debug.LogError("WorldFactory:GetTemplateIndexForType : unknown worldtype : " + type);
			return -1;
		}
		
		int index = worldTypes.IndexOf( type );
		return (index * 2); // 2 entries per index
	}
	*/

	public World CreateDebugWorld(FR.WorldType type, Fraction[] fractions, FR.Target composition = FR.Target.BOTH, bool fillAllInteractionGroups = true)
	{
		World output = CreateWorld(type, composition);
		List<Fraction> fractionList = new List<Fraction>();
		fractionList.AddRange( fractions );
		RendererFactory.use.CreateRenderers( output, fractionList, ((fillAllInteractionGroups) ? -1 : 0) );
		FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
		HUDManager.use.SetMode( composition );


		FRCamera.use.MoveToInteractionGroup( 1, 1, false );

		if( Application.isPlaying )
		{
			//Debug.LogError("App is playing, so initialize operation icons");
			HUDManager.use.UpdateOperationIcons(1);
		}

		return output;
	}

	public Fraction[] debug_initialFractions;
	public int test = 5;

	public FR.WorldType defaultWorldType = FR.WorldType.DESERT;

	/*
	public World CreateWorld(FR.WorldType type, FR.Target composition = FR.Target.BOTH, bool fillAllInteractionGroups = false)
	{
		if( debug_initialFractions == null || debug_initialFractions.Length == 0 )
		{
			debug_initialFractions = new Fraction[2];
			debug_initialFractions[0] = new Fraction(4,2);
			debug_initialFractions[1] = new Fraction(7,4);
		}
		
		return CreateWorld(type, debug_initialFractions, composition, fillAllInteractionGroups);
	}
	*/
	
	public World CreateWorld(FR.WorldType type, FR.Target composition = FR.Target.BOTH ) 
	{
		/*
		if( !worldTypes.Contains(type) )
		{
			Debug.LogError("WorldFactory:CreateWorld : unknown worldtype : " + type);
			return null;
		}
		*/

		if( !composition.HasNumerator() && !composition.HasDenominator() )
		{
			Debug.LogError("WorldFactory:CreateWorld : Composition had neither Numerator nor Denominator. " + composition);
			return null;
		}
		
		List<WorldPart> templates = GetWorldParts( type );
		if( templates == null || templates.Count == 0 )
		{
			Debug.LogError("WorldFactory:CreateWorld : no prefabs defined for world type " + type);
			return null;
		}

		GameObject WORLD = GameObject.Find("WORLD");
		if( WORLD != null )
			GameObject.DestroyImmediate( WORLD );
			
		WORLD = new GameObject("WORLD");
		WORLD.transform.position = new Vector3(0, 0, 0);


		WorldPart template = templates[ UnityEngine.Random.Range(0, templates.Count) ];
		
		World world = WORLD.AddComponent<World>();

		bool numeratorHasDirectionalLight = false;
		if( composition.HasNumerator() )
		{
			world.numerator = (WorldPart) GameObject.Instantiate( template );//worldTemplates[ index ] );
			world.numerator.gameObject.SetActive(true);
			world.numerator.transform.parent = WORLD.transform;
			world.numerator.transform.position   = new Vector3(0, 0, 15.0f );//LugusCamera.numerator.transform.position.zAdd( 15.0f );

			Transform denominatorSpecifics = world.numerator.DenominatorSpecificObjects();
			if( denominatorSpecifics != null )
			{
				denominatorSpecifics.gameObject.SetActive(false);
			}

			Light[] lights = world.numerator.GetComponentsInChildren<Light>(true);
			foreach( Light light in lights )
			{
				if(  light.type == LightType.Directional  )
				{
					if( !numeratorHasDirectionalLight )
					{
						numeratorHasDirectionalLight = true;
					}
					else
					{
						Debug.LogError("WorldFactory:CreateWorld : numerator had more than 1 directional light!");
						light.enabled = false;
					}
				}
			}
		}
		else
			world.numerator = null;

		if( composition.HasDenominator() )
		{
			world.denominator = (WorldPart) GameObject.Instantiate( template );// worldTemplates[ index + 1 ] );
			world.denominator.gameObject.SetActive(true); 
			world.denominator.transform.parent = WORLD.transform;
			world.denominator.transform.position = new Vector3(0, -1200.0f, 15.0f );//LugusCamera.denominator.transform.position.zAdd( 15.0f );
			
			Transform numeratorSpecifics = world.denominator.NumeratorSpecificObjects();
			if( numeratorSpecifics != null )
			{
				numeratorSpecifics.gameObject.SetActive(false);
			}

			if( numeratorHasDirectionalLight )
			{
				Light[] lights = world.numerator.GetComponentsInChildren<Light>(true);
				foreach( Light light in lights )
				{
					if( light.type == LightType.Directional )
					{
						light.enabled = false;
					}
				}
			}
		}
		else
			world.denominator = null;




		if( onWorldGenerated != null )
		{
			onWorldGenerated( type, composition );
		}

		return world;
	}


	/*
	// TODO: remove, just for debugging
	public void CreateFractions(Fraction[] fractions)
	{
		Fraction fr = new Fraction(fractions[0].Numerator.Value, fractions[0].Denominator.Value); 
		
		FractionRenderer frr = RendererFactory.use.CreateRenderer( fr );
		if( fractions[0].Numerator.Value != 0 )
			frr.Numerator.transform.position = new Vector3(0,0,0);
		if( fractions[0].Denominator.Value != 0 )
			frr.Denominator.transform.position = new Vector3(0,-5,0);
		
		
		Fraction fr2 = new Fraction(fractions[1].Numerator.Value, fractions[1].Denominator.Value); 
		
		FractionRenderer frr2 = RendererFactory.use.CreateRenderer( fr2 );
		if( fractions[1].Numerator.Value != 0 )
			frr2.Numerator.transform.position = new Vector3(10,0,0);
		if( fractions[1].Denominator.Value != 0 )
			frr2.Denominator.transform.position = new Vector3(10,-5,0);

		ValidateInternalFraction( fr,  "fraction 1 : ");
		ValidateInternalFraction( fr2, "fraction 2 : ");
	}
	*/




	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;
		
		GUILayout.BeginArea( new Rect(0, Screen.height - 200 , 200, 200 ), GUI.skin.box);
		GUILayout.BeginVertical();
		
		FR.WorldType[] worldTypes = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
		foreach( FR.WorldType worldType in worldTypes )
		{
			if( worldType == FR.WorldType.NONE )
				continue;
			
			if( GUILayout.Button("Create " + worldType + "\n") )
			{
				Fraction[] fractions = new Fraction[2];
				fractions[0] = new Fraction( 6, 3 );
				fractions[1] = new Fraction( 2, 3 );
				
				WorldFactory.use.CreateDebugWorld( worldType, fractions );
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

}
