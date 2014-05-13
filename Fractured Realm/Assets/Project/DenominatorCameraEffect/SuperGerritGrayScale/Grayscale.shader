Shader "RGB -> Grey" {

    

Properties {

    _MainTex ("Texture", 2D) = ""

}

 

SubShader {Pass {

    GLSLPROGRAM

    varying mediump vec2 uv;

    

    #ifdef VERTEX

    void main () {

        gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

        uv = gl_MultiTexCoord0.xy;

    }

    #endif

    

    #ifdef FRAGMENT

    uniform lowp sampler2D _MainTex;

    void main () {

        gl_FragColor = vec4(

            vec3(dot(texture2D(_MainTex, uv).rgb, vec3(.222, .707, .071)) ), 1);

    }

    #endif

    ENDGLSL

}}

 

}