using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RendererFactory))]
public class RendererFactoryEditor : Editor 
{
	protected bool showNumerators = true;
	protected bool showDenominators = true;
	
	protected bool showDefault = false;
	
	public override void OnInspectorGUI()
	{
		RendererFactory subject = (RendererFactory) target;
		
		EditorGUIUtility.LookLikeInspector();
		
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{
			DrawDefaultInspector();
		}
		
		EditorGUILayout.LabelField("------------");
		
		//DrawDefaultInspector();
		
		
		if( subject.Numerators == null || subject.Numerators.Length != 6 )
			subject.Numerators = new CharacterRenderer[6];
		
		if( subject.Denominators == null || subject.Denominators.Length != 6 )
			subject.Denominators = new CharacterRenderer[6];
	
		
		showNumerators = EditorGUILayout.Foldout(showNumerators, "Numerators");
		if( showNumerators )
		{
			for( int i = 0; i < 6; ++i )
			{
				CharacterRenderer oldCharacter = subject.Numerators[i];
				CharacterRenderer newCharacter = (CharacterRenderer) EditorGUILayout.ObjectField( "Numerator " + (i + 1), oldCharacter, typeof(CharacterRenderer), false );
				
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
				CharacterRenderer oldCharacter = subject.Denominators[i];
				CharacterRenderer newCharacter = (CharacterRenderer) EditorGUILayout.ObjectField( "Denominator " + (i + 1), oldCharacter, typeof(CharacterRenderer), false );
				
				// TODO: perform some validation here (necessary components etc.)
				
				if( oldCharacter != newCharacter )
					subject.Denominators[i] = newCharacter;
			}
		}
		
		
	}
}