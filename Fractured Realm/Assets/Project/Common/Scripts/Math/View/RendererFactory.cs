using UnityEngine;
using System.Collections;

/*
namespace FR
{
	public enum CharacterType
	{
		NONE,
		NUMERATOR,
		DENOMINATOR
	}
}
*/
using System.Collections.Generic;

public class RendererFactory : LugusSingletonExisting<RendererFactory> 
{	
	public CharacterRenderer[] Numerators;
	public CharacterRenderer[] Denominators; 

	public Portal[] Portals; // 0 is numerator, 1 is denominator

	public Portal CreatePortal( int value, FR.Target part )
	{
		Number number = new Number(value);
		number.IsNumerator = part.HasNumerator();
		number.IsDenominator = part.HasDenominator();

		return CreatePortal( number );
	}

	public Portal CreatePortal( Transform position, int value, FR.Target part )
	{
		Portal output = CreatePortal( value, part );

		Number number = new Number(value);
		number.IsNumerator = (part == FR.Target.NUMERATOR);
		
		// TODO: probably this has to be replaced with another visualization instead of just a new NumberRenderer...
		NumberRenderer numberR = CreateRenderer( number );

		numberR.transform.parent = output.transform;
		if( value <= 6 )
			numberR.transform.localPosition = new Vector3(3.0f, 1.2f, 0.0f);
		else
			numberR.transform.localPosition = new Vector3(3.0f, 0.2f, 0.0f);

		numberR.Animator.RotateInDirection( Vector3.back );

		foreach( CharacterRenderer c in numberR.Characters )
		{
			c.ShowBody(false);
		}

		foreach( Collider c in numberR.GetComponentsInChildren<Collider>() )
		{
			c.enabled = false;
		}

		numberR.enabled = false;
		GameObject.Destroy( numberR ); // remove the renderer component, other scripts are searching for these and shouldn't find this one

		numberR.transform.localScale = numberR.transform.localScale * 0.75f;

		output.transform.position = position.position;
		output.transform.parent = position.parent.parent; // position is portalEntry or portalExit, within an InteractionGroup, which is within numerator or denominator

		return output;
	}

	public Portal CreatePortal(Number number)
	{
		Portal prefab = null;
		if( number.IsNumerator )
		{
			if( Portals.Length > 0 )
			{
				prefab = Portals[0];
			}
			else
			{
				Debug.LogError("RendererFactory:CreatePortal : no portal prefab defined for numerator! index 0");
				return null;
			}
		}
		if( number.IsDenominator )
		{
			if( Portals.Length > 1 )
			{
				prefab = Portals[1];
			}
			else
			{
				Debug.LogError("RendererFactory:CreatePortal : no portal prefab defined for denominator! index 1");
				return null;
			}
		}

		// TODO: switch to correct mask visualisation for number.Value

		Portal portal = (Portal) GameObject.Instantiate( prefab );
		return portal;
	}

	public List<FractionRenderer> CreateRenderers(World world, Fraction[] fractions, int interactionGroupIndex = 0)
	{
		List<Fraction> fractionList = new List<Fraction>();
		fractionList.AddRange( fractions );

		return CreateRenderers(world, fractionList, interactionGroupIndex);
	}

