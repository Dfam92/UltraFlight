Shader "Custom/ConvexPlane"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BulgeStrength ("Bulge Strength", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BulgeStrength;

            v2f vert (appdata v)
            {
                v2f o;
                
                // Calculate distance from center
                float2 posXZ = v.vertex.xz;
                float dist = length(posXZ);

                // Deform based on distance (inverse parabola)
                float bulge = (1.0 - dist * dist) * _BulgeStrength;
                bulge = max(bulge, 0); // Evita valores negativos

                v.vertex.y += bulge;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}