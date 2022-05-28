Shader "Winscreen" {
    Properties {
        _Color("Color", Color) = (1,1,1,0)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _SpecularColor("Specular", Color) = (0.2,0.2,0.2)
    }
 
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        LOD 200
       
        CGPROGRAM
        #pragma surface surf StandardSpecular fullforwardshadows
 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
 
        sampler2D _MainTex;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        half _Glossiness;
        fixed3 _SpecularColor;
        fixed4 _Color;
        #pragma surface surf StandardSpecular fullforwardshadows alpha:blend
        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// *_Color;
            o.Albedo = c.rgb * _Color;
            o.Specular = _SpecularColor;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
 