Shader "Custom/URPGrass"
{
    Properties
    {
        [Header(Shading)]
        _TopColor("Top Color", Color) = (0.57, 0.84, 0.32, 1)
        _BottomColor("Bottom Color", Color) = (0.06, 0.38, 0.07, 1)
        _TranslucentGain("Translucent Gain", Range(0, 1)) = 0.5

        [Header(Blades (1 Unit is 1 Meter in VR))]
        _BladeHeight("Blade Height", Range(0.01, 2.0)) = 0.15
        _BladeHeightRandom("Blade Height Random", Range(0.0, 1.0)) = 0.05
        _BladeWidth("Blade Width", Range(0.005, 0.5)) = 0.02
        _BladeWidthRandom("Blade Width Random", Range(0.0, 0.2)) = 0.005
        _BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
        _BladeForward("Blade Forward Amount", Range(0, 1)) = 0.08
        _BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2

        [Header(Tessellation)]
        _TessellationUniform("Tessellation Uniform", Range(1, 64)) = 15

        [Header(Wind)]
        _WindDistortionMap("Wind Distortion Map", 2D) = "bump" {}
        _WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
        _WindStrength("Wind Strength", Range(0, 2)) = 0.3
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

    #define BLADE_SEGMENTS 3

    TEXTURE2D(_WindDistortionMap);
    SAMPLER(sampler_WindDistortionMap);

    CBUFFER_START(UnityPerMaterial)
        float4 _TopColor;
        float4 _BottomColor;
        float _TranslucentGain;
        float _BendRotationRandom;
        float _BladeHeight;
        float _BladeHeightRandom;
        float _BladeWidth;
        float _BladeWidthRandom;
        float _TessellationUniform;
        float4 _WindDistortionMap_ST;
        float2 _WindFrequency;
        float _WindStrength;
        float _BladeForward;
        float _BladeCurve;
    CBUFFER_END

    float rand(float3 co)
    {
        return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
    }

    float3x3 AngleAxis3x3(float angle, float3 axis)
    {
        float c, s;
        sincos(angle, s, c);

        float t = 1 - c;
        float x = axis.x;
        float y = axis.y;
        float z = axis.z;

        return float3x3(
            t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
            t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
            t * x * z - s * y,  t * y * z + s * x,  t * z * z + c
        );
    }

    struct vertexInput
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct vertexOutput
    {
        float4 vertex : SV_POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };

    struct TessellationFactors
    {
        float edge[3] : SV_TessFactor;
        float inside : SV_InsideTessFactor;
    };

    vertexInput vert(vertexInput v)
    {
        return v;
    }

    vertexOutput tessVert(vertexInput v)
    {
        vertexOutput o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex = v.vertex;
        o.normal = v.normal;
        o.tangent = v.tangent;
        return o;
    }

    TessellationFactors patchConstantFunction(InputPatch<vertexInput, 3> patch)
    {
        TessellationFactors f;
        f.edge[0] = _TessellationUniform;
        f.edge[1] = _TessellationUniform;
        f.edge[2] = _TessellationUniform;
        f.inside = _TessellationUniform;
        return f;
    }

    [domain("tri")]
    [outputcontrolpoints(3)]
    [outputtopology("triangle_cw")]
    [partitioning("integer")]
    [patchconstantfunc("patchConstantFunction")]
    vertexInput hull(InputPatch<vertexInput, 3> patch, uint id : SV_OutputControlPointID)
    {
        return patch[id];
    }

    [domain("tri")]
    vertexOutput domain(TessellationFactors factors, OutputPatch<vertexInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
    {
        vertexInput v;

        #define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) v.fieldName = \
            patch[0].fieldName * barycentricCoordinates.x + \
            patch[1].fieldName * barycentricCoordinates.y + \
            patch[2].fieldName * barycentricCoordinates.z;

        MY_DOMAIN_PROGRAM_INTERPOLATE(vertex)
        MY_DOMAIN_PROGRAM_INTERPOLATE(normal)
        MY_DOMAIN_PROGRAM_INTERPOLATE(tangent)
        UNITY_TRANSFER_INSTANCE_ID(patch[0], v);

        return tessVert(v);
    }

    struct geometryOutput
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 normalWS : TEXCOORD1;
        float3 positionWS : TEXCOORD2;
        float fogFactor : TEXCOORD3;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };

    float3 _LightDirection;
    float3 _LightPosition;

    geometryOutput VertexOutput(float3 posWS, float2 uv, float3 normWS, vertexOutput inputVertex)
    {
        geometryOutput o = (geometryOutput)0;

        // Transfer VR eye index (SV_RenderTargetArrayIndex) to each generated vertex
        UNITY_TRANSFER_INSTANCE_ID(inputVertex, o);
        UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(inputVertex, o);

        o.uv = uv;
        o.normalWS = normWS;
        o.positionWS = posWS;

        #ifdef SHADOW_CASTER_PASS
            #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                float3 lightDirectionWS = normalize(_LightPosition - posWS);
            #else
                float3 lightDirectionWS = _LightDirection;
            #endif
            o.pos = TransformWorldToHClip(ApplyShadowBias(posWS, normWS, lightDirectionWS));
            #if UNITY_REVERSED_Z
                o.pos.z = min(o.pos.z, UNITY_NEAR_CLIP_VALUE);
            #else
                o.pos.z = max(o.pos.z, UNITY_NEAR_CLIP_VALUE);
            #endif
        #else
            o.pos = TransformWorldToHClip(posWS);
        #endif

        o.fogFactor = ComputeFogFactor(o.pos.z);
        return o;
    }

    geometryOutput GenerateGrassVertex(float3 vertexPositionWS, float width, float height, float forward, float2 uv, float3x3 transformMatrix, vertexOutput inputVertex)
    {
        float3 tangentPoint = float3(width, forward, height);
        float3 tangentNormal = normalize(float3(0, -1, forward));

        float3 worldNormal = normalize(mul(transformMatrix, tangentNormal));
        float3 worldPosition = vertexPositionWS + mul(transformMatrix, tangentPoint);

        return VertexOutput(worldPosition, uv, worldNormal, inputVertex);
    }

    [maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
    void geo(triangle vertexOutput IN[3], inout TriangleStream<geometryOutput> triStream)
    {
        // Set up VR stereo eye matrices (unity_StereoEyeIndex) inside the Geometry Shader
        UNITY_SETUP_INSTANCE_ID(IN[0]);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN[0]);

        float3 posWS = TransformObjectToWorld(IN[0].vertex.xyz);
        float3 vNormalWS = TransformObjectToWorldNormal(IN[0].normal);
        float3 vTangentWS = TransformObjectToWorldDir(IN[0].tangent.xyz);
        float3 vBinormalWS = cross(vNormalWS, vTangentWS) * IN[0].tangent.w * GetOddNegativeScale();

        float3x3 tangentToWorld = float3x3(
            vTangentWS.x, vBinormalWS.x, vNormalWS.x,
            vTangentWS.y, vBinormalWS.y, vNormalWS.y,
            vTangentWS.z, vBinormalWS.z, vNormalWS.z
        );

        float3x3 facingRotationMatrix = AngleAxis3x3(rand(posWS) * TWO_PI, float3(0, 0, 1));
        float3x3 bendRotationMatrix = AngleAxis3x3(rand(posWS.zzx) * _BendRotationRandom * PI * 0.5, float3(-1, 0, 0));

        float2 uv = posWS.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
        float2 windSample = (SAMPLE_TEXTURE2D_LOD(_WindDistortionMap, sampler_WindDistortionMap, uv, 0).xy * 2 - 1) * _WindStrength;
        float3 wind = normalize(float3(windSample.x, windSample.y, 0.0001));

        float3x3 windRotation = AngleAxis3x3(PI * length(windSample), wind);

        float3x3 transformationMatrix = mul(mul(mul(tangentToWorld, windRotation), facingRotationMatrix), bendRotationMatrix);
        float3x3 transformationMatrixFacing = mul(tangentToWorld, facingRotationMatrix);

        float height = (rand(posWS.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
        float width = (rand(posWS.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
        float forward = rand(posWS.yyz) * _BladeForward;

        for (int i = 0; i < BLADE_SEGMENTS; i++)
        {
            float t = i / (float)BLADE_SEGMENTS;
            float segmentHeight = height * t;
            float segmentWidth = width * (1 - t);
            float segmentForward = pow(t, _BladeCurve) * forward;

            float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

            triStream.Append(GenerateGrassVertex(posWS, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix, IN[0]));
            triStream.Append(GenerateGrassVertex(posWS, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix, IN[0]));
        }

        triStream.Append(GenerateGrassVertex(posWS, 0, height, forward, float2(0.5, 1), transformationMatrix, IN[0]));
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 4.6
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma geometry geo
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            half4 frag(geometryOutput i, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float3 normal = isFrontFace ? normalize(i.normalWS) : -normalize(i.normalWS);

                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                float shadow = mainLight.shadowAttenuation;
                float NdotL = saturate(saturate(dot(normal, mainLight.direction)) + _TranslucentGain) * shadow;

                float3 ambient = SampleSH(normal);
                float4 lightIntensity = NdotL * float4(mainLight.color, 1.0) + float4(ambient, 1.0);

                float4 col = lerp(_BottomColor, _TopColor * lightIntensity, i.uv.y);
                col.rgb = MixFog(col.rgb, i.fogFactor);
                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma target 4.6
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma geometry geo
            #pragma fragment fragShadow

            #pragma multi_compile_instancing
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #define SHADOW_CASTER_PASS

            half4 fragShadow(geometryOutput i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                return 0;
            }
            ENDHLSL
        }
    }
}