using System.Collections;
using UnityEngine;

// Sangue Mortal (Epica): o sangue que o dono derrama vira uma poca venenosa; inimigos nela
// levam dano de veneno e acumulo de envenenamento.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Veneno/Sangue Mortal")]
public class SangueMortal : DadosHabilidadePassiva
{
    [Header("Sangue Mortal:")]
    [Tooltip("Dano por tique em % do dano de veneno do dono")]
    [SerializeField] private EscalaValor _danoPercentualVeneno = new EscalaValor(10f, 40f, 100);
    [SerializeField] private EscalaValor _raioPoca = new EscalaValor(1f, 2.5f, 100);
    [SerializeField] private EscalaValor _duracaoPoca = new EscalaValor(5f, 12f, 100);
    [SerializeField] private float _intervaloTique = 1f;
    [Tooltip("Tempo minimo entre duas pocas (evita uma poca por golpe numa sequencia rapida)")]
    [SerializeField] private float _intervaloMinimoEntrePocas = 0.5f;
    [SerializeField] private Color _corNumeroDano = new Color32(60, 160, 60, 255);

    [Header("Visual:")]
    [Tooltip("Vazio = usa a particula de envenenado")]
    [SerializeField] private GameObject _particulaPoca;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<SangueMortal>, IReageDanoRecebido
    {
        private EfeitoVeneno _efeitoVeneno;
        private GameObject _particulaPoca;
        private float _tempoUltimaPoca = -100f;

        public Logica(SangueMortal dados) : base(dados) { }

        protected override void AoAtivar()
        {
            _particulaPoca = _dados._particulaPoca != null
                ? _dados._particulaPoca
                : Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Envenenado");

            // Reaproveita a logica de acumulo de envenenamento existente
            _efeitoVeneno = CriarComponenteAuxiliar<EfeitoVeneno>("Efeito Sangue Mortal");
        }

        public void AoReceberDano(InformacaoDano informacao)
        {
            if (informacao.Dano <= 0 || Time.time - _tempoUltimaPoca < _dados._intervaloMinimoEntrePocas) return;

            _tempoUltimaPoca = Time.time;
            IniciarRotina(PocaVenenosa(Dono.transform.position));
        }

        private IEnumerator PocaVenenosa(Vector2 posicao)
        {
            float raio = _dados._raioPoca.Avaliar(Nivel);
            float tempoRestante = _dados._duracaoPoca.Avaliar(Nivel);

            GameObject particula = InstanciarParticula(_particulaPoca, posicao, tempoRestante);
            if (particula != null)
                particula.transform.localScale *= raio;

            while (tempoRestante > 0f)
            {
                float dano = Dono != null ? Dono._poderVeneno._dano * _dados._danoPercentualVeneno.Avaliar(Nivel) / 100f : 0f;
                foreach (Ser_Vivo inimigo in SeresVivosNaArea(posicao, raio, CamadasAlvo))
                {
                    AplicarDanoHabilidade(inimigo, Utilidades.ArredondarNegativo(dano - inimigo._poderVeneno._negacaoDano), Tipo_Dano.Veneno, _dados._corNumeroDano);
                    if (inimigo != null && inimigo.VidaAtual > 0 && _efeitoVeneno != null)
                        _efeitoVeneno.Aplicar(Dono, inimigo);
                }

                yield return new WaitForSeconds(_dados._intervaloTique);
                tempoRestante -= _dados._intervaloTique;
            }
        }
    }
}
