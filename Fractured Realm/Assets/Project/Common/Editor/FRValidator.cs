// C# example:
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

public class FRValidator : EditorWindow 
{
    [MenuItem ("FR/FRValidator")]
    static void Init () 
	{
        FRValidator window = (FRValidator)EditorWindow.GetWindow (typeof (FRValidator));
	}
	
	[SerializeField]
	int fr1_numerator = 1;
	[SerializeField]
	int fr1_denominator = 1;
	
	[SerializeField]
	int fr2_numerator = 1;
	[SerializeField]
	int fr2_denominator = 1;
	
	void OnGUI()
	{
		bool validateAll = false;	
		if( GUILayout.Button("Validate All") )
		{
			validateAll = true;
		}
		
		if( validateAll || GUILayout.Button("Validate Characters") )
		{
			ValidateCharacterFactory();
		}
		
		if( validateAll || GUILayout.Button("Validate Worlds") )
		{
			ValidateWorldFactory();
		}
		
		if( validateAll || GUILayout.Button("Validate Effects") )
		{
			ValidateEffectsFactory();
		}
		
		if( validateAll || GUILayout.Button("Validate Scene Setup") )
		{
			ValidateScene();
		}
		
		GUILayout.Label("-------------------");
		GUILayout.Label("-------------------");
		GUILayout.Label("-------------------");
		
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		
		fr1_numerator = EditorGUILayout.IntField(fr1_numerator);
		fr1_denominator = EditorGUILayout.IntField(fr1_denominator);
		
		/*
		if( fr1_numerator == 0 || fr1_denominator == 0 )
		{
			fr1_numerator = 0;
			fr1_denominator = 0;
		}
		*/
		
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		
		fr2_numerator = EditorGUILayout.IntField(fr2_numerator);
		fr2_denominator = EditorGUILayout.IntField(fr2_denominator);
		
		/*
		if( fr2_numerator == 0 || fr2_denominator == 0 )
		{
			fr2_numerator = 0;
			fr2_denominator = 0;
		}
		*/
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		
		if( GUILayout.Button("Spawn Numbers") )
		{
			Fraction[] fractions = new Fraction[2];
			fractions[0] = new Fraction( fr1_numerator, fr1_denominator );
			fractions[1] = new Fraction( fr2_numerator, fr2_denominator );
			
			WorldFactory.use.debug_initialFractions = fractions;
			EditorUtility.SetDirty(WorldFactory.use);
			
			WorldFactory.use.CreateFractions(fractions);
		}

		if( GUILayout.Button("Create Forest") )
		{
			Fraction[] fractions = new Fraction[2];
			fractions[0] = new Fraction( fr1_numerator, fr1_denominator );
			fractions[1] = new Fraction( fr2_numerator, fr2_denominator );
			
			WorldFactory.use.debug_initialFractions = fractions;
			EditorUtility.SetDirty(WorldFactory.use);
			
			WorldFactory.use.CreateWorld(FR.WorldType.FOREST, fractions);
		}
		
		if( GUILayout.Button("Create Desert") )
		{
			Fraction[] fractions = new Fraction[2];
			fractions[0] = new Fraction( fr1_numerator, fr1_denominator );
			fractions[1] = new Fraction( fr2_numerator, fr2_denominator );
			
			WorldFactory.use.debug_initialFractions = fractions;
			EditorUtility.SetDirty(WorldFactory.use);
			
			WorldFactory.use.CreateWorld(FR.WorldType.DESERT, fractions);
		}
	}
	
	public void ValidateCharacterFactory()
	{
		Debug.Log("START ValidateCharacterFactory");
		
		CharacterFactory cf = CharacterFactory.use; 
		if( cf == null )
		{
			Debug.LogError("FRValidator:ValidateCharacterFactory : cf is null!");
			return;
		}
		
		Logus.Assert( cf.Numerators.Length == 6 );
		Logus.Assert( cf.Denominators.Length == 6 );
		
		int i = 1;
		foreach( Character go in cf.Numerators )
		{
			if( go == null )
				Debug.LogError("FRValidator:ValidateCharacterFactory : Numerator " + i + " is null!" );
			
			i++;
		}
		
		i = 1;
		foreach( Character go in cf.Denominators )
		{
			if( go == null )
				Debug.LogError("FRValidator:ValidateCharacterFactory : Denominator " + i + " is null!" );
			
			i++;
		}
		
		Debug.Log("END ValidateCharacterFactory");
	}
	
	public void ValidateWorldFactory()
	{
		Debug.Log("START ValidateWorldFactory");
		
		Logus.Assert( WorldFactory.use.worldTemplates.Count == (WorldFactory.use.worldTypes.Count * 2) );
		
		FR.WorldType[] types = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
		foreach( FR.WorldType type in types )
		{
			if( type == FR.WorldType.NONE )
				continue;
			
			
			Debug.Log("CHECK " + type);
			
			if( !WorldFactory.use.worldTypes.Contains(type) )
			{
				Debug.LogError("FRValidator:ValidateWorldFactory : factory doesn't contain parts for type " + type);
			}
			else
			{
				int index = WorldFactory.use.GetTemplateIndexForType(type);
				WorldPart numerator = WorldFactory.use.worldTemplates[index];
				WorldPart denominator = WorldFactory.use.worldTemplates[index + 1];
					
				if( Logus.Assert( numerator != null) )
				{
					if( Logus.Assert( numerator.InteractionPointsContainer != null ) )
					{
						Logus.Assert( numerator.SpawnLeft != null );
						Logus.Assert( numerator.SpawnRight != null );
					}
				}
				
				if( Logus.Assert( denominator != null) )
				{
					if( Logus.Assert( denominator.InteractionPointsContainer != null ) )
					{
						Logus.Assert( denominator.SpawnLeft != null );
						Logus.Assert( denominator.SpawnRight != null );
					}
				}
			}
		}
		
		
		Debug.Log("END ValidateWorldFactory");
	}
	
	public void ValidateEffectsFactory()
	{
		Debug.Log("START ValidateEffectsFactory");
		
		Logus.Assert( EffectFactory.use.effects.Count == (EffectFactory.use.effectTypes.Count * 2) );
		
		FR.EffectType[] types = (FR.EffectType[]) Enum.GetValues(typeof(FR.EffectType));
		foreach( FR.EffectType type in types )
		{
			if( type == FR.EffectType.NONE )
				continue;
			
			Debug.Log("CHECK " + type);
			
			if( !EffectFactory.use.effectTypes.Contains(type) )
			{ 
				Debug.LogError("FRValidator:ValidateEffectsFactory : factory doesn't contain definition for type " + type);
			}
			else
			{
				int index = EffectFactory.use.GetEffectIndexForType(type);
				
				Effect normal = EffectFactory.use.effects[index];
				Effect spirit = EffectFactory.use.effects[index + 1];
					
				if( Logus.Assert( normal != null) )
				{
				}
				
				if( Logus.Assert( spirit != null) )
				{
				}
			}
		}
		
		
		Debug.Log("END ValidateEffectsFactory");
	}
	
	public void ValidateScene()
	{
		Debug.Log("START ValidateScene");
		
		Logus.Assert( LugusCamera.numerator != null );
		Logus.Assert( LugusCamera.denominator != null );
		
		Debug.Log("END ValidateScene");
	}
}








