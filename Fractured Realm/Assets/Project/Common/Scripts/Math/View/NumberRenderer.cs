using UnityEngine;
using System.Collections;

public class NumberRenderer : MonoBehaviour
{
	// A Number can be rendered with 2 characters at this moment
	// this is because we only have characters up to the number 6, while we want to display numbers up to 12
	protected Character[] _characters = null;
	public Character[] Characters
	{
		get{ InitiateRenderers(); return _characters; } // this is passed by reference, like we expect
		set{ _characters = value; }
	}
	
	private void InitiateRenderers()
	{
		if( _characters != null )
			return;
		
		_characters = new Character[2];
		_characters[0] = null;
		_characters[1] = null;
	}
	
	public Character interactionCharacter
	{
		get
		{
			InitiateRenderers();
			
			return _characters[0];
			
			/*
			if( _characters[1] == null )
				return _characters[0]; 
			else
				return _characters[1];
			*/
		}
	}
	
	/*
	public void UpdateRenderers()
	{
		foreach( Character c in _characters )
		{
			// TODO
		}
	}
	*/
	
	public void NumberValueChanged()
	{
		// this renderer object is effectively removed by this call!
		// as the CharacterFactory will create a completely new NumberRenderer for the new value
		CharacterFactory.use.ReplaceRenderer(this, _number);
	}
	
	protected Number _number = null;
	public Number Number
	{
		get{ return _number; }
		set{ _number = value; }
	}
	
	public FractionRenderer FractionRenderer
	{
		get
		{
			if( Number == null )
			{
				Debug.LogError("NumberRenderer:FractionRenderer get : no Number known!");
				return null;
			}
			
			return Number.Fraction.Renderer;
		}
	}
	
	/*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	*/
}
