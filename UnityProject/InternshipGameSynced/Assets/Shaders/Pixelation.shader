Shader "PostEffect/Pixelation"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        
        //for Pixelation      
        float _WidthPixelation;
        float _HeightPixelation;
        
        //for color precision
        float _ColorPrecision;
        
        float4 Frag (Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            //pixelation 
            float2 uv = input.texcoord.xy;
            uv.x = floor(uv.x * _WidthPixelation) / _WidthPixelation;
            uv.y = floor(uv.y * _HeightPixelation) / _HeightPixelation;
            
            float4 Color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_PointClamp, uv, _BlitMipLevel);
            //color precision
            Color = floor(Color * _ColorPrecision)/_ColorPrecision;
            return Color;
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
