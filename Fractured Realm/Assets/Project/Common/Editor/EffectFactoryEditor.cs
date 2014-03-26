using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(EffectFactory))]
public class EffectFactoryEditor : Editor 
{
	protected bool showDefault = false;
	
	public override void OnInspectorGUI()
	{
		EffectFactory subject = (EffectFactory) target;
		
		EditorGUIUtility.LookLikeInspector();
		
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
			
			FR.EffectType[] effectTypes = (FR.EffectType[]) Enum.GetValues(typeof(FR.EffectType));
			foreach( FR.EffectType effectType in effectTypes )
			{
				if( effectType == FR.EffectType.NONE )
					continue;
				
				if( !subject.effectTypes.Contains(effectType) )
				{
					subject.effectTypes.Add(effectType);
					subject.effects.Add(null);
					subject.effects.Add(null);
				}
				
				
				int index = subject.GetEffectIndexForType(effectType);
				EditorGUILayout.LabelField("" + effectType);
				Effect normal = subject.effects[index];
				Effect spirit = subject.effects[index + 1];
				EditorGUILayout.LabelField("- Normal:" + ((normal == null) ? "???" : normal.name) );
				EditorGUILayout.LabelField("- Spirit:" + ((spirit == null) ? "???" : spirit.name) );
			}
		}
		
		EditorGUILayout.LabelField("------------");
	
		FR.EffectType[] effectTypes2 = (FR.EffectType[]) Enum.GetValues(typeof(FR.EffectType));
		foreach( FR.EffectType effectType in effectTypes2 )
		{
			if( effectType == FR.EffectType.NONE )
					continue;
				
			EditorGUILayout.LabelField("" + effectType);
			
			
			if( !subject.effectTypes.Contains(effectType) )
			{
				subject.effectTypes.Add(effectType);
				subject.effects.Add(null);
				subject.effects.Add(null);
			}
			
			
			int index = subject.GetEffectIndexForType(effectType);
			Effect oldEffect = subject.effects[ index ];
			Effect newEffect = (Effect) EditorGUILayout.ObjectField( "- Normal : ", oldEffect, typeof(Effect), false );
			
			if( oldEffect != newEffect )
			{
				subject.effects[ index ] = newEffect;
			}
			
			
			oldEffect = subject.effects[ index + 1 ];
			newEffect = (Effect) EditorGUILayout.ObjectField( "- Spirit : ", oldEffect, typeof(Effect), false );
			
			if( oldEffect != newEffect )
			{
				subject.effects[ index + 1 ] = newEffect;
			}
		}
		
		
	}
}