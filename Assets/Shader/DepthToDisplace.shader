Shader "Unlit/DepthToDisplace"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    float _DepthScale;

    void Vertex(
        float4 position : POSITION,
        float2 texcoord : TEXCOORD0,
        out float4 cs_position : SV_Position,
        out float3 ws_position : TEXCOORD0,
        out float4 color : COLOR
    )
    {
        float4 uv = float4(texcoord, 0, 0);

        float depth = tex2Dlod(_MainTex, uv).x;
        position.z += depth * _DepthScale;

        float3 eps = float3(_MainTex_TexelSize.xy, 0);
        float depth_b = tex2Dlod(_MainTex, uv - eps.zyzz).x;
        float depth_t = tex2Dlod(_MainTex, uv + eps.zyzz).x;
        float depth_l = tex2Dlod(_MainTex, uv - eps.xzzz).x;
        float depth_r = tex2Dlod(_MainTex, uv + eps.xzzz).x;

        float3 norm = /*normalize(cross(
            float3(kkkk
        ));*/
        
        
         float3(
            (depth_r - depth_l) * _DepthScale / eps.x / 2,
            (depth_t - depth_b) * _DepthScale / eps.y / 2,
            1
        );
        norm = normalize(norm);

        ws_position = mul(unity_ObjectToWorld, position);
        cs_position = UnityWorldToClipPos(ws_position);
        color = norm.z * smoothstep(0.01, 0.02, depth);
    }

    float4 Fragment(
        float4 cs_position : SV_Position,
        float3 ws_position : TEXCOORD0,
        float4 color : COLOR
    ) : SV_Target
    {
        float y = ws_position.y * 400;

        float yi = abs(0.5 - frac(y));
        yi = 1 - smoothstep(0.0, fwidth(y), yi);

        float3 c = yi * color;

        //c *= 0 + 1 *Random(dot(abs(ws_position), float3(50, 50 * 50, 50 * 50 * 50)));
        //c *=2;
        float nn = snoise(ws_position * 600);
        c *= 1 + nn;

        c = GammaToLinearSpace(saturate(c));
        c *= 1 + smoothstep(0.5, 1, nn) * 60;

        return float4(c, 1);
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
            ENDCG
        }
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
