using System.Collections;
using UnityEngine;

// Reserva Biologica (Lendaria): permite ressuscitar de X mortes com um percentual da vida.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Vitalidade/Reserva Biologica")]
public class ReservaBiologica : DadosHabilidadePassiva
{
    [Header("Reserva Biologica:")]
    [SerializeField] private EscalaValor _quantidadeRessurreicoes = new EscalaValor(2f, 5f, 100);
    [SerializeField] private EscalaValor _recuperacaoVidaPercentual = new EscalaValor(30f, 80f, 100);
    [SerializeField] private float _tempoInvulneravelAposRessurreicao = 1.5f;

    [Header("Visual:")]
    [SerializeField] private Color _corRessurreicao = new Color(1f, 0.25f, 0.35f);
    [SerializeField] private float _tempoTransicaoCor = 0.2f;
    [SerializeField] private GameObject _particulaRessurreicao;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<ReservaBiologica>, IReageMorteDono
    {
        private int _ressurreicoesUsadas;
        private int _quadroUltimaRessurreicao = -1;
        private float _vidaUltimaRessurreicao;

        public Logica(ReservaBiologica dados) : base(dados) { }

        private int RessurreicoesRestantes =>
            Mathf.Max(0, Mathf.RoundToInt(_dados._quantidadeRessurreicoes.Avaliar(Nivel)) - _ressurreicoesUsadas);

        public bool AoDonoMorrer()
        {
            // O Ser_Vivo.AplicarDano zera a vida duas vezes no mesmo golpe; a segunda nao gasta outra ressurreicao
            if (Time.frameCount == _quadroUltimaRessurreicao)
            {
                Dono.VidaAtual = _vidaUltimaRessurreicao;
                return true;
            }

            if (RessurreicoesRestantes <= 0) return false;

            _ressurreicoesUsadas++;
            _quadroUltimaRessurreicao = Time.frameCount;
            _vidaUltimaRessurreicao = Mathf.Max(1f, Dono._vidaMax * _dados._recuperacaoVidaPercentual.Avaliar(Nivel) / 100f);
            Dono.VidaAtual = _vidaUltimaRessurreicao;

            Utilidades.InstanciarNumeroDano("Ressurreicao!", Dono.transform, _dados._corRessurreicao, 12);
            InstanciarParticula(_dados._particulaRessurreicao, CentroDe(Dono), 3f, Dono.transform);
            IniciarRotina(ProtecaoAposRessurreicao());
            return true;
        }

        private IEnumerator ProtecaoAposRessurreicao()
        {
            Dono.Invulneravel(1);
            TingirMembros(_dados._corRessurreicao, _dados._tempoTransicaoCor);
            yield return new WaitForSeconds(_dados._tempoInvulneravelAposRessurreicao);
            RestaurarCorMembros(_dados._tempoTransicaoCor);
            Dono.Invulneravel(0);
        }
    }
}
