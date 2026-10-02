using UnityEngine;

// Ceifador (Raro): ganha um percentual da vida maxima de cada inimigo que matar.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Vitalidade/Ceifador")]
public class Ceifador : DadosHabilidadePassiva
{
    [Header("Ceifador:")]
    [Tooltip("Percentual da vida maxima do inimigo morto que vira cura")]
    [SerializeField] private EscalaValor _ganhoVidaPorInimigoPercentual = new EscalaValor(5f, 25f, 100);
    [SerializeField] private Color _corCura = new Color(0.55f, 0f, 0.1f);
    [SerializeField] private GameObject _particulaAlmaCeifada;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<Ceifador>, IReageAbate
    {
        public Logica(Ceifador dados) : base(dados) { }

        public void AoMatar(Ser_Vivo vitima)
        {
            if (!EhAlvo(vitima)) return;

            Curar(vitima._vidaMax * _dados._ganhoVidaPorInimigoPercentual.Avaliar(Nivel) / 100f, _dados._corCura);
            InstanciarParticula(_dados._particulaAlmaCeifada, CentroDe(Dono), 2f, Dono.transform);
        }
    }
}
