using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(WorldFactory))]
public class WorldFactoryEditor : Editor 
{
	
	protected bool showNumerators = true;
	protected bool showDenominators = true;
	
	protected bool showDefault = false;

	protected WorldPart newItem = null;
	
	public override void OnInspectorGUI()
	{
		WorldFactory subject = (WorldFactory) target;
		
		EditorGUIUtility.LookLikeInspector();
		
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
			
			FR.WorldType[] worldTypes = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
			foreach( FR.WorldType worldType in worldTypes )
			{
				if( worldType == FR.WorldType.NONE )
					continue;

				/*
				if( !subject.worldTypes.Contains(worldType) )
				{
					subject.worldTypes.Add(worldType);
					subject.worldTemplates.Add(null);
					subject.worldTemplates.Add(null);
				}
				*/

				/*
				EditorGUILayout.LabelField("" + worldType);
				int index = subject.GetTemplateIndexForType(worldType);
				WorldPart numerator = subject.worldTemplates[index];
				WorldPart denominator = subject.worldTemplates[index + 1];
				EditorGUILayout.LabelField("- Numerator:" + ((numerator == null) ? "???" : numerator.name) );
				EditorGUILayout.LabelField("- Denominator:" + ((denominator == null) ? "???" : denominator.name) );
				*/
			}
		}
		
		EditorGUILayout.LabelField("------------");


		FR.WorldType[] types = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
		foreach( FR.WorldType worldType in types )
		{
			if( worldType == FR.WorldType.NONE )
				continue;
			
			EditorGUILayout.LabelField("" + worldType);

			List<WorldPart> parts = subject.GetWorldParts( worldType );

			if( parts == null )
			{
				parts = new List<WorldPart>();
			}

			List<WorldPart> toRemove = new List<WorldPart>();
			foreach( WorldPart part in parts )
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField( "", part, typeof(WorldPart), false );

				if( GUILayout.Button("REMOVE") )
				{
					toRemove.Add( part );
				}

				GUILayout.EndHorizontal();
			}

			foreach( WorldPart part in toRemove )
			{
				parts.Remove(part);
			}

			GUILayout.BeginHorizontal();

			newItem = (WorldPart) EditorGUILayout.ObjectField( "", newItem, typeof(WorldPart), false );
			if( GUILayout.Button("ADD") )
			{
				parts.Add( newItem );
				newItem = null;

				EditorUtility.SetDirty( subject );
			}

			GUILayout.EndHorizontal();
		}

		/*
		EditorGUILayout.LabelField("------------");
		EditorGUILayout.LabelField("------------");


	
		//FR.WorldType[] types = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
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
		*/
		
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