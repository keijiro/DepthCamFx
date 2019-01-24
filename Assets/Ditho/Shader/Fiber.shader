Shader "Hidden/Ditho/Warp"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Common.hlsl"
    #include "SimplexNoise2D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    float2 _DepthParams; // dist, cutoff
    float2 _CurveParams; // freq, speed
    float2 _NoiseParams; // amp, speed

    float3 _LineColor;
    float _Attenuation;

    float _LocalTime;

    void Vertex(
        uint vid : SV_VertexID,
        out float4 cs_position : SV_Position,
        out float alpha : COLOR
    )
    {
        // Noise parameters
        float n1 = vid * _CurveParams.x;
        float n2 = _LocalTime * _CurveParams.y;
        float n3 = _LocalTime * _NoiseParams.y;

        // UV coordinates
        float2 uv = float2(
            snoise(float2(n1, 98.32 + n2)),
            snoise(float2(12.32 - n2, n1))
        ) * 0.5 + 0.5;

        // Displacement sample
        float d = tex2Dlod(_MainTex, float4(uv, 0, 0)).x;

        // Object space position
        float3 os_pos = float3(uv - 0.5, 0);
        os_pos.xy *= float2(-1, _MainTex_TexelSize.x * _MainTex_TexelSize.w);
        os_pos = lerp(os_pos, float3(0, 0, _DepthParams.x), d);

        // Additional noise
        os_pos.z *= 1 + snoise(float2(n3, n1 * -10)) * _NoiseParams.x;

        // Attenuation noise
        float atten = saturate(10 * (snoise(_LocalTime * 2) / 2 + snoise(_LocalTime)));
        atten *= saturate(10 * snoise(os_pos.xy * 10 + _LocalTime * 10));
        atten = lerp(0, atten, saturate(_Attenuation * 2));
        atten = lerp(atten, 1, saturate(_Attenuation * 2 - 1));

        // Output
        cs_position = UnityObjectToClipPos(float4(os_pos, 1));
        alpha = d * atten;
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float alpha : COLOR
    ) : SV_Target
    {
        // Alpha to coverage
        float dither = Random(cs_position.x + cs_position.y * 10000);
        clip(alpha - (dither + 1) * _DepthParams.y / 2);

        return float4(_LineColor, 1);
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
