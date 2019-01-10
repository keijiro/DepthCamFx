Shader "Hidden/Ditho/Warp"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Common.hlsl"
    #include "SimplexNoise3D.hlsl"

    float2 _DepthParams; // dist, cutoff
    float3 _Extent;

    float2 _Speed;
    float2 _Length;

    float3 _LineColor;
    float3 _SparkleColor;
    float _SparkleDensity;

    float _LocalTime;

    void Vertex(
        uint vid : SV_VertexID,
        out float4 cs_position : SV_Position,
        out float3 ws_position : TEXCOORD0,
        out float color : COLOR
    )
    {
        uint seed = (vid / 2) * 4;
        uint sw = vid & 1;

        float spd = lerp(1 - _Speed .y, 1, Random(seed + 2)) * _Speed .x;
        float len = lerp(1 - _Length.y, 1, Random(seed + 3)) * _Length.x;

        float3 pos = float3(
            Random(seed + 0) - 0.5,
            Random(seed + 1) - 0.5,
            (spd * _LocalTime) % 1 - len * sw
        );
        color = pos.z;

        pos *= _Extent;
        pos = float3(pos.xy, _DepthParams.x) * pos.z;

        ws_position = mul(unity_ObjectToWorld, float4(pos, 1));
        cs_position = UnityWorldToClipPos(ws_position);
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 ws_position : TEXCOORD0,
        float alpha : COLOR
    ) : SV_Target
    {
        // Alpha to coverage
        float dither = Random(cs_position.x + cs_position.y * 10000);
        clip(alpha - (dither + 1) * _DepthParams.y / 2);

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
