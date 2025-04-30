Shader "Custom/ConvexPlaneChequered"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BulgeStrength ("Bulge Strength", Float) = 0.5
        _BulgeExponent ("Bulge Exponent", Float) = 2.0
        _BulgeScale ("Bulge Scale", Float) = 0.5
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
            float _BulgeExponent;
            float _BulgeScale;

            v2f vert (appdata v)
            {
                v2f o;

                float2 posXZ = v.vertex.xz * _BulgeScale; // <= controla o espalhamento
                float dist = length(posXZ);

                float bulge = (1.0 - pow(dist, _BulgeExponent)) * _BulgeStrength;
                bulge = max(bulge, 0.0);

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