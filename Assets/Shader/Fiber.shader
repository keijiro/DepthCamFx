Shader "Hidden/DepthCamFx/Fiber"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Common.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float _DepthScale;
    float3 _LineColor;

    void Vertex(
        uint vid : SV_VertexID,
        out float4 cs_position : SV_Position,
        out float3 color : COLOR
    )
    {
        // UV coordinate
        float4 uv = float4(Random(vid * 2), Random(vid * 2 + 1), 0, 0);

        // Object space position
        float4 pos = float4(uv.xy - 0.5, 0, 1);
        pos.y *= _MainTex_TexelSize.x * _MainTex_TexelSize.w;

        // Displacement by depth
        float depth = tex2Dlod(_MainTex, uv).x;
        pos.z += depth * _DepthScale;

        // Output
        cs_position = UnityObjectToClipPos(pos);
        color = depth;
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 color : COLOR
    ) : SV_Target
    {
        clip(color.r - 0.1);
        return float4(_LineColor * color, 1);
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
