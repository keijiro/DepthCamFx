Shader "Hidden/Ditho/Surface"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Common.hlsl"
    #include "SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float2 _DepthParams; // dist, cutoff
    float2 _NoiseParams; // amp, anim

    float3 _LineColor;
    float2 _LineParams; // repeat, width

    float3 _SparkleColor;
    float _SparkleDensity;

    float _LocalTime;

    void Vertex(
        float2 uv : POSITION,
        out float4 cs_position : SV_Position,
        out float3 ws_position : TEXCOORD0,
        out float3 color : COLOR
    )
    {
        // Displacement sample
        float d = tex2Dlod(_MainTex, float4(uv, 0, 0)).x;

        // Object space position
        float3 os_pos = float3(uv - 0.5, 0);
        os_pos.xy *= float2(-1, _MainTex_TexelSize.x * _MainTex_TexelSize.w);
        os_pos = lerp(os_pos, float3(0, 0, _DepthParams.x), d);

        // Additional noise
        float3 np = float3(uv * 20, _LocalTime * _NoiseParams.y);
        os_pos += snoise_grad(np).xyz * float3(0.1, 0.1, 1) * _NoiseParams.x;

        // Normal vector calculation
        float3 eps = float3(_MainTex_TexelSize.xy, 0);

        float depth_b = tex2Dlod(_MainTex, float4(uv - eps.zy, 0, 0)).x;
        float depth_t = tex2Dlod(_MainTex, float4(uv + eps.zy, 0, 0)).x;
        float depth_l = tex2Dlod(_MainTex, float4(uv - eps.xz, 0, 0)).x;
        float depth_r = tex2Dlod(_MainTex, float4(uv + eps.xz, 0, 0)).x;

        float3 bv_h = float3(eps.x, 0, (depth_r - depth_l) * 2);
        float3 bv_v = float3(0, eps.x, (depth_t - depth_b) * 2);

        float3 normal = normalize(cross(bv_h, bv_v));

        // Output
        ws_position = mul(unity_ObjectToWorld, float4(os_pos, 1));
        cs_position = UnityWorldToClipPos(ws_position);
        color = float3(normal.z, uv.y, d);
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 ws_position : TEXCOORD0,
        float3 color : COLOR
    ) : SV_Target
    {
        // Alpha to coverage
        float dither = Random(cs_position.x + cs_position.y * 10000);
        clip(color.z - (dither + 1) * _DepthParams.y / 2);

        // Potential
        float pt = color.y * _LineParams.x;

        // Line intensity
        float li = saturate(1 - abs(0.5 - frac(pt)) / (fwidth(pt) * _LineParams.y));

        // World space position based noise field
        float nf = snoise(ws_position * 500);

        // Color mixing
        float3 lc = _LineColor * (1 + nf);
        float3 sc = _SparkleColor * smoothstep(1 - _SparkleDensity, 1, nf);
        return float4(li * saturate(color.x) * (lc + sc), 1);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            ZWrite [_ZWrite]
            Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend]
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
