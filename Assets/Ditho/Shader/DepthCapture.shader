Shader "Hidden/Ditho/Depth Capture"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _CameraDepthTexture;

    float4 Vertex(
        float4 position : POSITION,
        inout float2 texcoord : TEXCOORD
    ) : SV_Position
    {
        return UnityObjectToClipPos(position);
    }

    float4 Fragment(
        float4 position : SV_Position,
        float2 texcoord : TEXCOORD
    ) : SV_Target
    {
        return 1 - Linear01Depth(tex2D(_CameraDepthTexture, texcoord));
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 4.5
            ENDCG
        }
    }
}