	// use interactionGroupIndex = -1 to fill all interaction groups (debug purposes)
	public List<FractionRenderer> CreateRenderers(World world, List<Fraction> fractions, int interactionGroupIndex = 0)
	{
		List<FractionRenderer> output = new List<FractionRenderer>();

		// Spawn the LEFT fraction (fraction 0)
		int groupStart = interactionGroupIndex;
		int groupEnd = groupStart + 1;
		if( interactionGroupIndex == -1 )
		{
			if( world.numerator != null )
				groupEnd = world.numerator.InteractionGroups.Length;
			else
				groupEnd = world.denominator.InteractionGroups.Length;

			groupStart = 0;
		}
		
		//Debug.LogError("Creating world renderers " + groupStart + " -> " + groupEnd );
		
		for( int i = groupStart; i < groupEnd; ++i )
		{
			//Debug.LogError("Creating left renderers for interactionGroupIndex " + i );

			Fraction fr = new Fraction(fractions[0].Numerator.Value, fractions[0].Denominator.Value); 
			
			FractionRenderer frr = RendererFactory.use.CreateRenderer( fr );
			
			if( fr.Numerator.Value != 0 && (world.numerator != null) )
			{
				frr.Numerator.transform.parent = world.numerator.transform;
				frr.Numerator.SpawnPosition = world.numerator.InteractionGroups[i].Spawn1.position;
				frr.Numerator.transform.position = world.numerator.InteractionGroups[i].Spawn1.position;
				
				if( interactionGroupIndex == -1 ) // debug
				{
					//Portal p = RendererFactory.use.CreatePortal( fr.Numerator.Value, FR.Target.NUMERATOR );
					//p.transform.position = world.numerator.InteractionGroups[i].PortalEntry.position;
					//p.transform.parent = world.numerator.transform;

					CreatePortal( world.numerator.InteractionGroups[i].PortalEntry, fr.Numerator.Value, FR.Target.NUMERATOR );
				}
			}
			if( fr.Denominator.Value != 0 && (world.denominator != null) )
			{
				frr.Denominator.transform.parent = world.denominator.transform;
				frr.Denominator.SpawnPosition = world.denominator.InteractionGroups[i].Spawn1.position;
				frr.Denominator.transform.position = world.denominator.InteractionGroups[i].Spawn1.position;
				
				
				if( interactionGroupIndex == -1 ) // debug
				{
					CreatePortal( world.denominator.InteractionGroups[i].PortalEntry, fr.Denominator.Value, FR.Target.DENOMINATOR );

					//Portal p = RendererFactory.use.CreatePortal( fr.Denominator.Value, FR.Target.DENOMINATOR );
					//p.transform.position = world.denominator.InteractionGroups[i].PortalEntry.position;
					//p.transform.parent = world.denominator.transform;
				}
			}
			
			ValidateInternalFraction( fr,  "fraction 1 : ");
		}
		
		
		// Spawn the RIGHT fraction (fraction 1)
		for( int i = groupStart; i < groupEnd; ++i )
		{
			//Debug.LogError("Creating right renderers for interactionGroupIndex " + i );

			Fraction fr2 = new Fraction(fractions[1].Numerator.Value, fractions[1].Denominator.Value); 
			
			FractionRenderer frr2 = RendererFactory.use.CreateRenderer( fr2 );
			
			if( fr2.Numerator.Value != 0 && (world.numerator != null) )
			{
				frr2.Numerator.transform.parent = world.numerator.transform;
				frr2.Numerator.SpawnPosition = world.numerator.InteractionGroups[i].Spawn2.position;
				frr2.Numerator.transform.position = world.numerator.InteractionGroups[i].Spawn2.position;
				
				// TODO: calculate expected end-value for the portal
				if( interactionGroupIndex == -1 ) // debug
				{
					CreatePortal( world.numerator.InteractionGroups[i].PortalExit, fr2.Numerator.Value, FR.Target.NUMERATOR );

					//Portal p = RendererFactory.use.CreatePortal( fr2.Numerator.Value, FR.Target.NUMERATOR );
					//p.transform.position = world.numerator.InteractionGroups[i].PortalExit.position;
					//p.transform.parent = world.numerator.transform;
				}
			}
			if( fr2.Denominator.Value != 0 && (world.denominator != null) )
			{
				frr2.Denominator.transform.parent = world.denominator.transform; 
				frr2.Denominator.SpawnPosition = world.denominator.InteractionGroups[i].Spawn2.position;
				frr2.Denominator.transform.position = world.denominator.InteractionGroups[i].Spawn2.position;
				
				
				// TODO: calculate expected end-value for the portal
				
				if( interactionGroupIndex == -1 ) // debug
				{
					CreatePortal( world.denominator.InteractionGroups[i].PortalExit, fr2.Denominator.Value, FR.Target.DENOMINATOR );

					//Portal p = RendererFactory.use.CreatePortal( fr2.Denominator.Value, FR.Target.DENOMINATOR );
					//p.transform.position = world.denominator.InteractionGroups[i].PortalExit.position;
					//p.transform.parent = world.denominator.transform;
				}
				
			}
			
			ValidateInternalFraction( fr2, "fraction 2 : ");
		}
	
		return output;
	}

	
	public FractionRenderer CreateRenderer(Fraction fraction)
	{
		FractionRenderer rend = new FractionRenderer();
		FractionAnimator anim = new FractionAnimator();

		if( fraction.Numerator.Value != 0 )
			CreateRenderer( fraction.Numerator );
		
		if( fraction.Denominator.Value != 0 )
			CreateRenderer( fraction.Denominator );


		rend.Fraction = fraction;
		fraction.Renderer = rend;

		rend.Animator = anim;
		anim.Renderer = rend;
		
		return rend;
	}
	
	
	public NumberRenderer ReplaceRenderer(NumberRenderer renderer, Number newValue)
	{
		// Should only replace the renderer of the same number for now!!! 
		if( renderer.Number != newValue )
		{
			// TODO: remove Number newValue from the function definition and get it from the renderer directly!
			Debug.LogError("CharacterFactory:ReplaceRenderer: This only works when replacing the renderer for the same number!");
		}
		
		NumberRenderer newRenderer = null;

		if( newValue.Value != 0 )
		{
			// make sure we stick to the visualization of the current renderer (needed during division in the multiply visualizers)
			bool tempIsNumerator = newValue.IsNumerator;

			if( renderer.VisualizedAsNumerator )
				newValue.IsNumerator = true;
			else
				newValue.IsNumerator = false;

			newRenderer = CreateRenderer(newValue);
			newRenderer.transform.parent = renderer.transform.parent;
			newRenderer.transform.position = renderer.transform.position;
			newRenderer.transform.rotation = renderer.transform.rotation;
			
			
			newRenderer.interactionCharacter.transform.position = renderer.interactionCharacter.transform.position;
			newRenderer.interactionCharacter.transform.rotation = renderer.interactionCharacter.transform.rotation;

			newValue.IsNumerator = tempIsNumerator;
		}
		
		/*
		if( newRenderer.Characters[0] != null && renderer.Characters[0] != null )
		{
			newRenderer.Characters[0].transform.position = renderer.Characters[0].transform.position;
			newRenderer.Characters[0].transform.rotation = renderer.Characters[0].transform.rotation;
		}
		
		if( newRenderer.Characters[1] != null && renderer.Characters[1] != null )
		{
			newRenderer.Characters[1].transform.position = renderer.Characters[1].transform.position;
			newRenderer.Characters[1].transform.rotation = renderer.Characters[1].transform.rotation;
		}
		*/
		
		/*
		// we go from a value above 6 to a value 6 or lower
		// this means there were 2 characters, now there is only 1 left
		if( newRenderer.Characters[1] == null && renderer.Characters[1] != null )
		{
			newRenderer.Characters[0].transform.position = renderer.Characters[1].transform.position;
			newRenderer.Characters[0].transform.rotation = renderer.Characters[1].transform.rotation;
			
			//newRenderer.Characters[0].transform.localScale *= 2.0f;
			//newRenderer.Characters[0].GetComponent<Animator>().SetBool("Floating", false);
			//newRenderer.Characters[0].ShowBody(true);
		}
		*/
		
		// TODO: what if newRenderer has 1 but old one doesn't... can that ever happen? 
		
		FreeRenderer(renderer);
		
		return newRenderer; 
	}
	
