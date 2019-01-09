Shader "Hidden/Ditho/Surface Body"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Common.hlsl"
    #include "SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    float _DepthScale;

    float3 _LineColor;
    float _LineRepeat;

    float3 _SparkleColor;
    float _SparkleDensity;

    void Vertex(
        float2 uv : POSITION,
        out float4 cs_position : SV_Position,
        out float3 ws_position : TEXCOORD0,
        out float2 color : COLOR
    )
    {
        // Displacement sample
        float z = tex2Dlod(_MainTex, float4(uv, 0, 0)).x;

        // Object space position
        float aspect = _MainTex_TexelSize.x * _MainTex_TexelSize.w;
        float4 os_pos = float4(uv.x - 0.5, (uv.y - 0.5) * aspect, z * _DepthScale, 1);

        // Normal vector calculation
        float3 eps = float3(_MainTex_TexelSize.xy, 0);

        float depth_b = tex2Dlod(_MainTex, float4(uv - eps.zy, 0, 0)).x;
        float depth_t = tex2Dlod(_MainTex, float4(uv + eps.zy, 0, 0)).x;
        float depth_l = tex2Dlod(_MainTex, float4(uv - eps.xz, 0, 0)).x;
        float depth_r = tex2Dlod(_MainTex, float4(uv + eps.xz, 0, 0)).x;

        float3 bv_h = float3(eps.x, 0, (depth_r - depth_l) * _DepthScale);
        float3 bv_v = float3(0, eps.x, (depth_t - depth_b) * _DepthScale);

        float3 normal = normalize(cross(bv_h, bv_v));

        // Output
        ws_position = mul(unity_ObjectToWorld, os_pos);
        cs_position = UnityWorldToClipPos(ws_position);
        color = float2(normal.z, z);
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 ws_position : TEXCOORD0,
        float2 color : COLOR
    ) : SV_Target
    {
        float dither = Random(cs_position.x + cs_position.y * 10000);
        clip(color.y - 0.08 * dither);

        // Potential
        float pt = ws_position.y * _LineRepeat;

        // Line intensity
        float li = saturate(1 - abs(0.5 - frac(pt)) / fwidth(pt));

        // World space position based noise field
        float nf = snoise(ws_position * 500);

        // Color mixing
        float3 lc = _LineColor * (1 + nf);
        float3 sc = _SparkleColor * smoothstep(1 - _SparkleDensity, 1, nf);
        return float4(li * color.x * (lc + sc), 1);
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
