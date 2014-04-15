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
		ICE
	}
}

public class WorldFactory : LugusSingletonExisting<WorldFactory> 
{	
	public void Start()
	{
		// TODO: CHANGE!
		CreateWorld(FR.WorldType.DESERT);
		
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
		WORLD.transform.position = new Vector3(0, 0, 0);
		
		World world = WORLD.AddComponent<World>();
		world.numerator = (WorldPart) GameObject.Instantiate( worldTemplates[ index ] );
		world.denominator = (WorldPart) GameObject.Instantiate( worldTemplates[ index + 1 ] ); 
		
		world.numerator.transform.parent = WORLD.transform;
		world.denominator.transform.parent = WORLD.transform;
		
		world.numerator.transform.position   = new Vector3(LugusCamera.numerator.transform.position.x, LugusCamera.numerator.transform.position.y, LugusCamera.numerator.transform.position.z + 15.0f);
		world.denominator.transform.position = new Vector3(LugusCamera.denominator.transform.position.x, LugusCamera.denominator.transform.position.y,  LugusCamera.denominator.transform.position.z + 15.0f);
		
		Fraction fr = new Fraction(fractions[0].Numerator.Value, fractions[0].Denominator.Value); 
		
		FractionRenderer frr = CharacterFactory.use.CreateRenderer( fr );

		if( fr.Numerator.Value != 0 )
		{
			frr.Numerator.transform.parent = world.numerator.transform;
			frr.Numerator.transform.position = world.numerator.SpawnLeft.position;
		}
		if( fr.Denominator.Value != 0 )
		{
			frr.Denominator.transform.parent = world.denominator.transform;
			frr.Denominator.transform.position = world.denominator.SpawnLeft.position;
		}
		
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
		
		if( fr2.Numerator.Value != 0 )
		{
			frr2.Numerator.transform.parent = world.numerator.transform;
			frr2.Numerator.transform.position = world.numerator.SpawnRight.position;
		}
		if( fr2.Denominator.Value != 0 )
		{
			frr2.Denominator.transform.parent = world.denominator.transform;
			frr2.Denominator.transform.position = world.denominator.SpawnRight.position;
		}
		
		/*
		c = CharacterFactory.use.CreateRenderer( fr2.Numerator );
		c.transform.parent = world.numerator.transform;
		c.transform.position = world.numerator.SpawnRight.position;
		
		c = CharacterFactory.use.CreateRenderer( fr2.Denominator );
		c.transform.parent = world.denominator.transform;
		c.transform.position = world.denominator.SpawnRight.position;
		*/
		
		//LugusCamera.numerator.transform.position = world.numerator.transform.position + new Vector3(512, 192, -300);
		//LugusCamera.denominator.transform.position = world.denominator.transform.position + new Vector3(512,192,-300);

		
		ValidateInternalFraction( fr,  "fraction 1 : ");
		ValidateInternalFraction( fr2, "fraction 2 : ");

		return world;
	}

	// TODO: remove, just for debugging
	public void CreateFractions(Fraction[] fractions)
	{
		Fraction fr = new Fraction(fractions[0].Numerator.Value, fractions[0].Denominator.Value); 
		
		FractionRenderer frr = CharacterFactory.use.CreateRenderer( fr );
		if( fractions[0].Numerator.Value != 0 )
			frr.Numerator.transform.position = new Vector3(0,0,0);
		if( fractions[0].Denominator.Value != 0 )
			frr.Denominator.transform.position = new Vector3(0,-5,0);
		
		
		Fraction fr2 = new Fraction(fractions[1].Numerator.Value, fractions[1].Denominator.Value); 
		
		FractionRenderer frr2 = CharacterFactory.use.CreateRenderer( fr2 );
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

		Debug.Log (messagePrefix + "Validate : function completed");

	}
	
	public void FreeWorld(Character character)
	{
		// TODO: add actual pooling and re-use of objects!
		GameObject.Destroy( character.gameObject );
	}
}
