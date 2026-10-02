using UnityEngine;
using UnityEngine.Rendering.Universal;

// Descarga Reflexiva (Epica): ao ser atingido, ha uma chance de liberar uma onda de choque eletrica em volta.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Eletricidade/Descarga Reflexiva")]
public class DescargaReflexiva : DadosHabilidadePassiva
{
    [Header("Descarga Reflexiva:")]
    [SerializeField] private EscalaValor _chancePercentual = new EscalaValor(10f, 35f, 100);
    [SerializeField] private EscalaValor _raioArea = new EscalaValor(2f, 5f, 100);
    [Tooltip("Dano da onda em % do dano eletrico do dono")]
    [SerializeField] private EscalaValor _danoPercentualEletrico = new EscalaValor(150f, 300f, 100);
    [SerializeField] private Color _corNumeroDano = Color.yellow;

    [Header("Visual:")]
    [Tooltip("Vazio = usa a particula de raio do efeito fulminar")]
    [SerializeField] private GameObject _particulaOndaChoque;
    [SerializeField] private float _duracaoParticula = 1.5f;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<DescargaReflexiva>, IReageDanoRecebido
    {
        private GameObject _particulaOndaChoque;

        public Logica(DescargaReflexiva dados) : base(dados) { }

        protected override void AoAtivar()
        {
            _particulaOndaChoque = _dados._particulaOndaChoque != null
                ? _dados._particulaOndaChoque
                : Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Raio");
        }

        public void AoReceberDano(InformacaoDano informacao)
        {
            if (informacao.Atacante == null || !DonoVivo) return;
            if (Random.Range(0f, 100f) >= _dados._chancePercentual.Avaliar(Nivel)) return;

            LiberarOndaChoque();
        }

        private void LiberarOndaChoque()
        {
            Vector2 centro = CentroDe(Dono);
            float raio = _dados._raioArea.Avaliar(Nivel);

            GameObject particula = InstanciarParticula(_particulaOndaChoque, centro, _dados._duracaoParticula);
            Light2D luz = particula != null ? particula.GetComponentInChildren<Light2D>() : null;
            if (luz != null)
            {
                luz.pointLightOuterRadius = raio * 1.5f;
                luz.pointLightInnerRadius = raio * 0.5f;
            }

            float dano = Dono._poderEletricidade._dano * _dados._danoPercentualEletrico.Avaliar(Nivel) / 100f;
            foreach (Ser_Vivo inimigo in SeresVivosNaArea(centro, raio, CamadasAlvo))
                AplicarDanoHabilidade(inimigo, Utilidades.ArredondarNegativo(dano - inimigo._poderEletricidade._negacaoDano), Tipo_Dano.Eletricidade, _dados._corNumeroDano);

            Object.FindAnyObjectByType<Camera_Controller>()?.Tremer(raio);
        }
    }
}
