using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Roubo de Velocidade (Comum): golpes acumulam nos inimigos; ao atingir o limite, eles ficam mais lentos.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Velocidade/Roubo de Velocidade")]
public class RouboVelocidade : DadosHabilidadePassiva
{
    [Header("Roubo de Velocidade:")]
    [Tooltip("Acumulo somado no inimigo a cada golpe")]
    [SerializeField] private EscalaValor _acumuloPorGolpe = new EscalaValor(1f, 3f, 100);
    [Tooltip("Acumulo necessario para desacelerar o inimigo")]
    [SerializeField] private float _acumuloNecessario = 5f;
    [SerializeField] private EscalaValor _desaceleracaoPercentual = new EscalaValor(20f, 60f, 100);
    [SerializeField] private EscalaValor _duracaoDesaceleracao = new EscalaValor(3f, 6f, 100);
    [SerializeField] private Color _corAviso = new Color(0.4f, 0.8f, 1f);
    [SerializeField] private GameObject _particulaDesaceleracao;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<RouboVelocidade>, IReageDanoCausado, IReageAbate
    {
        private readonly Dictionary<Ser_Vivo, float> _acumuloPorInimigo = new Dictionary<Ser_Vivo, float>();
        private readonly Dictionary<Ser_Vivo, Coroutine> _desaceleracoesAtivas = new Dictionary<Ser_Vivo, Coroutine>();
        private readonly Dictionary<Ser_Vivo, float> _fatorAplicadoPorInimigo = new Dictionary<Ser_Vivo, float>();

        public Logica(RouboVelocidade dados) : base(dados) { }

        public void AoCausarDano(InformacaoDano informacao)
        {
            if (informacao.Origem == null || !EhAlvo(informacao.Vitima)) return;

            Ser_Vivo inimigo = informacao.Vitima;
            _acumuloPorInimigo.TryGetValue(inimigo, out float acumulo);
            acumulo += _dados._acumuloPorGolpe.Avaliar(Nivel);

            if (acumulo >= _dados._acumuloNecessario)
            {
                acumulo = 0f;
                Desacelerar(inimigo);
            }
            _acumuloPorInimigo[inimigo] = acumulo;
        }

        public void AoMatar(Ser_Vivo vitima)
        {
            _acumuloPorInimigo.Remove(vitima);
        }

        private void Desacelerar(Ser_Vivo inimigo)
        {
            if (inimigo.VidaAtual <= 0) return;

            if (_desaceleracoesAtivas.TryGetValue(inimigo, out Coroutine desaceleracaoAnterior) && desaceleracaoAnterior != null)
            {
                PararRotina(desaceleracaoAnterior);
            }
            else
            {
                float fator = Mathf.Max(0.05f, 1f - _dados._desaceleracaoPercentual.Avaliar(Nivel) / 100f);
                _fatorAplicadoPorInimigo[inimigo] = fator;
                AplicarFatorVelocidade(inimigo, fator);
            }

            float duracao = _dados._duracaoDesaceleracao.Avaliar(Nivel);
            EstadosCombate.Registrar(inimigo, Estado_Combate.Desacelerado, duracao);
            Utilidades.InstanciarNumeroDano("Lento!", inimigo.transform, _dados._corAviso, 8);
            InstanciarParticula(_dados._particulaDesaceleracao, CentroDe(inimigo), duracao, inimigo.transform);

            _desaceleracoesAtivas[inimigo] = IniciarRotina(EncerrarDesaceleracao(inimigo, duracao));
        }

        private IEnumerator EncerrarDesaceleracao(Ser_Vivo inimigo, float duracao)
        {
            yield return new WaitForSeconds(duracao);
            _desaceleracoesAtivas.Remove(inimigo);
            DevolverVelocidade(inimigo);
        }

        private void DevolverVelocidade(Ser_Vivo inimigo)
        {
            if (!_fatorAplicadoPorInimigo.TryGetValue(inimigo, out float fator)) return;

            _fatorAplicadoPorInimigo.Remove(inimigo);
            if (inimigo != null)
                AplicarFatorVelocidade(inimigo, 1f / fator);
        }

        private static void AplicarFatorVelocidade(Ser_Vivo serVivo, float fator)
        {
            if (float.IsInfinity(fator) || float.IsNaN(fator)) return;

            NavMeshAgent agente = serVivo.GetComponent<NavMeshAgent>();
            if (agente != null)
                agente.speed *= fator;
            else
                serVivo._velocidadeMovimento *= fator;
        }

        protected override void AoDesativar()
        {
            // Devolve a velocidade de quem ainda estava lento
            foreach (Ser_Vivo inimigo in new List<Ser_Vivo>(_fatorAplicadoPorInimigo.Keys))
                DevolverVelocidade(inimigo);
            _desaceleracoesAtivas.Clear();
        }
    }
}
