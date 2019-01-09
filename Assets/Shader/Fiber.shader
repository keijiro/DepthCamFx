Shader "Hidden/DepthCamFx/Fiber"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise2D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float _DepthScale;

    float2 _CurveParams;
    float2 _NoiseParams;

    float3 _LineColor;
    float _LocalTime;

    void Vertex(
        uint vid : SV_VertexID,
        out float4 cs_position : SV_Position,
        out float3 color : COLOR
    )
    {
        float n1 = vid * _CurveParams.x;
        float n2 = _LocalTime * _CurveParams.y;
        float n3 = _LocalTime * _NoiseParams.y;

        float x = snoise(float2(n1, 98.32 + n2)) * 0.5;
        float y = snoise(float2(12.32 - n2, n1)) * 0.5;
        float z = tex2Dlod(_MainTex, float4(x + 0.5, y + 0.5, 0, 0)).x;

        z *= 1 + snoise(float2(n3, n1 * -10)) * _NoiseParams.x;

        float aspect = _MainTex_TexelSize.x * _MainTex_TexelSize.w;
        float4 os_pos = float4(x, y * aspect, z * _DepthScale, 1);

        cs_position = UnityObjectToClipPos(os_pos);
        color = z > 0.01 ? 0.001 : -1e+5;
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 color : COLOR
    ) : SV_Target
    {
        clip(color.r);
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
