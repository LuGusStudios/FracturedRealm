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

		// re-use SIMPLIFY animations for the DOUBLE animations as well
		foreach( string animationName in starterAnimations[FR.OperationType.SIMPLIFY] )
		{
			if( !starterAnimations.ContainsKey(FR.OperationType.DOUBLE) )
			{
				starterAnimations[FR.OperationType.DOUBLE] = new List<string>();
			}
			
			starterAnimations[FR.OperationType.DOUBLE].Add( animationName );
		}

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

	public static void GenerateHTMLAnimationBreakdown( UnityEditorInternal.AnimatorController ac)
	{
		string codePath = Application.dataPath + "/" + "Project/Common/Scripts/Characters/Animations/OperationVisualizers/";
		string outputPath = Application.dataPath + "/" + "Scenes/Builders/Animations/Output/";
		
		
		if( !Directory.Exists( codePath ) )
		{
			Debug.LogError("GenerateHTMLAnimationBreakdown : codePath does not exist! " + codePath);
			return;
		}
		
		if( !Directory.Exists( outputPath ) )
		{
			Debug.LogError("GenerateHTMLAnimationBreakdown : outputPath does not exist! " + outputPath);
			return;
		}

		List<string> processedOperationStages = new List<string>(); // keep track of generated files
		
		List<FRAnimationData> animations = new List<FRAnimationData>();
		RetrieveAnimationsFromController(0, ac.GetLayer(0).stateMachine, animations );
		
		Dictionary<FR.OperationType, List<FRAnimationData>> starterAnimations = new Dictionary<FR.OperationType, List<FRAnimationData>>();
		Dictionary<FR.OperationType, List<FRAnimationData>> stage2Animations = new Dictionary<FR.OperationType, List<FRAnimationData>>(); 

		
		// first pass: gather info about various stages and generate stage-base classes where needed
		foreach( FRAnimationData animation in animations )
		{
			if( animation.operation == FR.OperationType.NONE )
				continue; 

			if( !starterAnimations.ContainsKey(animation.operation) )
				starterAnimations[ animation.operation ] = new List<FRAnimationData>();
			if( !stage2Animations.ContainsKey(animation.operation) )
				stage2Animations[ animation.operation ] = new List<FRAnimationData>();


			if( animation.stage == FR.AnimationStage.Stage1 || animation.stage == FR.AnimationStage.NONE )
				starterAnimations[ animation.operation ].Add( animation );
			if( animation.stage == FR.AnimationStage.Stage2 )
				stage2Animations[ animation.operation ].Add( animation );
		}

		string itemTemplate = "";
		itemTemplate = File.ReadAllText( outputPath + "animationItemTemplate.html" );

		FRAnimations.use.FillDictionary();

		string output = "";
		foreach( FR.OperationType operation in starterAnimations.Keys )
		{
			output += "<div class=\"operationBox\">";
			output += "<h1>" + operation + " (" + (starterAnimations[operation].Count) + ")</h1>";

			Comparison<FRAnimationData> comparison = delegate( FRAnimationData g1, FRAnimationData g2 )
			{ 		
				
				g1.type = (FR.Animation) Enum.Parse( typeof(FR.Animation), g1.name );
				g1 = FRAnimations.use.animations[ (int) g1.type ];
				
				g2.type = (FR.Animation) Enum.Parse( typeof(FR.Animation), g2.name );
				g2 = FRAnimations.use.animations[ (int) g2.type ];
				
				// < 0 : g1 precedes g2
				// > 0 : g1 follows g2

				// want to reverse sort by enum int value here : first finished, then testing, in progress, none

				if( ( (int) g1.visualizer.GetImplementationStatus()) > ( (int) g2.visualizer.GetImplementationStatus()) )
					return -1;
				else
					return 1;
			};

			starterAnimations[ operation ].Sort( comparison );
			stage2Animations[ operation ].Sort( comparison );

			foreach( FRAnimationData animation in starterAnimations[ operation ] )
			{
				output += FillAnimationHTMLTemplate( animation, itemTemplate );
			}

			output += "<div class=\"clearer\"></div>";
			
			foreach( FRAnimationData animation in stage2Animations[ operation ] )
			{
				output += FillAnimationHTMLTemplate( animation, itemTemplate );
			}
			
			output += "<div class=\"clearer\"></div>";
			output += "</div>";
		}


		
		string template = File.ReadAllText( outputPath + "indexTemplate.html" );
		template = template.Replace( "/*", "" );
		template = template.Replace( "*/", "" );
		
		template = template.Replace("{CONTENT}", output );
		
		if( File.Exists( outputPath + "index.html" ) )
		{
			File.Delete( outputPath + "index.html" );
		}
		
		File.WriteAllText( outputPath + "index.html", template );
	}

	protected static string FillAnimationHTMLTemplate( FRAnimationData animation, string template )
	{
		List<string> errors = new List<string>();

		string status = "error";

		// the animation.type hasn't been assigned as of yet! construct it, and use FRAnimations to get full FRAnimationData
		animation.type = (FR.Animation) Enum.Parse( typeof(FR.Animation), animation.name );

		//Debug.Log ("ANIMATION " + animation.type);

		animation = FRAnimations.use.animations[ (int) animation.type ]; // same data, but with all variables filled in!

		IOperationVisualizer visualizer = animation.visualizer;
		if( visualizer != null )
		{
			status = "Status" + visualizer.GetImplementationStatus();
		}
		else
		{
			Debug.LogError("FillAnimationHTMLTemplate: " + animation.name + " has no visualizer!!!");
			errors.Add("No visualizer known!");
		}

		string description = "";

		string stageName = "";
		if( animation.stage == FR.AnimationStage.NONE )
			stageName = "Stage0";
		else
			stageName = "" + animation.stage;

		string nextAnimations = "";

		List<FR.Animation> nexts = visualizer.NextAnimations;
		foreach( FR.Animation next in nexts )
		{
			nextAnimations += "- " + next + "<br/>";
		}

		if( nextAnimations != "" )
		{
			nextAnimations = "NextAnimations:<br/>" + nextAnimations;
		}

		if( animation.stage == FR.AnimationStage.Stage1 && nextAnimations == "" )
		{
			// no follow-up animations for stage 1 : that's a paddlin'!
			errors.Add("No NextAnimations() known for this stage1 animation! Expected at least 1");
		}

		if( visualizer.GetImplementationStatus() != FR.VisualizerImplementationStatus.NONE )
		{
			float duration = visualizer.ApproximateDuration();
			if( duration < 0.0f )
			{
				// not really necessary at this time, not used anywhere...
				//errors.Add("ApproximateDuration is negative!");
			}
			else
			{
				description += "(" + duration + "s) ";
			}

			if( visualizer is OperationVisualizerDivide )
			{
				float transitionTime = ( (OperationVisualizerDivide) visualizer ).TimeToTransition();
				if( transitionTime < 0.0f )
				{
					errors.Add("TimeToTransition is negative! " + transitionTime);
				}
			}
		}



		List<string> todos = new List<string>();

		string filePath = Application.dataPath + "/" + "Project/Common/Scripts/Characters/Animations/OperationVisualizers/";
		filePath += animation.VisualizerClassName() + ".cs";

		if( !File.Exists(filePath) )
		{
			errors.Add("No .cs file exists for this animation! Expected " + animation.VisualizerClassName() );
		}
		else
		{
			string fileContents = File.ReadAllText( filePath );
			string[] lines = fileContents.Split('\n');

			foreach( string line in lines )
			{
				if( line.Contains("TODO") || line.Contains("Todo") || line.Contains("todo") ||
				   line.Contains("FIXME")|| line.Contains("Fixme") || line.Contains("fixme") )
				{
					todos.Add( line.Trim() );
				}
			}
		}



		description += "" + stageName;

		string output = template;
		output = output.Replace("{NAME}", animation.originalStageName + "_" + animation.name);
		output = output.Replace("{DESCRIPTION}", "" + description);

		string classnames = "animation " + stageName + " " + status; 

		string errorOut = "";
		if( errors.Count != 0 )
		{
			foreach( string error in errors ) 
			{
				errorOut += "<div class=\"errorEntry\">" + error + "</div><br/>";
			}

			classnames += " error"; 
		}

		string todoOut = "";
		foreach( string todo in todos )
		{
			todoOut += "<div class=\"todoEntry\">"+ todo +"</div>";
		}

		output = output.Replace("{CLASS}", classnames);
		output = output.Replace("{NEXT_ANIMATIONS}", nextAnimations);
		
		output = output.Replace("{ERRORS}", errorOut);
		output = output.Replace("{TODOS}", todoOut);

		return output;
	}
}
