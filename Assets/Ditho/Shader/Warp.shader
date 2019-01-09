Shader "Hidden/Ditho/Warp"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Common.hlsl"
    #include "SimplexNoise3D.hlsl"

    float2 _Speed;
    float2 _Length;

    float3 _LineColor;
    float3 _SparkleColor;
    float _SparkleDensity;

    float _LocalTime;

    void Vertex(
        uint vid : SV_VertexID,
        out float4 cs_position : SV_Position,
        out float3 ws_position : TEXCOORD0
    )
    {
        uint seed = (vid / 2) * 4;
        uint sw = vid & 1;

        float spd = lerp(1 - _Speed .y, 1, Random(seed + 2)) * _Speed .x;
        float len = lerp(1 - _Length.y, 1, Random(seed + 3)) * _Length.x;

        float x = Random(seed + 0) - 0.5;
        float y = Random(seed + 1) - 0.5;
        float z = (spd * _LocalTime) % 1 - 0.5;

        float4 pos = float4(x, y, z + len * sw, 1);

        ws_position = mul(unity_ObjectToWorld, pos);
        cs_position = UnityWorldToClipPos(ws_position);
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 ws_position : TEXCOORD0
    ) : SV_Target
    {
        // World space position based noise field
        float nf = snoise(ws_position * 500);

        // Color mixing
        float3 lc = _LineColor * (1 + nf);
        float3 sc = _SparkleColor * smoothstep(1 - _SparkleDensity, 1, nf);
        return float4(lc + sc, 1);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 4.5
            ENDCG
        }
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 4.5
            ENDCG
        }
    }
}
