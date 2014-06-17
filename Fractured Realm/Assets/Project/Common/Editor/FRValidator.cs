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

	protected bool useDefaultFractions = true;

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

		/*
		if( GUILayout.Button("Spawn just the numbers") )
		{
			Fraction[] fractions = new Fraction[2];
			fractions[0] = new Fraction( fr1_numerator, fr1_denominator );
			fractions[1] = new Fraction( fr2_numerator, fr2_denominator );
			
			WorldFactory.use.debug_initialFractions = fractions;
			EditorUtility.SetDirty(WorldFactory.use);

			WorldFactory.use.CreateFractions(fractions);
			FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
			HUDManager.use.SetMode( FR.Target.BOTH );
		}
		*/
		
		GUILayout.Space(10);

		FR.WorldType[] worldTypes = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
		foreach( FR.WorldType worldType in worldTypes )
		{
			if( worldType == FR.WorldType.NONE )
				continue;

			if( GUILayout.Button("Create " + worldType ) )
			{
				
				Fraction[] fractions = new Fraction[2];
				fractions[0] = new Fraction( fr1_numerator, fr1_denominator );
				fractions[1] = new Fraction( fr2_numerator, fr2_denominator );

				WorldFactory.use.defaultWorldType = worldType;
				WorldFactory.use.debug_initialFractions = fractions;
				EditorUtility.SetDirty(WorldFactory.use);
				
				
				WorldFactory.use.CreateDebugWorld( worldType, fractions );

				/*
				WorldFactory.use.CreateWorld(worldType, fractions, FR.Target.BOTH, true);
				FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
				HUDManager.use.SetMode( FR.Target.BOTH );
				*/
			}

		}

		GUILayout.Space(10);

		if( GUILayout.Button("Create Desert (Numerator Only test)") )
		{
			Fraction[] fractions = new Fraction[2];
			fractions[0] = new Fraction( fr1_numerator, 0 );
			fractions[1] = new Fraction( fr2_numerator, 0 );
			
			WorldFactory.use.debug_initialFractions = fractions;
			EditorUtility.SetDirty(WorldFactory.use);
			
			World world = WorldFactory.use.CreateWorld(FR.WorldType.DESERT, FR.Target.NUMERATOR);
			RendererFactory.use.CreateRenderers(world, fractions, true );
			FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
			HUDManager.use.SetMode( FR.Target.NUMERATOR );
			FRCamera.use.MoveToInteractionGroup( 1, 1, false );
		}
		
		if( GUILayout.Button("Create Desert (Denominator Only test)") )
		{
			Fraction[] fractions = new Fraction[2];
			fractions[0] = new Fraction( fr1_numerator, 0 );
			fractions[1] = new Fraction( fr2_numerator, 0 );
			
			WorldFactory.use.debug_initialFractions = fractions;
			EditorUtility.SetDirty(WorldFactory.use);

			World world = WorldFactory.use.CreateWorld(FR.WorldType.DESERT, FR.Target.DENOMINATOR);
			RendererFactory.use.CreateRenderers(world, fractions, true );
			FRCamera.use.mode = FR.Target.NONE; // force mode reset on cam
			HUDManager.use.SetMode( FR.Target.DENOMINATOR );
			FRCamera.use.MoveToInteractionGroup( 1, 1, false ); 
		}
		
		GUILayout.Label("-------------------");
		GUILayout.Label("-------------------");
		GUILayout.Label("-------------------");

		if( Application.isPlaying )
		{
			GUILayout.Label("OperationTester");
			GUILayout.Label("-------------------");
			
			useDefaultFractions = GUILayout.Toggle(useDefaultFractions, "Use default fractions");

			if( useDefaultFractions )
				FROperationTester.use.DrawGUI(null, null);
			else
				FROperationTester.use.DrawGUI(new Fraction( fr1_numerator, fr1_denominator ), new Fraction( fr2_numerator, fr2_denominator ));

		}
		else
		{
			GUILayout.Label("OperationTester : only works in Play mode");
			GUILayout.Label("-------------------");
		}
	}
	
	public void ValidateCharacterFactory()
	{
		Debug.Log("START ValidateCharacterFactory");
		
		RendererFactory cf = RendererFactory.use; 
		if( cf == null )
		{
			Debug.LogError("FRValidator:ValidateCharacterFactory : cf is null!");
			return;
		}
		
		Logus.Assert( cf.Numerators.Length == 6 );
		Logus.Assert( cf.Denominators.Length == 6 );
		
		int i = 1;
		foreach( CharacterRenderer go in cf.Numerators )
		{
			if( go == null )
				Debug.LogError("FRValidator:ValidateCharacterFactory : Numerator " + i + " is null!" );
			
			i++;
		}
		
		i = 1;
		foreach( CharacterRenderer go in cf.Denominators )
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

		FR.WorldType[] types = (FR.WorldType[]) Enum.GetValues(typeof(FR.WorldType));
		foreach( FR.WorldType type in types )
		{
			if( type == FR.WorldType.NONE )
				continue;
			
			
			Debug.Log("CHECK " + type);

			List<WorldPart> templates = WorldFactory.use.GetWorldParts( type );

			if( templates == null || templates.Count == 0 )
			{
				Debug.LogError("FRValidator:ValidateWorldFactory : factory doesn't contain WorldParts for type " + type);
			}
			else
			{
				foreach( WorldPart part in templates )
				{
					Debug.Log ("Validating WorldPart template : " + part.name);

					Logus.Assert( part.DenominatorSpecificObjects() != null );
					Logus.Assert( part.NumeratorSpecificObjects() != null );
					Logus.Assert( part.transform.FindChild("Geometry") != null );
					Logus.Assert( part.transform.FindChild("InteractionGroups") != null );


					// GetComponentsInChildren<> doesn't work in editor mode when checkin a prefab...

					/*
					bool directionalFound = false;
					Light[] lights = part.transform.GetComponentsInChildren<Light>();
					foreach( Light light in lights )
					{
						if( light.type == LightType.Directional )
						{
							directionalFound = true;
							break;
						}
					}

					Logus.Assert( directionalFound );


					if( Logus.Assert( part.InteractionGroups != null && part.InteractionGroups.Length > 0 ) )
					{
						InteractionGroup[] groups = part.InteractionGroups; 
						foreach( InteractionGroup group in groups )
						{
							Logus.Assert( group.Spawn1 != null );
							Logus.Assert( group.Spawn2 != null );
							Logus.Assert( group.PortalEntry != null );
							Logus.Assert( group.PortalExit != null );
							Logus.Assert( group.Camera != null );
						}
					}
					*/
				}
			}




			/*
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
					if( Logus.Assert( numerator.InteractionGroups != null && numerator.InteractionGroups.Length > 0 ) )
					{
						Logus.Assert( numerator.InteractionGroups[0].Spawn1 != null );
						Logus.Assert( numerator.InteractionGroups[0].Spawn2 != null );
					}
				}
				
				if( Logus.Assert( denominator != null) )
				{
					if( Logus.Assert( denominator.InteractionGroups != null && denominator.InteractionGroups.Length > 0 ) )
					{
						Logus.Assert( denominator.InteractionGroups[0].Spawn1 != null );
						Logus.Assert( denominator.InteractionGroups[0].Spawn2 != null );
					}
				}
			}
			*/
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








