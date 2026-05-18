Shader "Custom/Combate/BrancoBlink"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributos
            {
                float4 posicao  : POSITION;
                float2 uv       : TEXCOORD0;
                float4 cor      : COLOR;
            };

            struct Fragmento
            {
                float4 posicaoClip : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 cor         : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _RendererColor;
            CBUFFER_END

            Fragmento vert(Atributos entrada)
            {
                Fragmento saida;
                saida.posicaoClip = TransformObjectToHClip(entrada.posicao.xyz);
                saida.uv          = TRANSFORM_TEX(entrada.uv, _MainTex);
                saida.cor         = entrada.cor * _RendererColor;
                return saida;
            }

            half4 frag(Fragmento entrada) : SV_Target
            {
                half alfa = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, entrada.uv).a;
                return half4(1.0, 1.0, 1.0, alfa * entrada.cor.a);
            }
            ENDHLSL
        }
    }
}
