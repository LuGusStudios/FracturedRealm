//using UnityEngine;
//using System.Collections;
//
//public interface INumberFactory
//{
//	Number CreateNumber(int nr);
//}
//
//public class NumberFactory : INumberFactory
//{
//
//    private static INumberFactory _instance = null;
//
//    /// <summary>
//    /// Access SiteStructure.Instance to get the singleton object.
//    /// Then call methods on that instance.
//    /// </summary>
//    public static INumberFactory use
//    {
//		get 
//		{ 
//			if (_instance==null)
//            {
//				GameObject JESUS = GameObject.Find("JESUS");
//				if( JESUS == null )
//				{
//					JESUS = new GameObject("JESUS");
//				}
//				
//				_instance = JESUS.GetComponent<NumberFactoryMasks>();
//				if( _instance == null )
//					_instance = JESUS.AddComponent<NumberFactoryMasks>();
//			}
//			
//			return _instance; 
//		}
//	}
//	
//	public Number CreateNumber(int nr)
//	{
//		return NumberFactory.use.CreateNumber(nr);
//	}
//}
