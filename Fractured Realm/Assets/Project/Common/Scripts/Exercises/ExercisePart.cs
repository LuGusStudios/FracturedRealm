﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExercisePart
{
	public List<Fraction> fractions = new List<Fraction>();
	public List<FR.OperationType> operations = new List<FR.OperationType>();
	public List<Fraction> outcomes = new List<Fraction>();

	public Fraction First
	{
		get
		{  
			if( fractions != null && fractions.Count > 0 )
			{
				return fractions[0];
			}
			else
				return null;
		}
	}

	public Fraction Last
	{
		get
		{
			if( fractions != null && fractions.Count > 0 )
			{
				return fractions[ fractions.Count - 1 ];
			}
			else
				return null;
		}
	}

	public override string ToString ()
	{
		return string.Format ("{0} {2} {1} = {3}", First, Last, operations[0], outcomes[0]);
	}
}
