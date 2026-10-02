using UnityEngine;

// Rocha (Raro): ficar parado por um tempo cria um escudo que reduz o dano dos proximos X ataques.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Resistencia/Rocha")]
public class Rocha : DadosHabilidadePassiva
{
    [Header("Rocha:")]
    [SerializeField] private EscalaValor _reducaoDanoPercentual = new EscalaValor(25f, 60f, 100);
    [SerializeField] private EscalaValor _quantidadeAtaquesBloqueados = new EscalaValor(3f, 8f, 100);
    [Tooltip("Segundos parado necessarios para formar o escudo")]
    [SerializeField] private EscalaValor _tempoParadoNecessario = new EscalaValor(2f, 1f, 100);
    [Tooltip("Abaixo desta velocidade o dono e considerado parado")]
    [SerializeField] private float _velocidadeConsideradaParado = 0.3f;

    [Header("Visual:")]
    [SerializeField] private Color _corCorpoRocha = new Color(0.6f, 0.55f, 0.5f);
    [SerializeField] private float _tempoTransicaoCor = 0.25f;
    [SerializeField] private GameObject _particulaEscudoFormado;
    [SerializeField] private GameObject _particulaEscudoAtingido;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<Rocha>, IAtualizavel, IModificaDanoRecebido
    {
        private float _tempoParado;
        private int _ataquesRestantes;

        public Logica(Rocha dados) : base(dados) { }

        private bool EscudoAtivo => _ataquesRestantes > 0;

        protected override void AoDesativar()
        {
            if (EscudoAtivo) RestaurarCorMembros(_dados._tempoTransicaoCor);
        }

        public void Atualizar(float deltaTempo)
        {
            if (!DonoVivo || EscudoAtivo) return;

            bool parado = Dono._rigidbody == null || Dono._rigidbody.linearVelocity.magnitude <= _dados._velocidadeConsideradaParado;
            _tempoParado = parado ? _tempoParado + deltaTempo : 0f;

            if (_tempoParado >= _dados._tempoParadoNecessario.Avaliar(Nivel))
                FormarEscudo();
        }

        private void FormarEscudo()
        {
            _tempoParado = 0f;
            _ataquesRestantes = Mathf.Max(1, Mathf.RoundToInt(_dados._quantidadeAtaquesBloqueados.Avaliar(Nivel)));
            TingirMembros(Color.Lerp(Color.white, _dados._corCorpoRocha, 0.5f + FatorMagnitudeVisual * 0.5f), _dados._tempoTransicaoCor);
            InstanciarParticula(_dados._particulaEscudoFormado, CentroDe(Dono), 2f, Dono.transform);
        }

        public float ModificarDanoRecebido(InformacaoDano informacao)
        {
            if (!EscudoAtivo || informacao.Dano <= 0) return informacao.Dano;

            _ataquesRestantes--;
            InstanciarParticula(_dados._particulaEscudoAtingido, CentroDe(Dono), 1.5f);
            if (!EscudoAtivo)
                RestaurarCorMembros(_dados._tempoTransicaoCor);

            return informacao.Dano * (1f - Mathf.Clamp01(_dados._reducaoDanoPercentual.Avaliar(Nivel) / 100f));
        }
    }
}
