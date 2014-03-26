using UnityEngine;
using System.Collections;

public class FractionFactoryMasks : MonoBehaviour, IFractionFactory 
{
	public Transform[] masks;
	
	public Fraction CreateFraction(int numerator, int denominator)
	{
		
		Fraction output = null;//new Fraction( NumberFactory.use.CreateNumber(numerator), NumberFactory.use.CreateNumber(denominator) );
		/*
		Transform newFractionObject = (new GameObject("" + numerator + "/" + denominator)).transform;
		FractionRenderer fractionRenderer = newFractionObject.gameObject.AddComponent<FractionRenderer>();
		
		fractionRenderer.Fraction = output;
		output.Renderer = fractionRenderer;
		
		
		output.Numerator.Renderer.transform.parent = newFractionObject;
		output.Denominator.Renderer.transform.parent = newFractionObject;
		*/
		
		return output;
	}
	
	void Awake () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
