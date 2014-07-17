using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CampaignLoader 
{
	public static Campaign LoadCampaign(int nr)
	{
		Campaign output = new Campaign();
		output.name = "Campaign_" + nr;
		output.nr = nr;

		int count = 1;
		CampaignPart part = null;
		do
		{
			part = LoadCampaignPart(output.name + "_" + count);

			if( part != null )
			{
				part.nr = count;
				part.parent = output;
				output.parts.Add( part );
			}

			++count;
		}
		while( part != null );
		
		return output;
	}

	public static CampaignPart LoadCampaignPart( string name )
	{
		CampaignPart output = null;
		
		TextAsset file = LugusResources.use.Shared.GetTextAsset( name );
		
		if( file == LugusResources.use.errorTextAsset )
		{
			return null;
		}
		
		output = new CampaignPart();
		output.fileName = name;
		
		TinyXmlReader parser = new TinyXmlReader(file.text);
		
		while ( parser.Read() ) 
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "Name":
					{
						output.name = parser.content;
						break;
					}
					case "Description":
					{
						output.description = parser.content;
						break;
					}
					case "WorldType":  
					{
						try
						{
							output.worldType = (FR.WorldType) Enum.Parse(typeof(FR.WorldType), parser.content);
						}
						catch(Exception e)
						{
							Debug.LogError("CampaignLoader:LoadCampaignPart : WorldType could not be parsed! " + parser.content);
							output.worldType = FR.WorldType.NONE;
						}
						break;
					}
					case "ExerciseGroup":
					{
						output.exerciseGroups.Add( parser.content );
						Debug.LogError ("ExerciseGroup : added " + parser.content);
						break;
					}
				}
			}
		}
		
		return output;
	}
}
