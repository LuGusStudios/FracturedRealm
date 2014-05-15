using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	public enum WorldType
	{
		NONE,
		FOREST,
		DESERT,
		FIRE,
		ICE,
		JUNGLE
	}
}

public class WorldFactory : LugusSingletonExisting<WorldFactory> 
{	
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

		CreateDebugWorld( FR.WorldType.DESERT, debug_initialFractions, FR.Target.BOTH, true );
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
		World output = CreateWorld(type, fractions, composition, fillAllInteractionGroups);
		FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
		HUDManager.use.SetMode( composition );

		FRCamera.use.MoveToInteractionGroup( 1, 1, false );

		if( Application.isPlaying )
		{
			//Debug.LogError("App is playing, so initialize operation icons");
			MathInputManager.use.InitializeOperationIcons(1);
		}

		return output;
	}

	public Fraction[] debug_initialFractions;
	public int test = 5;
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
	
	public World CreateWorld(FR.WorldType type, Fraction[] fractions, FR.Target composition = FR.Target.BOTH, bool fillAllInteractionGroups = false ) 
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
		if( templates.Count == 0 )
		{
			Debug.LogError("WorldFactory:CreateWorld : no prefabs defined for world type " + type);
			return null;
		}

		GameObject WORLD = GameObject.Find("WORLD");
		if( WORLD != null )
			GameObject.DestroyImmediate( WORLD );
			
		WORLD = new GameObject("WORLD");
		WORLD.transform.position = new Vector3(0, 0, 0);


		WorldPart template = templates[ Random.Range(0, templates.Count) ];
		
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
			world.denominator.transform.position = new Vector3(0, -200.0f, 15.0f );//LugusCamera.denominator.transform.position.zAdd( 15.0f );
			
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



		// Spawn the LEFT fraction (fraction 0)
		int groupCount = 1;
		if( fillAllInteractionGroups )
		{
			if( composition.HasNumerator() )
				groupCount = world.numerator.InteractionGroups.Length;
			else
				groupCount = world.denominator.InteractionGroups.Length;
		}

		for( int i = 0; i < groupCount; ++i )
		{
			Fraction fr = new Fraction(fractions[0].Numerator.Value, fractions[0].Denominator.Value); 
			
			FractionRenderer frr = RendererFactory.use.CreateRenderer( fr );

			if( fr.Numerator.Value != 0 && composition.HasNumerator() )
			{
				frr.Numerator.transform.parent = world.numerator.transform;
				frr.Numerator.SpawnPosition = world.numerator.InteractionGroups[i].Spawn1.position;
				frr.Numerator.transform.position = world.numerator.InteractionGroups[i].Spawn1.position;


				Portal p = RendererFactory.use.CreatePortal( fr.Numerator.Value, FR.Target.NUMERATOR );
				p.transform.position = world.numerator.InteractionGroups[i].PortalEntry.position;
				p.transform.parent = world.numerator.transform;
			}
			if( fr.Denominator.Value != 0 && composition.HasDenominator() )
			{
				frr.Denominator.transform.parent = world.denominator.transform;
				frr.Denominator.SpawnPosition = world.denominator.InteractionGroups[i].Spawn1.position;
				frr.Denominator.transform.position = world.denominator.InteractionGroups[i].Spawn1.position;


				Portal p = RendererFactory.use.CreatePortal( fr.Denominator.Value, FR.Target.DENOMINATOR );
				p.transform.position = world.denominator.InteractionGroups[i].PortalEntry.position;
				p.transform.parent = world.denominator.transform;
			}
			
			ValidateInternalFraction( fr,  "fraction 1 : ");
		}

		
		// Spawn the RIGHT fraction (fraction 1)
		for( int i = 0; i < groupCount; ++i )
		{
			Fraction fr2 = new Fraction(fractions[1].Numerator.Value, fractions[1].Denominator.Value); 
			
			FractionRenderer frr2 = RendererFactory.use.CreateRenderer( fr2 );
			
			if( fr2.Numerator.Value != 0 && composition.HasNumerator() )
			{
				frr2.Numerator.transform.parent = world.numerator.transform;
				frr2.Numerator.SpawnPosition = world.numerator.InteractionGroups[i].Spawn2.position;
				frr2.Numerator.transform.position = world.numerator.InteractionGroups[i].Spawn2.position;

				// TODO: calculate expected end-value for the portal
				Portal p = RendererFactory.use.CreatePortal( fr2.Numerator.Value, FR.Target.NUMERATOR );
				p.transform.position = world.numerator.InteractionGroups[i].PortalExit.position;
				p.transform.parent = world.numerator.transform;
			}
			if( fr2.Denominator.Value != 0 && composition.HasDenominator() )
			{
				frr2.Denominator.transform.parent = world.denominator.transform; 
				frr2.Denominator.SpawnPosition = world.denominator.InteractionGroups[i].Spawn2.position;
				frr2.Denominator.transform.position = world.denominator.InteractionGroups[i].Spawn2.position;

				
				// TODO: calculate expected end-value for the portal
				Portal p = RendererFactory.use.CreatePortal( fr2.Denominator.Value, FR.Target.DENOMINATOR );
				p.transform.position = world.denominator.InteractionGroups[i].PortalExit.position;
				p.transform.parent = world.denominator.transform;
			
			}
			
			ValidateInternalFraction( fr2, "fraction 2 : ");
		}


		return world;
	}


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


	// TODO: remove, or just move : just for debugging
	protected void ValidateInternalFraction( Fraction fr, string messagePrefix )
	{
		if( fr.Numerator == null )
		{
			Debug.LogError(messagePrefix + "Validate: fraction doesn't have a Numerator!");
		}
		
		if( fr.Denominator == null )
		{
			Debug.LogError(messagePrefix + "Validate: fraction doesn't have a Denominator!");
		}

		if( fr.Renderer == null )
		{
			Debug.LogError(messagePrefix + "Validate: fraction doesn't have a renderer!");
		}
		
		if( fr.Renderer.Animator == null )
		{
			Debug.LogError(messagePrefix + "Validate: fraction doesn't have an animator!");
		}


		if( fr.Numerator.Fraction != fr )
		{
			Debug.LogError(messagePrefix + "Validate: Numerators fraction isn't what we expect! " + fr.Numerator.Fraction);
		}
		if( fr.Denominator.Fraction != fr )
		{
			Debug.LogError(messagePrefix + "Validate: Denominators fraction isn't what we expect! " + fr.Denominator.Fraction);
		}

		if( !fr.Numerator.IsNumerator )
			Debug.LogError(messagePrefix + "Validate: Numerator isn't marked as numerator! " + fr.Numerator.IsNumerator + " // " + fr.Numerator.IsDenominator );

		if( !fr.Denominator.IsDenominator )
			Debug.LogError(messagePrefix + "Validate: Denominator isn't marked as Denominator! " + fr.Denominator.IsDenominator + " // " + fr.Denominator.IsNumerator );


		if( fr.Numerator.OtherNumber != fr.Denominator )
			Debug.LogError(messagePrefix + "Validate: Numerator other number isn't the Denominator! " + fr.Numerator.OtherNumber);
		
		if( fr.Denominator.OtherNumber != fr.Numerator )
			Debug.LogError(messagePrefix + "Validate: Denominator other number isn't the Numerator! " + fr.Denominator.OtherNumber);


		if( fr.Numerator.Value == 0 && fr.Numerator.Renderer != null )
			Debug.LogError(messagePrefix + "Validate: Numerator has value 0, but still has a renderer! " + fr.Numerator.Renderer);
		
		if( fr.Denominator.Value == 0 && fr.Denominator.Renderer != null )
			Debug.LogError(messagePrefix + "Validate: Denominator has value 0, but still has a renderer! " + fr.Denominator.Renderer);

		
		if( fr.Numerator.Value != 0 && fr.Numerator.Renderer == null )
			Debug.LogError(messagePrefix + "Validate: Numerator has value != 0, but renderer is null! " + fr.Numerator.Renderer);
		
		if( fr.Denominator.Value != 0 && fr.Denominator.Renderer == null )
			Debug.LogError(messagePrefix + "Validate: Denominator has value != 0, but renderer is null! " + fr.Denominator.Renderer);

		if( fr.Numerator.ValuePast6 > 0 && fr.Numerator.Renderer.Characters.Length != 2 )
			Debug.LogError(messagePrefix + "Validate: Numerator is larger than 6, but has not 2 characters! " + fr.Numerator.Renderer.Characters.Length);

		if( fr.Denominator.ValuePast6 > 0 && fr.Denominator.Renderer.Characters.Length != 2 )
			Debug.LogError(messagePrefix + "Validate: Denominator is larger than 6, but has not 2 characters! " + fr.Denominator.Renderer.Characters.Length);
		

		if( fr.Numerator.Value != 0 )
		{
			if( fr.Numerator.ValuePast6 == 0 && fr.Numerator.Renderer.Characters.Length != 1 )
				Debug.LogError(messagePrefix + "Validate: Numerator is <= 6, but has not 1 character! " + fr.Numerator.Renderer.Characters.Length);
			
			if( fr.Numerator.Renderer.interactionCharacter == null )
				Debug.LogError(messagePrefix + "Validate: Numerator has no InteractionCharacter! " + fr.Numerator.Renderer.interactionCharacter);
		}

		if( fr.Denominator.Value != 0 )
		{
			if( fr.Denominator.ValuePast6 == 0 && fr.Denominator.Renderer.Characters.Length != 1 )
				Debug.LogError(messagePrefix + "Validate: Denominator is <= 6, but has not 1 character! " + fr.Denominator.Renderer.Characters.Length);

			if( fr.Denominator.Renderer.interactionCharacter == null )
				Debug.LogError(messagePrefix + "Validate: Denominator has no InteractionCharacter! " + fr.Denominator.Renderer.interactionCharacter);
		}

		//Debug.Log (messagePrefix + "Validate : function completed");
	}
}
