using UnityEngine;
using System.Collections;

public interface IFractionFactory
{
	Fraction CreateFraction(int nominator, int denominator);
}

public class FractionFactory : IFractionFactory
{

    private static IFractionFactory _instance = null;

    /// <summary>
    /// Access SiteStructure.Instance to get the singleton object.
    /// Then call methods on that instance.
    /// </summary>
    public static IFractionFactory use
    {
		get 
		{ 
			if (_instance==null)
            {
				GameObject JESUS = GameObject.Find("JESUS");
				if( JESUS == null )
				{
					JESUS = new GameObject("JESUS");
				}
				
				_instance = JESUS.GetComponent<FractionFactoryMasks>();
				if( _instance == null )
					_instance = JESUS.AddComponent<FractionFactoryMasks>();
			}
			
			return _instance; 
		}
	}
	
	public Fraction CreateFraction(int nominator, int denominator)
	{
		return FractionFactory.use.CreateFraction(nominator, denominator);
	}
}
