using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highlighter : MonoBehaviour 
{
	
	public Shader grayscaleShader = null;
	private Shader diffuseShader = null;
	private bool grayScaleOn = false;
	private Dictionary<Transform, Shader> shaderDictionary = new Dictionary<Transform, Shader>();
	
	
	private static Highlighter instance = null;
	public static Highlighter use
	{
		get
		{
			if (instance == null)
			{
				instance = (Highlighter)FindObjectOfType(typeof(Highlighter));
			}
			
			return instance;
		}
	}

	void Awake()
	{
		if( grayscaleShader == null )
		{
			grayscaleShader = Shader.Find("RGB -> Grey Lighted");
		}

		if( diffuseShader == null )
		{
			diffuseShader = Shader.Find("Diffuse");
		}
	}
	
	
	void Start () 
	{
		
	}
	
	public void ToggleGrayScale(Transform parentContainer)
	{
		grayScaleOn = !grayScaleOn;
		ShowGrayScale( grayScaleOn, parentContainer );
	}
	
	public void ToggleGrayScale(bool on, Transform parentContainer)
	{
		grayScaleOn = on;
		ShowGrayScale( grayScaleOn, parentContainer );
	}
	
	void ShowGrayScale(bool showing, Transform parentContainer)
	{
		List<Renderer> renderers = new List<Renderer>();
		
		renderers.AddRange( parentContainer.GetComponentsInChildren<MeshRenderer>() );
		renderers.AddRange( parentContainer.GetComponentsInChildren<SkinnedMeshRenderer>() );

		if (showing)
		{
			foreach(Renderer r in renderers)
			{

				if( r.gameObject.GetComponent<CharacterRenderer>() == null )
				{
					// keep a record of items not using standard diffuse shader
					if (r.material.shader != diffuseShader)
					{
						if (!shaderDictionary.ContainsKey(r.transform))
						{
							shaderDictionary.Add(r.transform, r.material.shader);
						}
					}
					
					r.material.shader = grayscaleShader;
				}
			}
		}
		else
		{
			foreach(Renderer r in renderers)
			{
				if ( r.gameObject.GetComponent<CharacterRenderer>() == null )
				{
					if (shaderDictionary.ContainsKey(r.transform))
					{
						r.material.shader = shaderDictionary[r.transform];
					}
					else
					{
						r.material.shader = diffuseShader;
					}
				}
			}
		}
	}
}
