using UnityEngine;

// Concentracao Infernal (Lendaria): com o dono parado, o dano da Rajada de fogo cresce a cada tique;
// ao se mover, cai na mesma proporcao (nunca abaixo do dano base da rajada).
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Fogo/Concentracao Infernal")]
public class ConcentracaoInfernal : DadosHabilidadePassiva
{
    [Header("Concentracao Infernal:")]
    [Tooltip("Aumento (%) do dano da rajada por tique parado")]
    [SerializeField] private EscalaValor _aumentoPorTiquePercentual = new EscalaValor(50f, 100f, 100);
    [Tooltip("Duracao de um tique de concentracao, em segundos")]
    [SerializeField] private float _duracaoTique = 1f;
    [Tooltip("Limite do multiplicador de dano (ex.: 5 = ate 5x o dano base)")]
    [SerializeField] private EscalaValor _multiplicadorMaximo = new EscalaValor(4f, 10f, 100);
    [SerializeField] private float _velocidadeConsideradaParado = 0.3f;

    [Header("Visual:")]
    [Tooltip("Multiplica a emissao das particulas da rajada conforme a concentracao (limitado a este valor)")]
    [SerializeField] private float _multiplicadorMaximoParticulas = 3f;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<ConcentracaoInfernal>, IReageAtaqueLancado, IAtualizavel, IModificaDanoCausado
    {
        private float _multiplicadorAtual = 1f;
        private Rajada _rajadaAtual;
        private ParticleSystem _particulasRajada;
        private float _emissaoBaseRajada;

        public Logica(ConcentracaoInfernal dados) : base(dados) { }

        public void AoLancarAtaque(Ataque ataque)
        {
            if (ataque._tipoDano != Tipo_Dano.Fogo || !(ataque is Rajada rajada)) return;

            _rajadaAtual = rajada;
            _multiplicadorAtual = 1f;
            _particulasRajada = rajada.GetComponent<ParticleSystem>();
            if (_particulasRajada != null)
                _emissaoBaseRajada = _particulasRajada.emission.rateOverTimeMultiplier;
        }

        public void Atualizar(float deltaTempo)
        {
            if (_rajadaAtual == null)
            {
                _multiplicadorAtual = 1f;
                return;
            }

            float variacao = _dados._aumentoPorTiquePercentual.Avaliar(Nivel) / 100f * deltaTempo / Mathf.Max(0.05f, _dados._duracaoTique);
            bool parado = Dono._rigidbody == null || Dono._rigidbody.linearVelocity.magnitude <= _dados._velocidadeConsideradaParado;

            _multiplicadorAtual = parado
                ? Mathf.Min(_multiplicadorAtual + variacao, _dados._multiplicadorMaximo.Avaliar(Nivel))
                : Mathf.Max(1f, _multiplicadorAtual - variacao);

            if (_particulasRajada != null)
            {
                var emissao = _particulasRajada.emission;
                emissao.rateOverTimeMultiplier = _emissaoBaseRajada * Mathf.Min(_multiplicadorAtual, _dados._multiplicadorMaximoParticulas);
            }
        }

        public float ModificarDanoCausado(InformacaoDano informacao)
        {
            if (informacao.Origem == null || informacao.Origem != _rajadaAtual) return informacao.Dano;
            return informacao.Dano * _multiplicadorAtual;
        }
    }
}
