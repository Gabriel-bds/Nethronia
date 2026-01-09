using Cinemachine;
using System.Collections;
using UnityEngine;

public class Parar_Tempo : Ataque
{
    [Header("Configuração do Slow")]
    [Range(0.05f, 1f)]
    [SerializeField] float slowTimeScale = 0.3f;

    [Header("Transição do Tempo")]
    [SerializeField] float tempoTransicao = 0.5f;

    // ===== TIME =====
    float _timeScaleOriginal;
    float _fixedDeltaOriginal;

    // ===== PLAYER =====
    float _velocidadeBase;
    float _animSpeedBase;
    float _latenciaMiraBase;
    float _dragBase;

    // ===== CAMERA =====
    CinemachineVirtualCamera _camera;
    CinemachineFramingTransposer _transposer;

    float _camLookaheadBase;
    float _camXDampingBase;
    float _camYDampingBase;

    // ===== CONTROLE =====
    bool _tempoParado;
    Coroutine _rotinaTransicaoTempo;
    Coroutine _rotinaCompensacaoPlayer;

    // ===== ANIMATOR DA HABILIDADE =====
    Animator _animatorHabilidade;
    AnimatorUpdateMode _updateModeOriginal;

    protected override void Start()
    {
        // Animator da habilidade ignora TimeScale
        _animatorHabilidade = GetComponent<Animator>();
        if (_animatorHabilidade != null)
        {
            _updateModeOriginal = _animatorHabilidade.updateMode;
            _animatorHabilidade.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        //base.Start();

        transform.position = _dono.transform.position;

        StartCoroutine(FinalizarHabilidade());
        CongelarTempo();
        _dono._mao.GetComponent<Mao>().RemoverAtaqueDisponivel(gameObject);
    }

    public void CongelarTempo()
    {
        if (!_tempoParado)
            AtivarPararTempo();
        else
            DesativarPararTempo();
    }

    void AtivarPararTempo()
    {
        _dono.GetComponent<GhostTrail>().ativo = true;

        _tempoParado = true;

        // ===== TIME BASE =====
        _timeScaleOriginal = Time.timeScale;
        _fixedDeltaOriginal = Time.fixedDeltaTime;

        if (_rotinaTransicaoTempo != null)
            StopCoroutine(_rotinaTransicaoTempo);

        _rotinaTransicaoTempo = StartCoroutine(
            TransicaoTimeScale(_timeScaleOriginal, slowTimeScale)
        );

        float compensacaoFinal = 1f / slowTimeScale;

        // ===== PLAYER BASE =====
        _velocidadeBase = _dono._velocidadeMovimento;
        _animSpeedBase = _dono._animator.speed;
        _latenciaMiraBase = _dono._mao.GetComponent<Mao>()._latenciaMira;
        _dragBase = _dono._rigidbody.drag;

        if (_rotinaCompensacaoPlayer != null)
            StopCoroutine(_rotinaCompensacaoPlayer);

        _rotinaCompensacaoPlayer = StartCoroutine(
            TransicaoCompensacaoPlayer(1f, compensacaoFinal)
        );

        // ===== CAMERA =====
        _camera = FindObjectOfType<CinemachineVirtualCamera>();
        if (_camera != null)
        {
            _transposer = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (_transposer != null)
            {
                _camLookaheadBase = _transposer.m_LookaheadTime;
                _camXDampingBase = _transposer.m_XDamping;
                _camYDampingBase = _transposer.m_YDamping;

                _transposer.m_LookaheadTime = _camLookaheadBase * slowTimeScale;
                _transposer.m_XDamping = _camXDampingBase * slowTimeScale;
                _transposer.m_YDamping = _camYDampingBase * slowTimeScale;
            }
        }
    }

    void DesativarPararTempo()
    {
        _dono.GetComponent<GhostTrail>().ativo = false;

        _tempoParado = false;

        // ===== TIME =====
        if (_rotinaTransicaoTempo != null)
            StopCoroutine(_rotinaTransicaoTempo);

        _rotinaTransicaoTempo = StartCoroutine(
            TransicaoTimeScale(Time.timeScale, _timeScaleOriginal)
        );

        // ===== PLAYER =====
        if (_rotinaCompensacaoPlayer != null)
            StopCoroutine(_rotinaCompensacaoPlayer);

        _rotinaCompensacaoPlayer = StartCoroutine(
            TransicaoCompensacaoPlayer(
                _dono._velocidadeMovimento / _velocidadeBase,
                1f
            )
        );

        _dono._rigidbody.velocity = Vector2.zero;

        // ===== CAMERA =====
        if (_transposer != null)
        {
            _transposer.m_LookaheadTime = _camLookaheadBase;
            _transposer.m_XDamping = _camXDampingBase;
            _transposer.m_YDamping = _camYDampingBase;
        }

        if (_animatorHabilidade != null)
            _animatorHabilidade.updateMode = _updateModeOriginal;

        foreach (GameObject habilidade in _dono._mao.GetComponent<Mao>()._ataques)
        {
            if (habilidade.GetComponent<Ataque>()._idAtaque == _idAtaque)
            {
                _dono._mao.GetComponent<Mao>().StartCoroutine(_dono._mao.GetComponent<Mao>().RecarregarAtaque(_tempoRecargaTotal, habilidade));
            }
        }
    }

    IEnumerator TransicaoTimeScale(float de, float para)
    {
        float tempo = 0f;

        while (tempo < tempoTransicao)
        {
            tempo += Time.unscaledDeltaTime;
            float t = tempo / tempoTransicao;

            Time.timeScale = Mathf.Lerp(de, para, t);
            Time.fixedDeltaTime = _fixedDeltaOriginal * Time.timeScale;

            yield return null;
        }

        Time.timeScale = para;
        Time.fixedDeltaTime = _fixedDeltaOriginal * para;
    }

    IEnumerator TransicaoCompensacaoPlayer(float de, float para)
    {
        float tempo = 0f;

        while (tempo < tempoTransicao)
        {
            tempo += Time.unscaledDeltaTime;
            float t = tempo / tempoTransicao;

            float fator = Mathf.Lerp(de, para, t);

            _dono._velocidadeMovimento = _velocidadeBase * fator;
            _dono._animator.speed = _animSpeedBase * fator;
            _dono._mao.GetComponent<Mao>()._latenciaMira = _latenciaMiraBase / fator;
            _dono._rigidbody.drag = _dragBase * fator;

            yield return null;
        }

        _dono._velocidadeMovimento = _velocidadeBase * para;
        _dono._animator.speed = _animSpeedBase * para;
        _dono._mao.GetComponent<Mao>()._latenciaMira = _latenciaMiraBase / para;
        _dono._rigidbody.drag = _dragBase * para;
    }

    IEnumerator FinalizarHabilidade()
    {
        yield return new WaitForSecondsRealtime(_tempoDeVida);
        DesativarPararTempo();
        Destroy(gameObject, 1f);
    }

    void OnDisable()
    {
        if (_tempoParado)
            DesativarPararTempo();
    }
}
