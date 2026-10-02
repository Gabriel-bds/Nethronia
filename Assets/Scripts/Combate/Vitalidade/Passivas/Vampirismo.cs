using UnityEngine;

// Vampirismo (Epica): cada dano causado a um inimigo devolve um percentual como vida.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Vitalidade/Vampirismo")]
public class Vampirismo : DadosHabilidadePassiva
{
    [Header("Vampirismo:")]
    [SerializeField] private EscalaValor _rouboVidaPercentual = new EscalaValor(15f, 40f, 100);
    [Tooltip("Curas menores que isso sao acumuladas ate passar do valor (evita encher a tela de numeros)")]
    [SerializeField] private float _curaMinimaExibida = 1f;
    [SerializeField] private Color _corCura = new Color(0.8f, 0f, 0.2f);
    [SerializeField] private GameObject _particulaDreno;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<Vampirismo>, IReageDanoCausado
    {
        private float _curaAcumulada;

        public Logica(Vampirismo dados) : base(dados) { }

        public void AoCausarDano(InformacaoDano informacao)
        {
            if (!EhAlvo(informacao.Vitima) || informacao.Dano <= 0) return;

            _curaAcumulada += informacao.Dano * _dados._rouboVidaPercentual.Avaliar(Nivel) / 100f;
            if (_curaAcumulada < _dados._curaMinimaExibida) return;

            Curar(_curaAcumulada, _dados._corCura);
            _curaAcumulada = 0f;
            InstanciarParticula(_dados._particulaDreno, CentroDe(informacao.Vitima), 1.5f);
        }
    }
}
