Shader "Unlit/DissolveShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _DissolveMap ("Dissolve Map", 2D) = "white" { }
        _DissolveAmount ("Dissolve Amount", Range (0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DissolveMap;
            fixed _DissolveAmount;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                fixed dissolveValue = tex2D(_DissolveMap, i.uv).r;
                clip(dissolveValue - _DissolveAmount);
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}