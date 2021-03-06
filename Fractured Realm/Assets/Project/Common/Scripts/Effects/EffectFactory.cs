using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FR
{
	public enum EffectType
	{
		NONE,
		
		FIRE_BALL,
		FIRE_HIT,
		
		JOIN_HIT,
		
		SHIELD
	}
}

public class EffectFactory : LugusSingletonExisting<EffectFactory> 
{	
	public List<FR.EffectType> effectTypes = new List<FR.EffectType>();
	public List<Effect> effects = new List<Effect>();
	
	public int GetEffectIndexForType(FR.EffectType type)
	{
		if( !effectTypes.Contains(type) )
		{
			Debug.LogError("EffectFactory:GetEffectIndexForType : unknown effecttype : " + type);
			return -1;
		}
		
		int index = effectTypes.IndexOf( type );
		return (index * 2); // 2 effects per type (normal and spirit world)
	}
	
	public Effect[] CreateEffects( FR.EffectType type )
	{
		Effect[] output = new Effect[2];
		//int index = GetEffectIndexForType(type);
		
		output[0] = CreateEffectNormal(type);
		output[1] = CreateEffectSpirit(type);
		
		return output;
	}

	public Effect[] CreateEffects( FR.Target selector, FR.EffectType type )
	{
		Effect[] output = new Effect[2];
		//int index = GetEffectIndexForType(type);

		if( selector.HasNumerator() )
			output[0] = CreateEffectNormal(type);
		else
			output[0] = null;

		if( selector.HasDenominator() )
			output[1] = CreateEffectSpirit(type);
		else
			output[1] = null;
		
		return output;
	}
	
	public Effect CreateEffectNormal(FR.EffectType type)
	{
		int index = GetEffectIndexForType(type);
		return (Effect) GameObject.Instantiate( effects[index] );
	}
	
	public Effect CreateEffectSpirit(FR.EffectType type)
	{
		int index = GetEffectIndexForType(type);
		return (Effect) GameObject.Instantiate( effects[index + 1] );
	}

	public Effect CreateEffect( FR.Target side, FR.EffectType type )
	{
		if( side.HasNumerator() )
			return CreateEffectNormal( type );
		else
			return CreateEffectSpirit( type );
	}
	
	public void FreeEffect( Effect effect )
	{
		GameObject.Destroy( effect.gameObject );
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
