// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PostEffect/Fog"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "./cginc/voronoi.cginc"
        
        float _FogDensity;
        float _FogDistance;
        float4 _FogColor;
        float4 _AmbientColor;
        
        float _FogNear;
        float _FogFar;
        float _FogAltScale;
        float _FogThinning;
        
        float _NoiseScale;
        float _NoiseStrength;

        float ComputeDistance(float depth)
        {
            float dist = depth * _ProjectionParams.z;
            dist -= _ProjectionParams.y * _FogDistance;
            return dist;
        }
        
        half ComputeFog(float z, float _Density)
        {
            half fog = 0.0;
            fog = exp2(_Density * z);
            //fog = _Density * z;
            //fog = exp2(-fog * fog);
            return saturate(fog);
        }

        float4 Frag (Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            //uvs
            float2 screenPos = input.texcoord.xy;
            float2 screenParam = _ScreenParams.xy;
            float2 uv = input.texcoord.xy;
            
            //base texture 
            float4 Color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, _BlitMipLevel);
            
            //background and color 
            float4 ambientColor = _AmbientColor; //float4(0.1,0.1,0.1,0.1);
            //depth handling
            float Depth = SampleSceneDepth(uv);
            float linearDepth = Linear01Depth(Depth, _ZBufferParams);
            //float finalDepth = linearDepth * _FogDistance;
            
            float dist = ComputeDistance(Depth);
            float fog = 1.0 - ComputeFog(dist, _FogDensity);
 
            float screenNoise = cnoise(screenPos * screenParam / _NoiseScale);            
  
            return lerp(Color, _FogColor * ambientColor , saturate(fog + (screenNoise * _NoiseStrength)) );
        }
    ENDHLSL
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
