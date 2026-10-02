using UnityEngine;

// Morte Congelante (Epica): quando um inimigo congelado morre, ele explode numa area que causa
// dano de gelo e acumulo de congelamento nos inimigos proximos.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Gelo/Morte Congelante")]
public class MorteCongelante : DadosHabilidadePassiva
{
    [Header("Morte Congelante:")]
    [SerializeField] private EscalaValor _raioArea = new EscalaValor(2f, 5f, 100);
    [SerializeField] private EscalaValor _dano = new EscalaValor(50f, 200f, 100);
    [Tooltip("Acumulo extra de congelamento, em % do acumulo maximo de cada inimigo atingido")]
    [SerializeField] private EscalaValor _acumuloCongelamentoPercentual = new EscalaValor(30f, 70f, 100);
    [SerializeField] private Color _corNumeroDano = new Color32(0, 145, 255, 255);

    [Header("Visual:")]
    [Tooltip("Vazio = usa a particula de flocos do efeito congelar")]
    [SerializeField] private GameObject _particulaExplosao;
    [SerializeField] private float _duracaoParticula = 1.5f;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<MorteCongelante>, IReageAbate
    {
        private EfeitoCongelar _efeitoCongelar;
        private GameObject _particulaExplosao;

        public Logica(MorteCongelante dados) : base(dados) { }

        protected override void AoAtivar()
        {
            _particulaExplosao = _dados._particulaExplosao != null
                ? _dados._particulaExplosao
                : Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Flocos");

            // Reaproveita a logica de congelamento existente
            _efeitoCongelar = CriarComponenteAuxiliar<EfeitoCongelar>("Efeito Morte Congelante");
        }

        public void AoMatar(Ser_Vivo vitima)
        {
            if (!EhAlvo(vitima) || !EstadosCombate.Possui(vitima, Estado_Combate.Congelado)) return;

            Vector2 centro = CentroDe(vitima);
            float raio = _dados._raioArea.Avaliar(Nivel);

            GameObject particula = InstanciarParticula(_particulaExplosao, centro, _dados._duracaoParticula);
            if (particula != null)
                particula.transform.localScale *= Mathf.Max(1f, raio / 2f);

            foreach (Ser_Vivo inimigo in SeresVivosNaArea(centro, raio, CamadasAlvo))
            {
                if (inimigo == vitima) continue;

                AplicarDanoHabilidade(inimigo, _dados._dano.Avaliar(Nivel), Tipo_Dano.Gelo, _dados._corNumeroDano);
                AcumularCongelamento(inimigo);
            }
        }

        private void AcumularCongelamento(Ser_Vivo inimigo)
        {
            if (inimigo == null || inimigo.VidaAtual <= 0 || _efeitoCongelar == null) return;

            Status statusGelo = inimigo._poderGelo._status;
            statusGelo._acumuloAtual += statusGelo._acumuloMax * _dados._acumuloCongelamentoPercentual.Avaliar(Nivel) / 100f;

            // O Aplicar soma o acumulo normal do dono e congela se passar do maximo
            _efeitoCongelar.Aplicar(Dono, inimigo);
        }
    }
}
