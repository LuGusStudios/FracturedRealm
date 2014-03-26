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

public class CharacterFactory : LugusSingletonExisting<CharacterFactory> 
{	
	public Character[] Numerators;
	public Character[] Denominators; 
	
	
	public FractionRenderer CreateRenderer(Fraction fraction)
	{
		FractionRenderer rend = new FractionRenderer();
		
		CreateRenderer( fraction.Numerator );
		CreateRenderer( fraction.Denominator );
		
		rend.Fraction = fraction;
		fraction.Renderer = rend;
		
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
		
		NumberRenderer newRenderer = CreateRenderer(newValue);
		newRenderer.transform.parent = renderer.transform.parent;
		newRenderer.transform.position = renderer.transform.position;
		newRenderer.transform.rotation = renderer.transform.rotation;
		
		
		newRenderer.interactionCharacter.transform.position = renderer.interactionCharacter.transform.position;
		newRenderer.interactionCharacter.transform.rotation = renderer.interactionCharacter.transform.rotation;
		
		
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
		//Debug.Log ("CharacterFactory : FreeRenderer : " + renderer.gameObject.name);
		GameObject.DestroyImmediate( renderer.gameObject );
	}
	
	public NumberRenderer CreateRenderer(Number number)
	{
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
		/*
		renderer.Characters[0] = (Character) GameObject.Instantiate( pool[ number.ValueTo6 - 1 ] );
		renderer.Characters[0].transform.eulerAngles = new Vector3(0, 180, 0);
		renderer.Characters[0].Number = number;
		*/
		renderer.Characters[0] = CreateCharacter( number, number.ValueTo6 );
		
		if( renderer.Characters[0] == null )
		{
			// invalid Number.Value : do not render
			
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
			renderer.Characters[1].transform.position += new Vector3(0, 140, 0);
			renderer.Characters[1].transform.localScale /= 2.0f;
			renderer.Characters[1].GetComponent<Animator>().SetBool("Floating", true);
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
		
		number.Renderer = renderer;
		renderer.Number = number;
		
		return renderer;
	}
	
	// ex CreacteCharacter(number, number.ValueTo6)
	// seems like passing the same thing twice, but number has multiple values, which each can have a character
	// we can't pass just the int value either, because we might want to use Number to get details about the fraction or world etc.
	public Character CreateCharacter(Number number, int value)
	{
		Character[] pool;
		if( number.IsNumerator )
			pool = Numerators;
		else 
			pool = Denominators;
		
		// TODO: add actual pooling and re-use of objects!
		value -= 1; // make it 0-based (use as index)
		if( value < 0 || value >= pool.Length )
		{
			Debug.LogError("CharacterFactory:CreateCharacter : value is not valid : 0 < " + value + " < " + pool.Length);
			
			return null;
		}
		
		Character newCharacter = (Character) GameObject.Instantiate( pool[ value ] );
		newCharacter.transform.eulerAngles = new Vector3(0, 180, 0);
		newCharacter.Value = value;
		
		return newCharacter;
	}
	
	
	public void FreeCharacter(Character character)
	{
		// TODO: add actual pooling and re-use of objects!
		GameObject.Destroy( character.gameObject );
	}
	
}
