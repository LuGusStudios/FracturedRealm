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

				if( e != null )
				{
					output.exercises.Add( e );
				}
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

		Exercise output = new Exercise();

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
							output.difficulty = (FR.ExerciseDifficulty) Enum.Parse(typeof(FR.ExerciseDifficulty), parser.content);
						}
						catch(Exception e)
						{
							Debug.LogError("ExerciseLoader:ParseExercise : difficulty could not be parsed! " + parser.content);
							output.difficulty = FR.ExerciseDifficulty.NONE;
						}
						Debug.Log ("DIFFICULTY : " + output.difficulty);
						break;
					}
					case "ExercisePart":
					{
						ParseExercisePart( parser );
						break;
					}
				}
			}
		}

		Debug.Log ("ExerciseLoader:ParseExercise done");
		
		return output;
	}

	protected static ExercisePart ParseExercisePart(TinyXmlReader parser)
	{
		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
		    (parser.tagName != "ExercisePart"))
		{
			Debug.Log("ExerciseLoader.ParseExercisePart(): unexpected tag type or tag name. " + parser.tagName + " // " + parser.tagType);
			return null;
		}

		Debug.Log ("ExerciseLoader:ParseExercisePart started");

		ExercisePart output = new ExercisePart();

		while (parser.Read("ExercisePart"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "Fraction":
					{
						output.fractions.Add( new Fraction(1,3) );
						//output.difficulty = (FR.ExerciseDifficulty) Enum.Parse(typeof(FR.ExerciseDifficulty), parser.content);
						break;
					}
					case "Outcome":
					{
						output.outcomes.Add( new Fraction(2,5) );
						break;
					}
					case "Operation":
					{
						output.operations.Add( FR.OperationType.SIMPLIFY );
						break;
					}
				}
			}
		}
		
		Debug.Log ("ExerciseLoader:ParseExercisePart done : " + output.ToString() );

		return output;
	}
}
