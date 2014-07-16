Shader "Unlit/UnlitWithAlphaGray"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)   
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white"
    }
 
    Category
    {
        Lighting Off
        ZWrite Off
                //ZWrite On  // uncomment if you have problems like the sprite disappear in some rotations.
        Cull back
        Blend SrcAlpha OneMinusSrcAlpha
                //AlphaTest Greater 0.001  // uncomment if you have problems like the sprites or 3d text have white quads instead of alpha pixels.
        Tags {Queue=Transparent}
 
        SubShader
        {
 
             Pass
             {
		    
		    	Color(.389, .1465, .4645, 0.5) // grayscale modifier
		
                SetTexture [_MainTex]
                {
                   ConstantColor [_Color]
                   Combine Texture * constant
                }
                
				SetTexture[_] {Combine previous Dot3 primary} // this one multiplies by the grayscal color above
            }
        }
    }
}