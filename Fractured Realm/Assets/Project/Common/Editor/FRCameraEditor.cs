using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(FRCamera))]
public class FRCameraEditor : Editor 
{
	
	protected bool showNumerators = true;
	protected bool showDenominators = true;
	
	protected bool showDefault = false;
	
	public override void OnInspectorGUI()
	{
		FRCamera subject = (FRCamera) target;
		
		//EditorGUIUtility.LookLikeInspector();

		if( GUILayout.Button("Numerator") )
		{
			HUDManager.use.SetMode( FR.Target.NUMERATOR );
		}
		else if( GUILayout.Button("Denominator") )
		{
			HUDManager.use.SetMode( FR.Target.DENOMINATOR );
		}
		else if( GUILayout.Button("Both") )
		{
			HUDManager.use.SetMode( FR.Target.BOTH );
		}
		/*
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
			
			FR.WorldType[] worldTypes = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
			foreach( FR.WorldType worldType in worldTypes )
			{
				if( worldType == FR.WorldType.NONE )
					continue;
				
				if( !subject.worldTypes.Contains(worldType) )
				{
					subject.worldTypes.Add(worldType);
					subject.worldTemplates.Add(null);
					subject.worldTemplates.Add(null);
				}
				
				EditorGUILayout.LabelField("" + worldType);
				int index = subject.GetTemplateIndexForType(worldType);
				WorldPart numerator = subject.worldTemplates[index];
				WorldPart denominator = subject.worldTemplates[index + 1];
				EditorGUILayout.LabelField("- Numerator:" + ((numerator == null) ? "???" : numerator.name) );
				EditorGUILayout.LabelField("- Denominator:" + ((denominator == null) ? "???" : denominator.name) );
			}
		}
		
		EditorGUILayout.LabelField("------------");
		
		FR.WorldType[] types = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
		foreach( FR.WorldType worldType in types )
		{
			if( worldType == FR.WorldType.NONE )
				continue;
			
			EditorGUILayout.LabelField("" + worldType);
			
			
			if( !subject.worldTypes.Contains(worldType) )
			{
				subject.worldTypes.Add(worldType);
				subject.worldTemplates.Add(null);
				subject.worldTemplates.Add(null);
			}
			
			int index = subject.GetTemplateIndexForType(worldType);
			WorldPart oldNumerator = subject.worldTemplates[ index ];
			WorldPart newNumerator = (WorldPart) EditorGUILayout.ObjectField( "- Numerator ", oldNumerator, typeof(WorldPart), false );
			
			if( oldNumerator != newNumerator )
			{
				subject.worldTemplates[ index ] = newNumerator;
			}
			
			WorldPart oldDenominator = subject.worldTemplates[ index + 1 ];
			WorldPart newDenominator = (WorldPart) EditorGUILayout.ObjectField( "- Denominator ", oldDenominator, typeof(WorldPart), false );
			
			if( oldDenominator != newDenominator )
			{
				subject.worldTemplates[ index + 1 ] = newDenominator;
			}
		}
		
		/*
		showNumerators = EditorGUILayout.Foldout(showNumerators, "Numerators");
		if( showNumerators )
		{
			for( int i = 0; i < 6; ++i )
			{
				Character oldCharacter = subject.Numerators[i];
				Character newCharacter = (Character) EditorGUILayout.ObjectField( "Numerator " + (i + 1), oldCharacter, typeof(Character), false );
				
				// TODO: perform some validation here (necessary components etc.)
				
				if( oldCharacter != newCharacter )
					subject.Numerators[i] = newCharacter;
			}
		}
		
		showDenominators = EditorGUILayout.Foldout(showDenominators, "Denominators");
		if(showDenominators)
		{
			for( int i = 0; i < 6; ++i )
			{
				Character oldCharacter = subject.Denominators[i];
				Character newCharacter = (Character) EditorGUILayout.ObjectField( "Denominator " + (i + 1), oldCharacter, typeof(Character), false );
				
				// TODO: perform some validation here (necessary components etc.)
				
				if( oldCharacter != newCharacter )
					subject.Denominators[i] = newCharacter;
			}
		}
		*/
		
		
	}
}