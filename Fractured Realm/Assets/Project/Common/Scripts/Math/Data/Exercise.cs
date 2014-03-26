using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Exercise
{
	public List<Fraction> fractions = new List<Fraction>();
	
	public void Add(Fraction f)
	{
		fractions.Add(f);
	}
}
