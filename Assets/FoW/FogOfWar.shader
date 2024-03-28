// Credit to Mina Pêcheux's "Let's make a RTS game in Unity/C#!" Chapters 13-15 for contents of this shader script and implementation
// Credit to Andrew Hung's "Implementing Attractive Fog of War in Unity" 

Shader "Custom/FogOfWar"
{
    Properties
    {
        _FullTexture ("Full Texture", 2D) = "white" {}
        _SemiTexture ("Semi Texture", 2D) = "white" {}
        _SemiOpacity ("Semi Opacity", float) = 0.5
        _Color ("Color", Color) = (0,0,0,0) // Black with complete transparency
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+100" } // Covers other transparent objects

        Pass
        {
            ZWrite Off // Prevents writing Z (depth) values to the Depth Buffer
            Blend SrcAlpha OneMinusSrcAlpha // Blends the color of fog with the materials in screen (for semi-transparency fog)
            ZTest Equal // Only draws pixels that are equal to Z values in the Depth Buffer

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4x4 unity_Projector;
            sampler2D _FullTexture;
            sampler2D _SemiTexture;
            fixed4 _Color;
            float _SemiOpacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = mul(unity_Projector, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 UV = i.uv;

                float aFull = tex2Dproj (_FullTexture, UV).a;
                float aSemi = tex2Dproj (_SemiTexture, UV).a;

                float a = aFull + _SemiOpacity * aSemi;

                // Minimap Shit
                _Color.a = max(0, _Color.a - a);
                return _Color;
            }
            ENDCG // End line for CG Program
        }
    }
}
