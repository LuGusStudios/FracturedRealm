//using UnityEngine;
//using System.Collections;
//
//public class NumberFactoryMasks : MonoBehaviour, INumberFactory 
//{
//	public Transform[] masks;
//	
//	public Number CreateNumber(int nr)
//	{
//		int index = nr - 1;
//		
//		if( (index > masks.Length - 1) || (index < 0) )
//		{
//			Debug.LogError("NumberFactoryMasks:CreateNumber : unknown nr : " + nr);
//			return null;
//		}
//		
//		Number output = new Number(nr);
//		
//		NumberRenderer numberRenderer = SpawnMask ( masks[index] );
//		
//		numberRenderer.Number = output;
//		output.Renderer = numberRenderer;
//		
//		// TODO : check for correct Prefab buildup
//		// ex. collider on top-level transform etc.
//		
//		
//		
//		return output;
//	}
//	
//	// removes the old renderer for this number and creates a new renderer
//	public void UpdateRenderer(Number number)
//	{		
//		int index = number.Value - 1;
//		
//		if( (index > masks.Length - 1) || (index < 0) )
//		{
//			Debug.LogError("NumberFactoryMasks:Update : unknown nr : " + (index + 1));
//			return;
//		}
//		
//		GameObject.Destroy(number.Renderer.gameObject);
//		
//		
//		NumberRenderer numberRenderer = SpawnMask ( masks[index] );
//		
//		numberRenderer.Number = number;
//		number.Renderer = numberRenderer;
//	}
//	
//	protected NumberRenderer SpawnMask( Transform prefab )
//	{
//		Transform output = (Transform) GameObject.Instantiate( prefab );
//		
//		output.gameObject.AddComponent<NumberAnimator>();
//		NumberRenderer numberRenderer = output.gameObject.AddComponent<NumberRenderer>();
//		
//		return numberRenderer;
//	}
//	
//	void Awake () 
//	{
//	
//	}
//	
//	// Update is called once per frame
//	void Update () 
//	{
//	
//	}
//}
