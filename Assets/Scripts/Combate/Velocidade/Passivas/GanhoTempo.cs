using System.Collections.Generic;
using UnityEngine;

// Ganho de Tempo (Raro): acertar inimigos adianta a recarga de todas as outras habilidades.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Velocidade/Ganho de Tempo")]
public class GanhoTempo : DadosHabilidadePassiva
{
    [Header("Ganho de Tempo:")]
    [Tooltip("Percentual do tempo total de recarga adiantado a cada ataque que acerta")]
    [SerializeField] private EscalaValor _diminuicaoRecargaPercentual = new EscalaValor(10f, 30f, 100);
    [SerializeField] private GameObject _particulaRecargaAdiantada;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<GanhoTempo>, IReageDanoCausado
    {
        // Cada instancia de ataque so adianta as recargas uma vez (evita que rajadas/areas adiantem a cada tique)
        private readonly HashSet<Ataque> _ataquesJaContados = new HashSet<Ataque>();

        public Logica(GanhoTempo dados) : base(dados) { }

        public void AoCausarDano(InformacaoDano informacao)
        {
            if (informacao.Origem == null || !EhAlvo(informacao.Vitima)) return;

            _ataquesJaContados.RemoveWhere(ataque => ataque == null);
            if (!_ataquesJaContados.Add(informacao.Origem)) return;

            Mao mao = Dono._mao != null ? Dono._mao.GetComponent<Mao>() : null;
            if (mao == null) return;

            mao.AdiantarRecargas(_dados._diminuicaoRecargaPercentual.Avaliar(Nivel), informacao.Origem._idAtaque);
            InstanciarParticula(_dados._particulaRecargaAdiantada, CentroDe(Dono), 1f, Dono.transform);
        }
    }
}
