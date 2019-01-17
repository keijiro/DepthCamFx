Shader "Alembic Point Cloud"
{
    Properties
    {
        _PointSize("Point Size", Float) = 0.01
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    float _PointSize;

    struct Varyings
    {
        float4 position : SV_POSITION;
    };

    Varyings Vertex(float4 position : POSITION)
    {
        Varyings v;
        v.position = UnityObjectToClipPos(position);
        return v;
    }

    void ConstructDisc(float4 origin, inout TriangleStream<Varyings> outStream)
    {
        float2 extent = abs(UNITY_MATRIX_P._11_22 * _PointSize);

        Varyings v;

        // Determine the number of slices based on the radius of the
        // point on the screen.
        float radius = extent.y / origin.w * _ScreenParams.y;
        uint slices = min((radius + 1) / 5, 4) + 2;

        // Slightly enlarge quad points to compensate area reduction.
        // Hopefully this line would be complied without branch.
        if (slices == 2) extent *= 1.2;

        // Top vertex
        v.position.y = origin.y + extent.y;
        v.position.xzw = origin.xzw;
        outStream.Append(v);

        UNITY_LOOP for (uint i = 1; i < slices; i++)
        {
            float sn, cs;
            sincos(UNITY_PI / slices * i, sn, cs);

            // Right side vertex
            v.position.xy = origin.xy + extent * float2(sn, cs);
            outStream.Append(v);

            // Left side vertex
            v.position.x = origin.x - extent.x * sn;
            outStream.Append(v);
        }

        // Bottom vertex
        v.position.x = origin.x;
        v.position.y = origin.y - extent.y;
        outStream.Append(v);

        outStream.RestartStrip();
    }

    [maxvertexcount(36)]
    void Geometry(
        triangle Varyings input[3],
        inout TriangleStream<Varyings> outStream
    )
    {
        ConstructDisc(input[0].position, outStream);
        ConstructDisc(input[1].position, outStream);
        ConstructDisc(input[2].position, outStream);
    }

    float4 Fragment(float4 position : SV_Position) : SV_Target
    {
        return 1;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            Cull Off
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment
            ENDCG
        }
        Pass
        {
            Cull Off
            Tags { "LightMode"="ShadowCaster" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment
            ENDCG
        }
    }
}