	public void FreeRenderer(FractionRenderer renderer)
	{
		FreeRenderer( renderer.Numerator );
		FreeRenderer( renderer.Denominator );
	}
	
	public void FreeRenderer(NumberRenderer renderer)
	{
		if( renderer == null )
			return;

		//Debug.Log ("CharacterFactory : FreeRenderer : " + renderer.gameObject.name);
		GameObject.Destroy/*Immediate*/( renderer.gameObject );
	}
	
	public NumberRenderer CreateRenderer(Number number)
	{
		if( number.Value == 0 )
		{
			Debug.LogError("CharacterFactory:CreateRenderer : number value was 0. No renderer created.");
			return null;
		}

		/*
		Character[] pool;
		if( number.IsNumerator )
			pool = Numerators;
		else 
			pool = Denominators;
			*/
		
		// TODO: add actual pooling and re-use of objects!
		
		/*
		int value = number.Value;
		value -= 1; // make it 0-based (use as index)
		if( value < 0 || value >= pool.Length )
		{
			Debug.LogError("CharacterFactory:CreateCharacter : value is not valid : 0 < " + value + " < " + pool.Length);
			value = 0;
		}
		*/
		
		GameObject nrGO = new GameObject("NR_" + number.Value);
		NumberRenderer renderer = nrGO.AddComponent<NumberRenderer>();
		NumberAnimator animator = nrGO.AddComponent<NumberAnimator>(); 
		/*
		renderer.Characters[0] = (Character) GameObject.Instantiate( pool[ number.ValueTo6 - 1 ] );
		renderer.Characters[0].transform.eulerAngles = new Vector3(0, 180, 0);
		renderer.Characters[0].Number = number;
		*/

		if( number.IsNumerator )
			renderer.VisualizedAsNumerator = true;
		else
			renderer.VisualizedAsDenominator = true;

		number.Renderer = renderer;
		renderer.Number = number;

		renderer.Characters[0] = CreateCharacter( number, number.ValueTo6 );
		
		if( renderer.Characters[0] == null )
		{
			// invalid Number.Value : do not render
			
			Debug.LogError("CharacterFactory:CreateRenderer : renderer.Characters[0] was null! Shouldn't happen!");

			number.Renderer = null;
			GameObject.Destroy( nrGO );
			return null;
		}
		
		renderer.Characters[0].transform.parent = renderer.transform;
		renderer.Characters[0].transform.position = Vector3.zero;
		
		renderer.Characters[0].NumberRenderer = renderer;
		
		if( number.ValuePast6 != 0 ) 
		{
			//renderer.Characters[1] = (Character) GameObject.Instantiate( pool[ number.ValuePast6 - 1 ] );
			
			renderer.Characters[1] = renderer.Characters[0];
			renderer.Characters[1].transform.position += new Vector3(0, 3.5f, 0);
			renderer.Characters[1].transform.localScale /= 2.0f;
			renderer.Characters[1].GetComponent<Animator>().SetBool("floating", true); // TODO: make floating a looping state!
			renderer.Characters[1].GetComponent<Animator>().SetTrigger("floating");
			renderer.Characters[1].ShowBody(false);
			
			
			renderer.Characters[0] = CreateCharacter( number, number.ValuePast6 );
			renderer.Characters[0].transform.parent = renderer.transform;
			renderer.Characters[0].transform.position = Vector3.zero;
			
			renderer.Characters[0].NumberRenderer = renderer;
			
			/*
			renderer.Characters[1] = CreateCharacter( number, number.ValuePast6 );
			
			renderer.Characters[1].transform.parent = renderer.transform;
			renderer.Characters[1].transform.position = Vector3.zero;
			
			
			
			
			// position next to eachother
			
			//renderer.Characters[0].transform.position += new Vector3(-120 * LuGusUtil.UNITS, 0, 0);
			//renderer.Characters[1].transform.position = new Vector3(120 * LuGusUtil.UNITS, 0, 0);
			
			
			// biggest number floating above smaller number
			renderer.Characters[0].transform.position += new Vector3(0, 140 * LuGusUtil.UNITS, 0);
			renderer.Characters[0].transform.localScale /= 2.0f;
			renderer.Characters[0].GetComponent<Animator>().SetBool("Floating", true);
			renderer.Characters[0].ShowBody(false);
			*/
			
			
		}
		
		//Debug.Log ("TEST REFERENCE : " + renderer.Characters[0].name);
		
		// TODO: characters should automatically be rotated the correct way!

		
		return renderer;
	}
	
