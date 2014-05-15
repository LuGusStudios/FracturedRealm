using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(PropFactory))]
public class PropFactoryEditor : Editor 
{
	protected bool showDefault = false;
	protected string prefabPath = "ThemeAssets/Props/Prefabs/";
	
	public override void OnInspectorGUI()
	{
		PropFactory subject = (PropFactory) target;
		
		DrawDefaultInspector();

		/*
		showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		if( showDefault )
		{

		}
		*/
		
		EditorGUILayout.LabelField("------------");

		EditorGUILayout.LabelField("Prefab folder:");
		prefabPath = (string) EditorGUILayout.TextField( prefabPath );

		if( GUILayout.Button("\nFill from prefabs\n") )
		{
			subject.props = new List<Prop>();

			string basePath = Application.dataPath + "/" + prefabPath;
			string outputPath = Application.dataPath + "/Project/Common/Scripts/Characters/Props/";

			// read all prefabs
			if( !Directory.Exists( basePath ) )
			{
				Debug.LogError("PropFactory:fill : prefabpath doesn't exist! " + basePath);
				return;
			}

			DirectoryInfo directoryFolder = new DirectoryInfo( basePath );

			string enumString = "\n";
			int propCounter = 1;

			FileInfo[] prefabs = directoryFolder.GetFiles();
			foreach( FileInfo prefab in prefabs )
			{
				if( prefab.Name.Contains(".meta") )
					continue;

				//Debug.Log ("Checking prefab : " + ("Assets/" + prefabPath + prefab.Name) );

				Prop prop = (Prop) AssetDatabase.LoadAssetAtPath( "Assets/" + prefabPath + prefab.Name, typeof(Prop) );

				if( prop == null )
				{
					Debug.LogError("PropFactoryEditor:fill : prefab didn't have a Prop script on it! " + prefab.Name);
				}
				else
				{
					subject.props.Add( prop );

					enumString += "\t\t" + prefab.Name.Replace(".prefab", "") + " = " + propCounter + ",\n";
					++propCounter;
				}
			}

			EditorUtility.SetDirty( subject );


			// write FRPropType enum to file
			string template = File.ReadAllText( outputPath + "FRPropTypeTemplate.cs" );
			template = template.Replace( "/*", "" );
			template = template.Replace( "*/", "" );
			
			template = template.Replace("ENUM", enumString );
			
			if( File.Exists( outputPath + "FRPropType.cs" ) )
			{
				File.Delete( outputPath + "FRPropType.cs" );
			}
			
			File.WriteAllText( outputPath + "FRPropType.cs", template );
		}

		
		
	}
}