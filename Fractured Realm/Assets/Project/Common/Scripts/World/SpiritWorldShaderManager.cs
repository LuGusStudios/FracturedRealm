using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiritWorldShaderManager : MonoBehaviour 
{
	public Shader[] preloadedShaders; // assign all grayscale shaders here so Shader.Find() will work!

	public static void MakeGrayScale( Transform parent )
	{
		Shader diffuseLighted = Shader.Find("Diffuse");
		Shader diffuseLightedGrayscale = Shader.Find("DiffuseGray");

		//Shader sprite = Shader.Find("Sprites/Default");
		//Shader spriteGrayscale = Shader.Find("Sprites/DefaultGray");

		Shader unlitTexture = Shader.Find("Unlit/Texture");
		Shader unlitTextureGrayscale = Shader.Find("Unlit/TextureGray");

		Shader particlesAlpha = Shader.Find ("Mobile/Particles/Alpha Blended");
		Shader particlesAlphaGrayscale = Shader.Find("Mobile/Particles/Alpha Blended Gray");

		Shader unlitAlpha = Shader.Find("Unlit/UnlitWithAlpha");
		Shader unlitAlphaGrayscale = Shader.Find("Unlit/UnlitWithAlphaGray");

		List<Renderer> renderers = new List<Renderer>();
		
		renderers.AddRange( parent.GetComponentsInChildren<MeshRenderer>() );

		foreach(Renderer r in renderers)
		{
			if( r.gameObject.GetComponent<CharacterRenderer>() != null )
				continue;

			foreach( Material mat in r.materials )
			{

				if( diffuseLighted && mat.shader == diffuseLighted )
				{
					mat.shader = diffuseLightedGrayscale;
				}
				else if( unlitTexture && mat.shader == unlitTexture )
				{
					mat.shader = unlitTextureGrayscale;
				}
				else if( particlesAlpha && mat.shader == particlesAlpha )
				{
					mat.shader = particlesAlphaGrayscale;
				}
				else if( unlitAlpha && mat.shader == unlitAlpha )
				{
					mat.shader = unlitAlphaGrayscale;
				}
			}
		}
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}
}
