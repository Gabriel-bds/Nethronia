using UnityEngine;

// Suspiro (Epica): abaixo de um limite de vida, o dano recebido diminui quanto menos vida o dono tiver.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Resistencia/Suspiro")]
public class Suspiro : DadosHabilidadePassiva
{
    [Header("Suspiro:")]
    [Tooltip("Reducao maxima de dano (%) quando a vida esta perto de zero")]
    [SerializeField] private EscalaValor _reducaoMaximaPercentual = new EscalaValor(50f, 80f, 100);
    [Tooltip("Abaixo deste percentual de vida a reducao comeca a valer")]
    [Range(0f, 100f)] [SerializeField] private float _limiteVidaPercentual = 50f;
    [SerializeField] private GameObject _particulaReducao;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<Suspiro>, IModificaDanoRecebido
    {
        public Logica(Suspiro dados) : base(dados) { }

        public float ModificarDanoRecebido(InformacaoDano informacao)
        {
            float vidaLimite = Dono._vidaMax * _dados._limiteVidaPercentual / 100f;
            if (vidaLimite <= 0 || Dono.VidaAtual >= vidaLimite) return informacao.Dano;

            // 0 no limite de vida, 1 com a vida beirando zero
            float proximidadeDaMorte = 1f - Dono.VidaAtual / vidaLimite;
            float reducao = _dados._reducaoMaximaPercentual.Avaliar(Nivel) / 100f * proximidadeDaMorte;

            InstanciarParticula(_dados._particulaReducao, CentroDe(Dono), 1.5f, Dono.transform);
            return informacao.Dano * (1f - Mathf.Clamp01(reducao));
        }
    }
}
