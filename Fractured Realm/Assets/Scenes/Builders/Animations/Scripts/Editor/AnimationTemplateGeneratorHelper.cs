using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
			
			
			animations.Add( animation );
		}
		
		int submachineCount = parent.stateMachineCount;
		
		for( int i = 0; i < submachineCount; ++i )
		{
			RetrieveAnimationsFromController( layer, parent.GetStateMachine(i), animations );
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
		
		List<string> operations = new List<string>();
		operations.Add ("Add");
		operations.Add ("Subtract");
		operations.Add ("Multiply");
		operations.Add ("Divide");
		operations.Add ("Simplify");
		operations.Add ("Double");

		// stores FROM -> TO of the animation stages
		Dictionary<string,string> stages = new Dictionary<string, string>();
		stages.Add("Starter", "Receiver");
		stages.Add("Ascend", "Descend");
		stages.Add("Attacks", "Hits");

		List<string> processedOperationStages = new List<string>(); // keep track of generated files

		List<FRAnimationData> animations = new List<FRAnimationData>();
		RetrieveAnimationsFromController(0, ac.GetLayer(0).stateMachine, animations );

		
		foreach( FRAnimationData animation in animations )
		{
			// couple of different options, figure out which one:
			// -1. animation is has various operation stages : ex. Add.Starter/forcePull and Add.Receiver/magnetAttractedFront
			// -2. animation is for an Operation, but doesn't use stages: ex. Simplify.backFlip, Multiply.headbang
			// -3. animation is not for an Operation: no OperationVisualizer needed

			// see if there's an operation in the parent name of the animation
			
			string[] splits = animation.parentName.Split('.');
			string parentOperation = splits[0];
			string parentStage = "";
			if( splits.Length > 1 )
			{
				parentStage = splits[1];
			}

			// case 3: animation is not for an operation
			if( !operations.Contains( parentOperation ) )
				continue; 

			string template = "";
			string fileName = "";

			// case 1: operation has various stages: generate base class for every stage
			if( !processedOperationStages.Contains( animation.parentName ) && !string.IsNullOrEmpty(parentStage) )
			{
				Debug.LogWarning("GenerateOperationVisualizerClasses : operationStage " + animation.parentName + " didn't have a base class yet. Generate it!");

				
				template = File.ReadAllText( basePath + "OperationVisualizerStageTemplate.cs" );
				template = template.Replace( "/*", "" );
				template = template.Replace( "*/", "" );
				
				template = template.Replace("OPERATION", parentOperation );
				template = template.Replace("STAGE", "_" + parentStage ); // if not chained (case 2: stage will just be empty)
				
				fileName = "OperationVisualizer" + parentOperation + "_" + parentStage + ".cs";
				
				if( File.Exists( outputPath + fileName ) )
				{
					File.Delete( outputPath + fileName );
				}
				
				File.WriteAllText( outputPath + fileName, template );


				// make sure base class is not generated again (multiple animations can share the same parent)
				processedOperationStages.Add( animation.parentName );
			}



			// generate class for specific animation
			template = File.ReadAllText( basePath + "OperationVisualizerAnimationTemplate.cs" );
			template = template.Replace( "/*", "" );
			template = template.Replace( "*/", "" );
			
			string stageName = ( !string.IsNullOrEmpty(parentStage) ) ? "_" + parentStage : "";

			template = template.Replace("OPERATION", parentOperation ); 
			template = template.Replace("STAGE", stageName );
			template = template.Replace("ANIMATION", "_" + animation.name );

			fileName = animation.VisualizerClassName() + ".cs";//"OperationVisualizer" + parentOperation + stageName + "_" + animation.name + ".cs";

			if( File.Exists( outputPath + fileName ) )
			{
				File.Delete( outputPath + fileName );
			}
			
			File.WriteAllText( outputPath + fileName, template );
		}
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
		
		
		string enumString = "public enum FRAnimation\n{\n\tNONE = -1, ";
		string ctorString = "FRAnimationData animation = null;\n\n";
		
		foreach( FRAnimationData animation in animations )
		{
			enumString += "\n\t" + animation.name + " = " + animation.hash + ",";
			
			ctorString += "\t\tanimation = new FRAnimationData();\n";
			ctorString += "\t\tanimation.name = \"" + animation.name + "\";\n";
			ctorString += "\t\tanimation.hash = " + animation.hash + ";\n";
			ctorString += "\t\tanimation.parentName = \"" + animation.parentName + "\";\n";
			ctorString += "\t\tanimation.layer = " + animation.layer + ";\n";

			string visualizer = animation.VisualizerClassName();
			if( !string.IsNullOrEmpty(visualizer) )
				ctorString += "\t\tanimation.visualizer = delegate(){ return new " + visualizer + "(); };\n";
			else
				ctorString += "\t\tanimation.visualizer = null;\n";
				
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
