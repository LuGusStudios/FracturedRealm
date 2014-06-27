using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(ExerciseManager))]
public class ExerciseManagerEditor : Editor 
{
	//protected bool showDefault = false;
	
	public override void OnInspectorGUI()
	{
		ExerciseManager subject = (ExerciseManager) target;
		
		EditorGUIUtility.LookLikeInspector();
		
		//showDefault = EditorGUILayout.Foldout(showDefault, "Show original");
		//if( showDefault )
		//{
			DrawDefaultInspector();
		//}
		
		EditorGUILayout.LabelField("------------");
		
		//DrawDefaultInspector();
		

		if( GUILayout.Button("Fill AllExerciseGroups.txt") )
		{
			FillAllExerciseGroupsFile(subject);
		}

		if( Application.isPlaying )
		{
			if( GUILayout.Button("Write HTML output") )
			{
				WriteHTMLOutput();
			}
		}
		else
		{
			GUILayout.Button("enter PLAYMODE to write html output");
		}
	}

	public void FillAllExerciseGroupsFile(ExerciseManager subject)
	{
		string basePath = Application.dataPath + "/Resources/Shared/Text/";
		//string outputPath = Application.dataPath + "/Project/Common/Scripts/Characters/Props/";
		
		// read all prefabs
		if( !Directory.Exists( basePath ) )
		{
			Debug.LogError("ExerciseManager:FillAllExerciseGroupsFile : basePath doesn't exist! " + basePath);
			return;
		}
		
		DirectoryInfo directoryFolder = new DirectoryInfo( basePath );

		string output = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n<ExerciseGroups>\n";

		FileInfo[] exerciseGroupFiles = directoryFolder.GetFiles();
		foreach( FileInfo exerciseGroupFile in exerciseGroupFiles )
		{
			if( exerciseGroupFile.Name.Contains(".meta") )
				continue;

			if( !exerciseGroupFile.Name.Contains("ExerciseGroup_") )
				continue;

			output += "\t<ExerciseGroup>" + exerciseGroupFile.Name.Replace( "" + exerciseGroupFile.Extension, "" ) + "</ExerciseGroup>\n";
		}

		output += "</ExerciseGroups>";

		
		if( File.Exists( basePath + "AllExerciseGroups.txt" ) )
		{
			File.Delete( basePath + "AllExerciseGroups.txt" );
		}
		
		File.WriteAllText( basePath + "AllExerciseGroups.txt", output );
	}

	public void WriteHTMLOutput()
	{
		//string codePath = Application.dataPath + "/" + "Project/Common/Scripts/Characters/Animations/OperationVisualizers/";
		string outputPath = Application.dataPath + "/" + "Scenes/Builders/GameplaySetup/Output/";

		
		if( !Directory.Exists( outputPath ) )
		{
			Debug.LogError("WriteHTMLOutput : outputPath does not exist! " + outputPath);
			return;
		}

		List<IOperation> operations = new List<IOperation>();
		operations.Add( new OperationAdd() );
		operations.Add( new OperationSubtract() );
		operations.Add( new OperationSimplify() );
		operations.Add( new OperationDouble() );
		operations.Add( new OperationMultiply() );
		operations.Add( new OperationDivide() );


		string output = "";
		List<string> groupNames = ExerciseLoader.LoadAllExerciseGroupNames();

		foreach( string groupName in groupNames )
		{
			ExerciseGroup group = ExerciseLoader.LoadExerciseGroup(groupName);

			if( group == null )
			{
				continue;
			}

			output += "<div class=\"ExerciseGroup\">";
			output += "<h1>" + group.name + "</h1>";
			output += "<p>" + group.description + "</p>";

			foreach( Exercise exercise in group.exercises )
			{
				if( exercise == null )
				{
					output += "<div class=\"Exercise ERROR\">ERROR</div>";
					continue;
				}

				output += "<div class=\"Exercise "+ exercise.worldType +"\">";
				output += "<p>" + exercise.difficulty + " / " + exercise.composition + " / " + exercise.worldType + "</p>";

				foreach( ExercisePart part in exercise.parts )
				{
					if( exercise == null )
					{
						output += "div class=\"ExercisePart ERROR\">ERROR</div>";
						continue;
					}

					output += "<div class=\"ExercisePart\">";

					foreach( FR.OperationType operationType in part.operations )
					{
						IOperation operation = null;
						foreach( IOperation found in operations )
						{
							if( found.type == operationType )
								operation = found;
						}
						
						if( operation == null )
						{
							output += "ERROR : type not found! " + operationType;
							continue;
						}



						Fraction fr1 = part.First;
						OperationState operationInfo = new OperationState(operationType, fr1, null); 


						bool ok = operation.AcceptState( operationInfo );
						if( !ok )
						{
							output += "ERROR : just 1 : " + operation.lastMessage;
							continue;
						}

						operationInfo.StopFraction = part.Last;

						ok = operation.AcceptState( operationInfo );
						if( !ok )
						{
							output += "ERROR : after 2 : " + operation.lastMessage;
							continue;
						}
						
						OperationState result = operation.Process( operationInfo );

						output += "<span class=\"frac\"><sup>"+ part.First.Numerator.Value +"</sup><span>/</span><sub>"+ part.First.Denominator.Value +"</sub></span>";
						output += "<span class=\"oper\"><sup>&nbsp;&nbsp;"+ IOperation.OperationTypeToString(operationType) +"&nbsp;&nbsp;</sup></span>";
						output += "<span class=\"frac\"><sup>"+ part.Last.Numerator.Value +"</sup><span>/</span><sub>"+ part.Last.Denominator.Value +"</sub></span>";
						output += "<span class=\"oper\"><sup>&nbsp;&nbsp;=&nbsp;&nbsp;</sup></span>";
						output += "<span class=\"frac\"><sup>"+ result.StartFraction.Numerator.Value +"</sup><span>/</span><sub>"+ result.StartFraction.Denominator.Value +"</sub></span><br/>";
					}

					output += "<p>";
					if( part.availableOperations != null )
					{
						foreach( KeyValuePair<FR.OperationType, int> pair in part.availableOperations )
						{
							for( int i = 0; i < pair.Value; ++i )
							{
								output += "" + IOperation.OperationTypeToString(pair.Key) + " ";
							}
						}
					}
					else
					{
						output += "ALL";
					}
					output += "</p>";


					output += "</div>";
				}


				
				output += "<div class=\"clearer\"></div></div>";
			}


			
			output += "</div>";
		}


		/*
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
		*/




		
		string template = File.ReadAllText( outputPath + "ExercisesIndexTemplate.html" );
		template = template.Replace( "/*", "" );
		template = template.Replace( "*/", "" );
		
		template = template.Replace("{CONTENT}", output );
		
		if( File.Exists( outputPath + "index.html" ) )
		{
			File.Delete( outputPath + "index.html" );
		}
		
		File.WriteAllText( outputPath + "index.html", template );
	}
}