	// ex CreacteCharacter(number, number.ValueTo6)
	// seems like passing the same thing twice, but number has multiple values, which each can have a character
	// we can't pass just the int value either, because we might want to use Number to get details about the fraction or world etc.
	public CharacterRenderer CreateCharacter(Number number, int value)
	{
		CharacterRenderer[] pool;

		if( number.Renderer != null )
		{
			if( number.Renderer.VisualizedAsNumerator )
				pool = Numerators;
			else
				pool = Denominators;
		}
		else
		{
			if( number.IsNumerator )
				pool = Numerators;
			else 
				pool = Denominators;
		}

		// TODO: add actual pooling and re-use of objects!
		value -= 1; // make it 0-based (use as index)
		if( number.Value == 0 || value < 0 || value >= pool.Length ) 
		{
			Debug.LogError("CharacterFactory:CreateCharacter : value is not valid : 0 < " + value + " < " + pool.Length + ". Value clamped.");

			if( number.Value == 0 || value < 0 )
				value = 1;
			else if( value >= pool.Length )
				value = pool.Length - 1;
		}
		
		CharacterRenderer newCharacter = (CharacterRenderer) GameObject.Instantiate( pool[ value ] );
		newCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		newCharacter.Value = value;

		/*
		newCharacter.Animator = newCharacter.GetComponent<CharacterAnimator>();
		if( newCharacter.Animator == null )
		{
			newCharacter.Animator = newCharacter.gameObject.AddComponent<CharacterAnimator>();
		}
		*/

		if( newCharacter.Animator == null )
			newCharacter.gameObject.AddComponent<CharacterAnimator>();

		
		return newCharacter;
	}
	
	
	public void FreeCharacter(CharacterRenderer character)
	{
		// TODO: add actual pooling and re-use of objects!
		GameObject.Destroy( character.gameObject );
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
