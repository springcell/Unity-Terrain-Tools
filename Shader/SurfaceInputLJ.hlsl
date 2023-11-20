#ifndef UNIVERSAL_INPUT_SURFACE_LJ_INCLUDED
#define UNIVERSAL_INPUT_SURFACE_LJ_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "./SurfaceDataLJ.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
TEXTURE2D(_BaseMap1);       SAMPLER(sampler_BaseMap1);
TEXTURE2D(_BaseMap2);       SAMPLER(sampler_BaseMap2);
TEXTURE2D(_BaseMap3);       SAMPLER(sampler_BaseMap3);

float4 _BaseMap_TexelSize;
float4 _BaseMap_MipInfo;
float4 _BaseMap2_TexelSize;
float4 _BaseMap2_MipInfo;
float4 _BaseMap3_TexelSize;
float4 _BaseMap3_MipInfo;
float4 _BaseMap4_TexelSize;
float4 _BaseMap4_MipInfo;
TEXTURE2D(_BumpMap);      SAMPLER(sampler_BumpMap);
TEXTURE2D(_BumpMap1);       SAMPLER(sampler_BumpMap1);
TEXTURE2D(_BumpMap2);       SAMPLER(sampler_BumpMap2);
TEXTURE2D(_BumpMap3);       SAMPLER(sampler_BumpMap3);
TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);

TEXTURE2D(_Splatmap);       SAMPLER(sampler_Splatmap);

///////////////////////////////////////////////////////////////////////////////
//                      Material Property Helpers                            //
///////////////////////////////////////////////////////////////////////////////
half Alpha(half albedoAlpha, half4 color, half cutoff)
{
#if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
    half alpha = albedoAlpha * color.a;
#else
    half alpha = color.a;
#endif

#if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
#endif

    return alpha;
}

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return half4(SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv));
}

half3 SampleNormal(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_bumpMap), half scale = half(1.0))
{
#ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
    #if BUMP_SCALE_NOT_SUPPORTED
        return UnpackNormal(n);
    #else
        return UnpackNormalScale(n, scale);
    #endif
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleEmission(float2 uv, half3 emissionColor, TEXTURE2D_PARAM(emissionMap, sampler_emissionMap))
{
#ifndef _EMISSION
    return 0;
#else
    return SAMPLE_TEXTURE2D(emissionMap, sampler_emissionMap, uv).rgb * emissionColor;
#endif
}

#endif
