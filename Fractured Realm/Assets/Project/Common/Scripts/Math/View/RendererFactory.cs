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
			newRenderer = CreateRenderer(newValue);
			newRenderer.transform.parent = renderer.transform.parent;
			newRenderer.transform.position = renderer.transform.position;
			newRenderer.transform.rotation = renderer.transform.rotation;
			
			
			newRenderer.interactionCharacter.transform.position = renderer.interactionCharacter.transform.position;
			newRenderer.interactionCharacter.transform.rotation = renderer.interactionCharacter.transform.rotation;
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
		if( number.IsNumerator )
			pool = Numerators;
		else 
			pool = Denominators;
		
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
	
}
