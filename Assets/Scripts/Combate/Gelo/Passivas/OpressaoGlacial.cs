using UnityEngine;

// Opressao Glacial (Lendaria): para cada inimigo congelado, o dano de gelo do dono aumenta.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Gelo/Opressao Glacial")]
public class OpressaoGlacial : DadosHabilidadePassiva
{
    [Header("Opressao Glacial:")]
    [Tooltip("Aumento (%) do dano de gelo por inimigo congelado")]
    [SerializeField] private EscalaValor _aumentoDanoPorCongeladoPercentual = new EscalaValor(10f, 30f, 100);
    [Tooltip("Limite do aumento total (%)")]
    [SerializeField] private float _aumentoMaximoPercentual = 300f;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<OpressaoGlacial>, IModificaDanoCausado
    {
        public Logica(OpressaoGlacial dados) : base(dados) { }

        public float ModificarDanoCausado(InformacaoDano informacao)
        {
            if (informacao.TipoDano != Tipo_Dano.Gelo) return informacao.Dano;

            int congelados = EstadosCombate.QuantidadeSeresVivosCom(Estado_Combate.Congelado);
            if (congelados <= 0) return informacao.Dano;

            float aumento = Mathf.Min(_dados._aumentoMaximoPercentual, _dados._aumentoDanoPorCongeladoPercentual.Avaliar(Nivel) * congelados);
            return informacao.Dano * (1f + aumento / 100f);
        }
    }
}
