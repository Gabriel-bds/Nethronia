using System.Collections;
using UnityEngine;

public class ParasitaCarmesim : Ataque
{
    [Header("Parasita Carmesim")]
    [SerializeField] private float _percentualDreno = 0.02f;
    [SerializeField] private float _intervaloDreno = 1f;
    [SerializeField] private float _duracaoTotal = 10f;
    [SerializeField] private float _percentualRoubo = 0.3f;

    private static readonly Color _corCarmesim = new Color(0.7f, 0f, 0.15f);
    private Inimigo[] _inimigosInscritos;

    protected override void Start()
    {
        gameObject.layer = default;
        ControlarEscalaVisualVitalidade();
        InscreversEmInimigos();
        StartCoroutine(Drenar());
        _dono._mao.GetComponent<Mao>().RemoverAtaqueDisponivel(gameObject);
    }

    private void ControlarEscalaVisualVitalidade()
    {
        Animator animador = GetComponent<Animator>();
        if (animador == null) return;
        float forca = Utilidades.LimitadorNumero(0, 1,
            (float)_dono._poderVitalidade._nivel / nivelMaximoMagnitudeVisual);
        animador.SetFloat("Forca", forca);
    }

    private void InscreversEmInimigos()
    {
        _inimigosInscritos = FindObjectsOfType<Inimigo>();
        foreach (Inimigo inimigo in _inimigosInscritos)
            inimigo.OnVidaAlterada += VerificarMorte;
    }

    private void VerificarMorte(Ser_Vivo serVivo, float vidaAtual, float vidaMax)
    {
        if (vidaAtual > 0) return;
        float valorRoubo = vidaMax * _percentualRoubo;
        Utilidades.AplicarDano(_dono, -valorRoubo, _corCarmesim);
    }

    private IEnumerator Drenar()
    {
        float tempoRestante = _duracaoTotal;
        while (tempoRestante > 0f)
        {
            float valorDreno = _dono._vidaMax * _percentualDreno;
            _dono.AplicarDano(valorDreno);
            Utilidades.InstanciarNumeroDano(valorDreno.ToString(), _dono.transform, _corCarmesim);
            yield return new WaitForSeconds(_intervaloDreno);
            tempoRestante -= _intervaloDreno;
        }
        Encerrar();
    }

    private void Encerrar()
    {
        foreach (ParticleSystem particula in GetComponentsInChildren<ParticleSystem>())
            particula.Stop();

        foreach (GameObject habilidade in _dono._mao.GetComponent<Mao>()._ataques)
        {
            if (habilidade.GetComponent<Ataque>()._idAtaque == _idAtaque)
                _dono._mao.GetComponent<Mao>().StartCoroutine(_dono._mao.GetComponent<Mao>().RecarregarAtaque(_tempoRecargaTotal, habilidade));
        }

        Destroy(gameObject, 1f);
    }

    private void OnDestroy()
    {
        if (_inimigosInscritos == null) return;
        foreach (Inimigo inimigo in _inimigosInscritos)
            if (inimigo != null)
                inimigo.OnVidaAlterada -= VerificarMorte;
    }
}
