using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	public enum WorldType
	{
		NONE,
		FOREST,
		DESERT
	}
}

public class WorldFactory : LugusSingletonExisting<WorldFactory> 
{	
	public void Start()
	{
		// TODO: CHANGE!
		CreateWorld(FR.WorldType.FOREST);
		
	}
	
	// Dictionary<FR.WorldType, List<WorldPart>> is NOT SERIALIZABLE by unity!
	// so we need to use 2 lists and GetTemplateIndexForType() to get the same functionality
	public List<FR.WorldType> worldTypes = new List<FR.WorldType>();
	public List<WorldPart> worldTemplates = new List<WorldPart>();
	
	//public Dictionary<FR.WorldType, List<WorldPart>> worldTemplates = new Dictionary<FR.WorldType, List<WorldPart>>();
	
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
	
	public Fraction[] debug_initialFractions;
	public int test = 5;
	public World CreateWorld(FR.WorldType type)
	{
		if( debug_initialFractions == null || debug_initialFractions.Length == 0 )
		{
			debug_initialFractions = new Fraction[2];
			debug_initialFractions[0] = new Fraction(1,11);
			debug_initialFractions[1] = new Fraction(7,4);
		}
		
		return CreateWorld(type, debug_initialFractions);
	}
	
	public World CreateWorld(FR.WorldType type, Fraction[] fractions) 
	{
		if( !worldTypes.Contains(type) )
		{
			Debug.LogError("WorldFactory:CreateWorld : unknown worldtype : " + type);
			return null;
		}
		
		// worlds consist of 2 separate entities: numerator and denominator parts
		// each has its own camera and setup
		
		int index = GetTemplateIndexForType(type);
		
		GameObject WORLD = GameObject.Find("WORLD");
		if( WORLD != null )
			GameObject.DestroyImmediate( WORLD );
			
		WORLD = new GameObject("WORLD");
		WORLD.transform.position = new Vector3(2000, 0, 0);
		
		World world = WORLD.AddComponent<World>();
		world.numerator = (WorldPart) GameObject.Instantiate( worldTemplates[ index ] );
		world.denominator = (WorldPart) GameObject.Instantiate( worldTemplates[ index + 1 ] ); 
		
		world.numerator.transform.parent = WORLD.transform;
		world.denominator.transform.parent = WORLD.transform;
		
		world.numerator.transform.localPosition = new Vector3(0, 2000, 0);
		world.denominator.transform.localPosition = new Vector3(0, 0, 0);
		
		Fraction fr = new Fraction(fractions[0].Numerator.Value, fractions[0].Denominator.Value); 
		
		FractionRenderer frr = CharacterFactory.use.CreateRenderer( fr );
		frr.Numerator.transform.parent = world.numerator.transform;
		frr.Numerator.transform.position = world.numerator.SpawnLeft.position;
		frr.Denominator.transform.parent = world.denominator.transform;
		frr.Denominator.transform.position = world.denominator.SpawnLeft.position;
		
		/*
		NumberRenderer c = CharacterFactory.use.CreateRenderer( fr.Numerator );
		c.transform.parent = world.numerator.transform;
		c.transform.position = world.numerator.SpawnLeft.position;
		 
		c = CharacterFactory.use.CreateRenderer( fr.Denominator );
		c.transform.parent = world.denominator.transform;
		c.transform.position = world.denominator.SpawnLeft.position;
		*/
		
		
		Fraction fr2 = new Fraction(fractions[1].Numerator.Value, fractions[1].Denominator.Value); 
		
		FractionRenderer frr2 = CharacterFactory.use.CreateRenderer( fr2 );
		frr2.Numerator.transform.parent = world.numerator.transform;
		frr2.Numerator.transform.position = world.numerator.SpawnRight.position;
		frr2.Denominator.transform.parent = world.denominator.transform;
		frr2.Denominator.transform.position = world.denominator.SpawnRight.position;
		
		/*
		c = CharacterFactory.use.CreateRenderer( fr2.Numerator );
		c.transform.parent = world.numerator.transform;
		c.transform.position = world.numerator.SpawnRight.position;
		
		c = CharacterFactory.use.CreateRenderer( fr2.Denominator );
		c.transform.parent = world.denominator.transform;
		c.transform.position = world.denominator.SpawnRight.position;
		*/
		
		LugusCamera.numerator.transform.position = world.numerator.transform.position + new Vector3(512, 192, -300);
		LugusCamera.denominator.transform.position = world.denominator.transform.position + new Vector3(512,192,-300);
		
		return world;
	}
	
	public void FreeWorld(Character character)
	{
		// TODO: add actual pooling and re-use of objects!
		GameObject.Destroy( character.gameObject );
	}
}
