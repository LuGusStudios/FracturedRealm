using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AnimationTemplateGeneratorHelper 
{
	protected static void RetrieveAnimationsFromController(int layer, UnityEditorInternal.StateMachine parent, List<FRAnimationData> animations)
	{
		int stateCount = parent.stateCount;
		for( int i = 0; i < stateCount; ++i )
		{
			UnityEditorInternal.State state = parent.GetState(i);
			
			// TODO: FIXME: check if this animation "name" (name + parent?) doesn't exist yet... important!
			// TODO: FIXME: must make sure parent names are also unique across layers... 
			FRAnimationData animation = new FRAnimationData();
			animation.name = state.name;
			animation.parentName = parent.name;
			animation.hash = Animator.StringToHash( animation.parentName + "." + animation.name);
			
			animation.movieClipName = "NONE YET";
			animation.layer = layer;


			// parent name can have various forms:
			// - Operation / identifier
			// - Operation.Stage
			string[] splits = animation.parentName.Split('.'); // if no ., splits[0] will just contain the full string
			animation.originalOperationName = splits[0];
			animation.operation = StringToOperationType( animation.originalOperationName );

			if( splits.Length > 1 )
			{
				// Operation.Stage
				animation.originalStageName = splits[1];
				animation.stage = StringToAnimationStage( animation.originalStageName );
			}


			
			
			animations.Add( animation );
		}
		
		int submachineCount = parent.stateMachineCount;
		
		for( int i = 0; i < submachineCount; ++i )
		{
			RetrieveAnimationsFromController( layer, parent.GetStateMachine(i), animations );
		}
	}

	protected static FR.OperationType StringToOperationType(string operationName)
	{
		operationName = operationName.ToUpper();

		try
		{
			return (FR.OperationType) Enum.Parse( typeof(FR.OperationType), operationName );
		}
		catch( ArgumentException e )
		{
			return FR.OperationType.NONE;
		}
	}

	protected static FR.AnimationStage StringToAnimationStage(string animationStage)
	{
		// stores FROM -> TO of the animation stages
		Dictionary<string,string> stages = new Dictionary<string, string>();
		stages.Add("Starter", "Receiver");
		stages.Add("Descend", "Ascend");
		stages.Add("Attacks", "Hits");

		if( stages.ContainsKey(animationStage) )
		{
			return FR.AnimationStage.Stage1;
		}
		else if( stages.ContainsValue(animationStage) )
		{
			return FR.AnimationStage.Stage2;
		}
		else
		{
			return FR.AnimationStage.NONE;
		}
	}

	public static void GenerateOperationVisualizerClasses(UnityEditorInternal.AnimatorController ac)
	{
		string basePath = Application.dataPath + "/" + "Project/Common/Scripts/Characters/Animations/";
		string outputPath = basePath + "OperationVisualizers/";
		
		
		if( !Directory.Exists( basePath ) )
		{
			Debug.LogError("GenerateOperationVisualizerClasses : basePath does not exist! " + basePath);
			return;
		}

		if( !Directory.Exists( outputPath ) )
		{
			Directory.CreateDirectory( outputPath );
		}


		List<string> processedOperationStages = new List<string>(); // keep track of generated files

		List<FRAnimationData> animations = new List<FRAnimationData>();
		RetrieveAnimationsFromController(0, ac.GetLayer(0).stateMachine, animations );

		Dictionary<FR.OperationType, List<string>> starterAnimations = new Dictionary<FR.OperationType, List<string>>();
		Dictionary<FR.OperationType, List<string>> stage2Animations = new Dictionary<FR.OperationType, List<string>>(); 


		string template = "";
		string fileName = "";

		// first pass: gather info about various stages and generate stage-base classes where needed
		foreach( FRAnimationData animation in animations )
		{
			// couple of different options, figure out which one:
			// -1. animation is has various operation stages : ex. Add.Starter/forcePull and Add.Receiver/magnetAttractedFront
			// -2. animation is for an Operation, but doesn't use stages: ex. Simplify.backFlip, Multiply.headbang
			// -3. animation is not for an Operation: no OperationVisualizer needed


			// case 3: animation is not for an operation
			if( animation.operation == FR.OperationType.NONE )
				continue; 

			// case 1: operation has various stages: generate base class for every stage if it doesn't exist yet
			// case 2 doesn't need extra base class: they just inherit from the original OperationVisualizer for their specific Operation
			if( !processedOperationStages.Contains( animation.parentName ) && (animation.stage != FR.AnimationStage.NONE) )
			{
				// don't overwrite the file if it already exists!
				// users might have already added code to the file
				
				
				fileName = "OperationVisualizer" + animation.originalOperationName + "_" + animation.originalStageName + ".cs";

				if( File.Exists( outputPath + fileName ) )
				{
					fileName = "OperationVisualizer" + animation.originalOperationName + "_" + animation.originalStageName + ".cs";
					Debug.Log ("GenerateOperationVisualizerClasses : " + fileName + " existed. Not overwritten.");
				}
				else
				{
					Debug.LogWarning("GenerateOperationVisualizerClasses : operationStage " + animation.parentName + " didn't have a base class yet. Generate it!");

					
					template = File.ReadAllText( basePath + "OperationVisualizerStageTemplate.cs" );
					template = template.Replace( "/*", "" );
					template = template.Replace( "*/", "" );
					
					template = template.Replace("OPERATION", animation.originalOperationName );
					template = template.Replace("STAGE", "_" + animation.originalStageName ); 

					/*
					if( File.Exists( outputPath + fileName ) )
					{
						File.Delete( outputPath + fileName );
					}
					*/
					
					File.WriteAllText( outputPath + fileName, template );
				}
				
				// make sure base class is not generated again (multiple animations can share the same parent)
				processedOperationStages.Add ( animation.parentName );
			}

			
			// if it's a starter animation, we need to keep it for later processing
			// if it doesn't have a specific stage, it's also a "starter" animation
			if( animation.stage == FR.AnimationStage.Stage1 || animation.stage == FR.AnimationStage.NONE )
			{
				if( !starterAnimations.ContainsKey(animation.operation) )
				{
					//Debug.LogWarning("new key found for stageAnimations : " + operationType);
					starterAnimations[animation.operation] = new List<string>();
				}
				
				//Debug.LogError("Adding " + animation.name + " to " + operationType);
				starterAnimations[animation.operation].Add( animation.name );
			}
			
			// if it's a receiver animation, we need to keep it for later processing
			if( animation.stage == FR.AnimationStage.Stage2 )
			{
				if( !stage2Animations.ContainsKey(animation.operation) )
				{
					stage2Animations[animation.operation] = new List<string>();
				}

				stage2Animations[animation.operation].Add( animation.name );
			}
		}


		// second pass : generate actual animation files per animation
		// we have to do this in 2 passes, because we need to know each possible STAGE2 animation for the STAGE1's because they need to be included in their files.
		foreach( FRAnimationData animation in animations )
		{
			if( animation.operation == FR.OperationType.NONE )
				continue; 

			string nextString = "";
			if( animation.stage == FR.AnimationStage.Stage1 )
			{
				nextString = "\n";

				if( stage2Animations.ContainsKey( animation.operation ) ) 
				{
					foreach( string stage2 in stage2Animations[animation.operation] )
					{
						nextString += "\t\t_nextAnimations.Add(FR.Animation." + stage2 + ");\n";
					}
				}
				else
					Debug.LogError("GenerateOperationVisualizerClasses : no stage2 animations found for stage1 " + animation.operation + " // " + animation.parentName + "." + animation.name);
			}


			bool generateFile = true;
			


			fileName = animation.VisualizerClassName() + ".cs";//"OperationVisualizer" + parentOperation + stageName + "_" + animation.name + ".cs";

			if( File.Exists(outputPath + fileName) )
			{
				// the animation-specific file already exists
				// if someone has started adjusting it already, we shouldn't overwrite it!

				string fileContents = File.ReadAllText( outputPath + fileName );
				if( fileContents.Contains("FR.VisualizerImplementationStatus.NONE") )
				{
					// implementationstate is NONE, so we can overwrite
					File.Delete( outputPath + fileName );
				}
				else
				{
					generateFile = false;

					if( animation.operation != FR.OperationType.NONE )
						Debug.Log ("GenerateOperationVisualizerClasses : " + animation.VisualizerClassName() + " already implemented. Not overwritten.");
				}
			}


			if( generateFile )
			{
				// generate class for specific animation
				template = File.ReadAllText( basePath + "OperationVisualizerAnimationTemplate.cs" );
				template = template.Replace( "/*", "" );
				template = template.Replace( "*/", "" );
				
				// if it's not a "Staged" animation, we need to change the class / file name to leave the stage name out completely
				string stageName = ( !string.IsNullOrEmpty(animation.originalStageName) ) ? "_" + animation.originalStageName : "";
				
				template = template.Replace("NEXTANIMATION", nextString );

				template = template.Replace("OPERATION", animation.originalOperationName ); 
				template = template.Replace("STAGE", stageName );
				template = template.Replace("ANIMATION", "_" + animation.name );
				template = template.Replace("TYPE", "" + animation.name );

				
				File.WriteAllText( outputPath + fileName, template );
			}
		}





		// we need a record somewhere of the animations that belong to a certain Operation ("starters" if they are staged)
		// each STARTER animation keeps track of possible RECEIVERS, so something individual for those isn't needed
		string ctorString = "\n";
		foreach( FR.OperationType operation in starterAnimations.Keys )
		{
			ctorString += "\t\t_operationAnimations[ FR.OperationType." + operation + " ] = new List<FR.Animation>();\n";

			foreach( string animationName in starterAnimations[operation] )
			{
				ctorString += "\t\t_operationAnimations[ FR.OperationType." + operation + " ].Add( FR.Animation."+ animationName +" );\n";
			}

			ctorString += "\n";
		}

		template = File.ReadAllText( basePath + "FRAnimationsStagesTemplate.cs" );
		template = template.Replace( "/*", "" );
		template = template.Replace( "*/", "" );

		template = template.Replace("INIT", ctorString );
		
		if( File.Exists( basePath + "FRAnimationsStages.cs" ) )
		{
			File.Delete( basePath + "FRAnimationsStages.cs" );
		}
		
		File.WriteAllText( basePath + "FRAnimationsStages.cs", template );
	}

	public static void GenerateFRAnimationsClass(UnityEditorInternal.AnimatorController ac)
	{
		// TODO: FIXME: loop over all layers to add all animations
		List<FRAnimationData> animations = new List<FRAnimationData>();
		RetrieveAnimationsFromController(0, ac.GetLayer(0).stateMachine, animations );
		
		/*
		 * 
		auto-generate ENUM with different anim states as fields
		-> this works: animator.CrossFade( "/Movements.running", 0.15f );
		-> this doesn't: animator.CrossFade( "Base Layer.running", 0.15f );
		-> neither does this: animator.CrossFade( "Base Layer./Movements.running", 0.15f );

		-> if state is named "Movements.test", neither "Movements.test.xxx" or "test.xxx" seems to work
		  -> So no "." inside substate names, yippy

        also need to make sure substate names are unique across layers this way, and "nested" substates... darned
		 */ 
		
		
		string enumString = "public enum Animation\n{\n\tNONE = -1, "; 
		string ctorString = "FRAnimationData animation = null;\n\n";
		
		foreach( FRAnimationData animation in animations )
		{
			enumString += "\n\t" + animation.name + " = " + animation.hash + ",";


			ctorString += "\t\tanimation = new FRAnimationData();\n";
			ctorString += "\t\tanimation.name = \"" + animation.name + "\";\n";
			ctorString += "\t\tanimation.type = FR.Animation." + animation.name + ";\n";
			ctorString += "\t\tanimation.hash = " + animation.hash + ";\n";
			ctorString += "\t\tanimation.parentName = \"" + animation.parentName + "\";\n";
			ctorString += "\t\tanimation.layer = " + animation.layer + ";\n";

			
			ctorString += "\t\tanimation.stage = FR.AnimationStage." + animation.stage + ";\n";
			ctorString += "\t\tanimation.operation = FR.OperationType." + animation.operation + ";\n";
			ctorString += "\t\tanimation.originalOperationName = \"" + animation.originalOperationName + "\";\n";
			ctorString += "\t\tanimation.originalStageName = \"" + animation.originalStageName + "\";\n";

			string visualizer = animation.VisualizerClassName();
			if( !string.IsNullOrEmpty(visualizer) )
				ctorString += "\t\tanimation.visualizerCreate = delegate(){ return new " + visualizer + "(); };\n";
			else
				ctorString += "\t\tanimation.visualizerCreate = null;\n";
				
			ctorString += "\t\tanimations.Add(" + animation.hash+ ", animation);\n\n";
		}
		
		
		enumString += "\n};";
		
		string outputPath = Application.dataPath + "/" + "Project/Common/Scripts/Characters/Animations/";
		
		string template = File.ReadAllText( outputPath + "FRAnimationsTemplate.cs" );
		template = template.Replace( "/*", "" );
		template = template.Replace( "*/", "" );
		
		template = template.Replace("ENUM", enumString );
		template = template.Replace("INIT", ctorString );
		
		if( File.Exists( outputPath + "FRAnimations.cs" ) )
		{
			File.Delete( outputPath + "FRAnimations.cs" );
		}
		
		File.WriteAllText( outputPath + "FRAnimations.cs", template );
		
		// ex. call GetAssetsInFolderRecursive("Project/CharactersNew/Animations", "", output);
	}
}
