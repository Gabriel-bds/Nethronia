Shader "Custom/Entidades/MembroCamadas"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _EsqueletoTex ("Esqueleto", 2D) = "white" {}
        [PerRendererData] _MusculoTex ("Musculo", 2D) = "white" {}
        _Intensidade ("Intensidade (1=normal, 0=dano maximo)", Range(0, 1)) = 1
        _Frequencia ("Frequencia Esqueleto", Float) = 10.0
        _OffsetRuido ("Offset Ruido Esqueleto", Vector) = (0, 0, 0, 0)
        _FrequenciaMusculo ("Frequencia Musculo", Float) = 10.0
        _OffsetRuidoMusculo ("Offset Ruido Musculo", Vector) = (0, 0, 0, 0)
        _PicoMusculo ("Pico Musculo", Range(0, 1)) = 0.5
        _PicoEsqueleto ("Pico Esqueleto (negativo = nunca total no range normal)", Float) = -0.5
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
                float4 posicaoClip : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 cor         : COLOR;
                float2 posObjeto   : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_EsqueletoTex);
            SAMPLER(sampler_EsqueletoTex);
            TEXTURE2D(_MusculoTex);
            SAMPLER(sampler_MusculoTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _EsqueletoTex_ST;
                float4 _MusculoTex_ST;
                float4 _RendererColor;
                float4 _OffsetRuido;
                float4 _OffsetRuidoMusculo;
                float _Intensidade;
                float _Frequencia;
                float _FrequenciaMusculo;
                float _PicoMusculo;
                float _PicoEsqueleto;
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
                half4 corMusculo   = SAMPLE_TEXTURE2D(_MusculoTex, sampler_MusculoTex, entrada.uv);
                corEsqueleto.a    *= corPadrao.a;
                corMusculo.a      *= corPadrao.a;

                // progresso = (1 - Intensidade) / (1 - Pico)
                // threshold = 1 - progresso  →  step(threshold, noise) revela a camada

                float2 posMus  = entrada.posObjeto * _FrequenciaMusculo + _OffsetRuidoMusculo.xy;
                float2 warpMus = float2(
                    valorNoise(posMus * 0.5 + float2(1.7, 9.2)) - 0.5,
                    valorNoise(posMus * 0.5 + float2(8.3, 2.8)) - 0.5
                ) * 1.5;
                float progressoMusculo = saturate((1.0 - _Intensidade) / (1.0 - _PicoMusculo));
                float revelarMusculo   = step(1.0 - progressoMusculo, valorNoise(posMus + warpMus));

                float2 posEsq  = entrada.posObjeto * _Frequencia + _OffsetRuido.xy;
                float2 warpEsq = float2(
                    valorNoise(posEsq * 0.5 + float2(1.7, 9.2)) - 0.5,
                    valorNoise(posEsq * 0.5 + float2(8.3, 2.8)) - 0.5
                ) * 1.5;
                float progressoEsqueleto = saturate((1.0 - _Intensidade) / (1.0 - _PicoEsqueleto));
                float revelarEsqueleto   = step(1.0 - progressoEsqueleto, valorNoise(posEsq + warpEsq));

                // Blend: musculo sobrepoe sprite padrao, esqueleto sobrepoe tudo
                half4 cor = lerp(corPadrao, corMusculo, revelarMusculo);
                cor       = lerp(cor, corEsqueleto, revelarEsqueleto);
                return cor;
            }
            ENDHLSL
        }
    }
}
