using UnityEngine;

// Mutacao Toxica (Lendaria): inimigos envenenados recebem mais dano e causam menos dano.
// Acumula a cada envenenamento ativo no inimigo.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Veneno/Mutacao Toxica")]
public class MutacaoToxica : DadosHabilidadePassiva
{
    [Header("Mutacao Toxica:")]
    [Tooltip("Dano extra (%) que o inimigo envenenado recebe, por envenenamento ativo")]
    [SerializeField] private EscalaValor _aumentoDanoRecebidoPercentual = new EscalaValor(10f, 25f, 100);
    [Tooltip("Reducao (%) do dano que o inimigo envenenado causa, por envenenamento ativo")]
    [SerializeField] private EscalaValor _reducaoDanoCausadoPercentual = new EscalaValor(10f, 25f, 100);
    [Tooltip("Limite da reducao do dano causado pelo inimigo (%)")]
    [Range(0f, 100f)] [SerializeField] private float _reducaoMaximaPercentual = 80f;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<MutacaoToxica>, IModificaDanoCausado, IModificaDanoRecebido
    {
        public Logica(MutacaoToxica dados) : base(dados) { }

        // O inimigo envenenado recebe mais dano do dono
        public float ModificarDanoCausado(InformacaoDano informacao)
        {
            if (!EhAlvo(informacao.Vitima)) return informacao.Dano;

            int envenenamentos = EstadosCombate.QuantidadeAcumulos(informacao.Vitima, Estado_Combate.Envenenado);
            if (envenenamentos <= 0) return informacao.Dano;

            return informacao.Dano * (1f + _dados._aumentoDanoRecebidoPercentual.Avaliar(Nivel) / 100f * envenenamentos);
        }

        // O inimigo envenenado causa menos dano no dono
        public float ModificarDanoRecebido(InformacaoDano informacao)
        {
            if (!EhAlvo(informacao.Atacante)) return informacao.Dano;

            int envenenamentos = EstadosCombate.QuantidadeAcumulos(informacao.Atacante, Estado_Combate.Envenenado);
            if (envenenamentos <= 0) return informacao.Dano;

            float reducao = Mathf.Min(_dados._reducaoMaximaPercentual, _dados._reducaoDanoCausadoPercentual.Avaliar(Nivel) * envenenamentos);
            return informacao.Dano * (1f - reducao / 100f);
        }
    }
}
