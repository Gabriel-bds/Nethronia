using UnityEngine;

// Ganho de Massa (Epica): o dano fisico aumenta conforme a velocidade com que o dono esta se movendo.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Velocidade/Ganho de Massa")]
public class GanhoMassa : DadosHabilidadePassiva
{
    [Header("Ganho de Massa:")]
    [Tooltip("Aumento maximo de dano fisico (%) ao se mover na velocidade de referencia")]
    [SerializeField] private EscalaValor _ganhoForcaPercentual = new EscalaValor(10f, 60f, 100);
    [Tooltip("Velocidade (do Rigidbody) que concede o bonus maximo")]
    [SerializeField] private float _velocidadeReferencia = 10f;
    [SerializeField] private GameObject _particulaGolpeComMassa;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<GanhoMassa>, IModificaDanoCausado
    {
        public Logica(GanhoMassa dados) : base(dados) { }

        public float ModificarDanoCausado(InformacaoDano informacao)
        {
            if (informacao.TipoDano != Tipo_Dano.Fisico || Dono._rigidbody == null) return informacao.Dano;

            float fatorVelocidade = Mathf.Clamp01(Dono._rigidbody.linearVelocity.magnitude / Mathf.Max(0.01f, _dados._velocidadeReferencia));
            if (fatorVelocidade <= 0) return informacao.Dano;

            if (informacao.Vitima != null && fatorVelocidade > 0.5f)
                InstanciarParticula(_dados._particulaGolpeComMassa, CentroDe(informacao.Vitima), 1.5f);

            return informacao.Dano * (1f + _dados._ganhoForcaPercentual.Avaliar(Nivel) / 100f * fatorVelocidade);
        }
    }
}
