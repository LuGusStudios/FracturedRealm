using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExerciseLoader 
{
	public static List<string> LoadAllExerciseGroupNames()
	{
		List<string> output = new List<string>();


		TextAsset file = LugusResources.use.Shared.GetTextAsset("AllExerciseGroups");

		if( file == LugusResources.use.errorTextAsset )
		{
			return output;
		}

		TinyXmlReader parser = new TinyXmlReader(file.text);
		
		while ( parser.Read() )
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
			    (parser.tagName == "ExerciseGroup"))
			{
				output.Add( parser.content );
				Debug.Log("ExerciseLoader:LoadAllGroupsNames : found group " + (output[output.Count - 1]));
			}
		}

		return output;
	}

	public static ExerciseGroup LoadExerciseGroup(string name)
	{
		ExerciseGroup output = null;

		TextAsset file = LugusResources.use.Shared.GetTextAsset( name );
		
		if( file == LugusResources.use.errorTextAsset )
		{
			Debug.LogError("ExerciseGroup " + name + " not found. ErrorTextAsset returned!");
			return output;
		}

		output = new ExerciseGroup();
		output.name = name;
		
		TinyXmlReader parser = new TinyXmlReader(file.text);
		
		while ( parser.Read() )
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
			    (parser.tagName == "Exercise"))
			{
				Exercise e = ParseExercise( parser );
				//Debug.Log("ExerciseLoader:LoadAllGroupsNames : found group " + (output[output.Count - 1]));

				//if( e != null )
				//{
					output.exercises.Add( e );
				//}
			}
			else if( (parser.tagType == TinyXmlReader.TagType.OPENING) &&
			        (parser.tagName == "Description"))
			{
				output.description = parser.content;
			}
			else if( (parser.tagType == TinyXmlReader.TagType.OPENING) &&
			        (parser.tagName == "PreScene"))
			{
				output.preSceneName = parser.content;
			}
			else if( (parser.tagType == TinyXmlReader.TagType.OPENING) &&
			        (parser.tagName == "PostScene"))
			{
				output.postSceneName = parser.content;
			}
		}


		return output;
	}

	protected static Exercise ParseExercise(TinyXmlReader parser)
	{
		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
		    (parser.tagName != "Exercise"))
		{
			Debug.Log("ExerciseLoader.ParseExercise(): unexpected tag type or tag name. " + parser.tagName + " // " + parser.tagType);
			return null;
		}
		
		Debug.Log ("ExerciseLoader:ParseExercise started");

		FR.WorldType previousWorldType = FR.WorldType.NONE;
		bool forceWorldTypeRandom = false;

		Exercise output = new Exercise();
		string temp = "";

		while (parser.Read("Exercise"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "Difficulty":
					{
						try
						{
							temp = parser.content.ToUpper();
							output.difficulty = (FR.ExerciseDifficulty) Enum.Parse(typeof(FR.ExerciseDifficulty), temp);
						}
						catch(Exception e)
						{
							Debug.LogError("ExerciseLoader:ParseExercise : difficulty could not be parsed! " + parser.content);
							output.difficulty = FR.ExerciseDifficulty.NONE;
						}
						//Debug.Log ("DIFFICULTY : " + output.difficulty);
						break;
					}
					case "Composition":
					{
						try
						{
							temp = parser.content.ToUpper();
							output.composition = (FR.Target) Enum.Parse(typeof(FR.Target), temp);
						}
						catch(Exception e)
						{
							Debug.LogError("ExerciseLoader:ParseExercise : composition could not be parsed! " + parser.content);
							output.composition = FR.Target.BOTH;
						}
						if( output.composition != FR.Target.BOTH &&
						    output.composition != FR.Target.NUMERATOR &&
						    output.composition != FR.Target.DENOMINATOR )
						{
							Debug.LogError("ExerciseLoader:ParseExercise composition can only be BOTH, NUMERATOR and DENOMINATOR");
							output.composition = FR.Target.BOTH;
						}
						//Debug.Log ("DIFFICULTY : " + output.difficulty);
						break;
					}
					case "WorldType":
					{
						temp = parser.content.ToUpper();
						
						if( temp == "PREVIOUS" )
						{
							output.worldType = previousWorldType;
						}
						else
						{
							try
							{
								output.worldType = (FR.WorldType) Enum.Parse(typeof(FR.WorldType), temp);

								if( output.worldType == FR.WorldType.NONE )
								{
									forceWorldTypeRandom = true;
								}
							}
							catch(Exception e)
							{
								Debug.LogError("ExerciseLoader:ParseExercise : WorldType could not be parsed! " + parser.content);
								output.worldType = FR.WorldType.NONE;
							}
						}
						break;
					}
					case "ExercisePart":
					{
						ExercisePart part = ParseExercisePart( output, parser );
						//if( part != null )
						//{
							output.parts.Add( part );
						//}
						break;
					}
				}
			}

			if( output.worldType == FR.WorldType.NONE && !forceWorldTypeRandom )
			{
				output.worldType = previousWorldType;
			}

			forceWorldTypeRandom = false;
			previousWorldType = output.worldType;
		}

		bool error = false;
		foreach( ExercisePart part in output.parts )
		{
			if( part == null )
			{
				error = true;
				break;
			}
		}

		if( error || output.difficulty == FR.ExerciseDifficulty.NONE ||
		    output.parts == null || output.parts.Count == 0 )
		{
			Debug.LogError("ExerciseLoader:ParseExercise : exercise had error or didn't have difficulty or parts set correctly!");
			return null;
		}

		Debug.Log ("ExerciseLoader:ParseExercise done");
		
		return output;
	}

	protected static ExercisePart ParseExercisePart(Exercise parent, TinyXmlReader parser)
	{
		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
		    (parser.tagName != "ExercisePart"))
		{
			Debug.Log("ExerciseLoader.ParseExercisePart(): unexpected tag type or tag name. " + parser.tagName + " // " + parser.tagType);
			return null;
		}

		Debug.Log ("ExerciseLoader:ParseExercisePart started");

		ExercisePart output = new ExercisePart();
		string temp = "";

		bool error = false;
		bool availableAll = false;
		while (parser.Read("ExercisePart"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "Fraction":
					{
						//output.fractions.Add( new Fraction(1,3) );
						output.fractions.Add( new Fraction(parser.content, parent.composition) );
						//output.difficulty = (FR.ExerciseDifficulty) Enum.Parse(typeof(FR.ExerciseDifficulty), parser.content);
						
						Fraction f = output.fractions[ output.fractions.Count - 1 ];
						if( f.Numerator.Value == -1 || f.Denominator.Value == - 1 )
							error = true;

						break;
					}
					case "Outcome":
					{
						output.outcomes.Add( new Fraction(parser.content, parent.composition) );
						//output.outcomes.Add( new Fraction(2,5) );
					
						Fraction f = output.outcomes[ output.outcomes.Count - 1 ];
						if( f.Numerator.Value == -1 || f.Denominator.Value == - 1 )
							error = true;

						break;
					}
					case "Operation":
					{
						FR.OperationType operation = FR.OperationType.NONE;
						try
						{
							temp = parser.content.ToUpper();
							operation = (FR.OperationType) Enum.Parse(typeof(FR.OperationType), temp);
						}
						catch(Exception e)
						{
							Debug.LogError("ExerciseLoader:ParseExercisePart : operation could not be parsed! " + parser.content);
							operation = FR.OperationType.NONE;
						}
						
						output.operations.Add(operation);

						break;
					}
					case "OperationAvailable":
					{
						temp = parser.content.ToUpper();
						if( temp == "ALL" )
						{
							availableAll = true;
						}
						else
						{
							FR.OperationType operation = FR.OperationType.NONE;
							try
							{
								operation = (FR.OperationType) Enum.Parse(typeof(FR.OperationType), temp);
							}
							catch(Exception e)
							{
								Debug.LogError("ExerciseLoader:ParseExercisePart : operationAvailable could not be parsed! " + parser.content);
								operation = FR.OperationType.NONE;
							}

							if( operation != FR.OperationType.NONE )
							{
								if( output.availableOperations == null )
								{
									output.availableOperations = new Dictionary<FR.OperationType, int>();
								}

								if( output.availableOperations.ContainsKey(operation) )
									output.availableOperations[operation] += 1;
								else
									output.availableOperations[operation] = 1;
							}
						}
						break;
					}
				}
			}
		}

		if( error || output.fractions == null || output.fractions.Count < 2 ||
		   output.outcomes == null || output.outcomes.Count == 0 ||
		   output.operations == null || output.operations.Count == 0 )
		{
			Debug.LogError("ExerciseLoader:ParseExercisePart : exercisePart has error or doesn't contain at least 2 fractions, 1 outcome and/or 1 operation! " + output.ToString());
			return null;
		}
		else
		{
			if( !availableAll )
			{
				// add the operations to the availableOperations
				if( output.availableOperations == null )
				{
					output.availableOperations = new Dictionary<FR.OperationType, int>();
				}

				foreach( FR.OperationType operation in output.operations )
				{
					if( output.availableOperations.ContainsKey(operation) )
						output.availableOperations[operation] += 1;
					else
						output.availableOperations[operation] = 1;
				}
			}
			else
			{
				// if availableOperations is null, we present the user with a choice of ALL the operations, with at least 3 of each
				// comes in handy with puzzle challenges / late game
				output.availableOperations = null;
			}

			Debug.Log ("ExerciseLoader:ParseExercisePart done : " + output.ToString() );
		}

		return output;
	}
}
