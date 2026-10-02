using UnityEngine;

// Sobrecarga (Lendaria): acertar inimigos com habilidades eletricas gera cargas; ao chegar no limite,
// o proximo ataque eletrico e amplificado.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Eletricidade/Sobrecarga")]
public class Sobrecarga : DadosHabilidadePassiva
{
    [Header("Sobrecarga:")]
    [SerializeField] private EscalaValor _cargasParaAtivar = new EscalaValor(15f, 8f, 100);
    [SerializeField] private EscalaValor _aumentoDanoEletricoPercentual = new EscalaValor(100f, 250f, 100);

    [Header("Visual:")]
    [SerializeField] private Color _corCorpoSobrecarregado = new Color(1f, 0.95f, 0.4f);
    [SerializeField] private float _tempoTransicaoCor = 0.2f;
    [Tooltip("Particula que fica no dono enquanto ele esta sobrecarregado")]
    [SerializeField] private GameObject _particulaSobrecarregado;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<Sobrecarga>, IReageDanoCausado, IModificaDanoCausado
    {
        private int _cargasAtuais;
        private bool _sobrecarregado;
        private Ataque _ataqueAmplificado;
        private GameObject _instanciaParticulaSobrecarregado;

        public Logica(Sobrecarga dados) : base(dados) { }

        protected override void AoDesativar()
        {
            if (_sobrecarregado) RestaurarCorMembros(_dados._tempoTransicaoCor);
            if (_instanciaParticulaSobrecarregado != null) Object.Destroy(_instanciaParticulaSobrecarregado);
        }

        public void AoCausarDano(InformacaoDano informacao)
        {
            if (informacao.TipoDano != Tipo_Dano.Eletricidade || !EhAlvo(informacao.Vitima)) return;
            if (_sobrecarregado || (informacao.Origem != null && informacao.Origem == _ataqueAmplificado)) return;

            _cargasAtuais++;
            if (_cargasAtuais >= Mathf.Max(1, Mathf.RoundToInt(_dados._cargasParaAtivar.Avaliar(Nivel))))
                Sobrecarregar();
        }

        private void Sobrecarregar()
        {
            _cargasAtuais = 0;
            _sobrecarregado = true;
            TingirMembros(_dados._corCorpoSobrecarregado, _dados._tempoTransicaoCor);
            Utilidades.InstanciarNumeroDano("Sobrecarga!", Dono.transform, _dados._corCorpoSobrecarregado, 10);
            _instanciaParticulaSobrecarregado = InstanciarParticula(_dados._particulaSobrecarregado, CentroDe(Dono), 0f, Dono.transform);
        }

        public float ModificarDanoCausado(InformacaoDano informacao)
        {
            if (informacao.TipoDano != Tipo_Dano.Eletricidade || informacao.Origem == null || !EhAlvo(informacao.Vitima)) return informacao.Dano;

            // O primeiro ataque eletrico depois de sobrecarregar e o amplificado (vale para todos os alvos dele)
            if (_sobrecarregado)
            {
                _sobrecarregado = false;
                _ataqueAmplificado = informacao.Origem;
                RestaurarCorMembros(_dados._tempoTransicaoCor);
                if (_instanciaParticulaSobrecarregado != null)
                    Object.Destroy(_instanciaParticulaSobrecarregado);
            }

            if (informacao.Origem != _ataqueAmplificado) return informacao.Dano;
            return informacao.Dano * (1f + _dados._aumentoDanoEletricoPercentual.Avaliar(Nivel) / 100f);
        }
    }
}
