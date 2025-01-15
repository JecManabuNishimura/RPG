Shader "Custom/FinalFantasyX"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        distortedTime("distortedTime", float) = 0
        _Glossiness("Glossiness",float) = 0  // 光沢の強さ
        _LightDirection("LightDirection",Vector) =  (0,0,0)// 光の方向
    }
    SubShader
    {
        Cull Off
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            struct appdata
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // 疑似乱数
            float rand(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 回転行列をかける
            float4 rotate(float4 pos, float4 axis, float angle)
            {
                float4 nAxis = normalize(axis);
                return mul(float4x4(cos(angle) + pow(nAxis.x, 2) * (1 - cos(angle)), nAxis.x * nAxis.y * (1 - cos(angle)) - nAxis.z * sin(angle), nAxis.x * nAxis.z * (1 - cos(angle)) + nAxis.y * sin(angle), 0,
                                nAxis.y * nAxis.x * (1 - cos(angle)) + nAxis.z * sin(angle), cos(angle) + pow(nAxis.y, 2) * (1 - cos(angle)), nAxis.y * nAxis.z * (1 - cos(angle)) - nAxis.x * sin(angle), 0,
                                nAxis.z * nAxis.x * (1 - cos(angle)) - nAxis.y * sin(angle), nAxis.z * nAxis.y * (1 - cos(angle)) + nAxis.x * sin(angle), cos(angle) + pow(nAxis.z, 2) * (1 - cos(angle)), 0,
                                0, 0, 0, 1
                    ), pos);
            };

            float distortedTime;
            sampler2D _MainTex;

            float _Glossiness; // 光沢の強さ
            float3 _LightDirection; // 光の方向

            appdata vert(appdata v)
            {
                return v;
            }

            [maxvertexcount(3)]
            void geom(triangle appdata input[3], inout TriangleStream<g2f> outStream)
            {
                float4 vertex0 = input[0].vertex;
                float4 vertex1 = input[1].vertex;
                float4 vertex2 = input[2].vertex;
                float3 norm = normalize(cross(vertex0.xyz - vertex1.xyz, vertex2.xyz - vertex0.xyz));
                float3 axis = normalize(rotate(vertex1 - vertex0, float4(norm, 0), rand(vertex0.xy) * 3.14159265358979 * 2));
                float4 center = (vertex0 + vertex1 + vertex2) / 3;
                float angle = rand(vertex0.xy) * distortedTime * 10;
                [unroll]
                for(int i = 0; i < 3; i++)
                {
                    float4 rotatedVert = center * ((distortedTime / 3) + 1 + pow(distortedTime, 2)) + rotate(input[i].vertex - center, float4(axis, 0), angle);
                    g2f o;
                    o.vertex = UnityObjectToClipPos(rotatedVert);
                    o.uv = input[i].uv; // 修正
                    outStream.Append(o);
                }

                outStream.RestartStrip();
            }

            float4 frag(g2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                
                // 法線ベクトル
                float3 normal = normalize(i.vertex.xyz);
                
                // 反射ベクトル
                float3 reflectDir = reflect(normalize(-_LightDirection), normal);
                
                // 視線ベクトル
                float3 viewDir = normalize(float3(0.0, 0.0, 1.0));
                
                // スペキュラーの計算
                float specular = pow(saturate(dot(reflectDir, viewDir)), 16.0);
                
                // 最終的な色
                float4 finalColor = color * (1 - _Glossiness) + specular * _Glossiness;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
