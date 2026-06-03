Shader "Custom/Entidades/MembroCamadas"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _EsqueletoTex ("Esqueleto", 2D) = "white" {}
        _Dano ("Dano", Range(0, 1)) = 0
        [HideInInspector] _EscalaRuido ("Escala do Ruido", Float) = 10.0
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
                float4 posicao : POSITION;
                float2 uv      : TEXCOORD0;
                float4 cor     : COLOR;
            };

            struct Fragmento
            {
                float4 posicaoClip   : SV_POSITION;
                float2 uv            : TEXCOORD0;
                float4 cor           : COLOR;
                float2 posObjeto     : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_EsqueletoTex);
            SAMPLER(sampler_EsqueletoTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _EsqueletoTex_ST;
                float4 _RendererColor;
                float _Dano;
                float _EscalaRuido;
            CBUFFER_END

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float valorNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(hash(i), hash(i + float2(1, 0)), f.x),
                    lerp(hash(i + float2(0, 1)), hash(i + float2(1, 1)), f.x),
                    f.y
                );
            }

            Fragmento vert(Atributos entrada)
            {
                Fragmento saida;
                saida.posicaoClip = TransformObjectToHClip(entrada.posicao.xyz);
                saida.uv          = TRANSFORM_TEX(entrada.uv, _MainTex);
                saida.cor         = entrada.cor * _RendererColor;
                saida.posObjeto   = entrada.posicao.xy;
                return saida;
            }

            half4 frag(Fragmento entrada) : SV_Target
            {
                half4 corPadrao    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, entrada.uv) * entrada.cor;
                half4 corEsqueleto = SAMPLE_TEXTURE2D(_EsqueletoTex, sampler_EsqueletoTex, entrada.uv);
                corEsqueleto.a    *= corPadrao.a;

                float ruido  = valorNoise(entrada.posObjeto * _EscalaRuido);
                float revelar = step(ruido, _Dano);

                return lerp(corPadrao, corEsqueleto, revelar);
            }
            ENDHLSL
        }
    }
}
