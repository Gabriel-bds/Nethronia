using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escudo : Ataque
{
    [Header("Escudo:")]
    [SerializeField] int nivelHabilidade;
    [SerializeField] float aumentoPercentualAtributos;
    [SerializeField] float tempoEscudo;
    [SerializeField] float aumentoPercentualAtributosPorNivel;
    [SerializeField] float aumentoTempoEscudoPorNivel;
    //[SerializeField] int nivelForcaMaximoParaAumento;
    [SerializeField] float aumentoMinimoPercentualAtributos;
    [SerializeField] float tempoMinimoEscudo;
    [SerializeField] float tempoTransicaoCor = 0.25f;
    public Color corCorpoEscudo;
    public Color corMinimaCorpoEscudo;

    List<SpriteRenderer> spritesMembros = new();
    Color corPadrao;

    // Start is called before the first frame update
    private void Awake()
    {
        //MagnitudeVisualFuria();
    }
    void Start()
    {
        transform.SetParent(_dono.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        foreach (var membro in _dono._membros)
        {
            var sr = membro.GetComponent<SpriteRenderer>();
            if (sr != null)
                spritesMembros.Add(sr);
        }

        corPadrao = spritesMembros[0].color;
        //MagnitudeVisualFuria();
        DefinirNivelEscudo();
        StartCoroutine(FuriaCoroutine());
        _dono._mao.GetComponent<Mao>().RemoverAtaqueDisponivel(gameObject);
        //ControlarRecarga();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void DefinirNivelEscudo()
    {
        aumentoPercentualAtributos = aumentoMinimoPercentualAtributos + aumentoPercentualAtributosPorNivel * nivelHabilidade;
        tempoEscudo = tempoMinimoEscudo + aumentoTempoEscudoPorNivel * nivelHabilidade;
    }
    IEnumerator AumentarAtributos()
    {
        float resistenciaPadrao = _dono._poderResistencia._negacaoDano;
        float resistenciaRepulsaoPadrao = _dono._poderResistencia._negacaoRepulsao;

        _dono._poderResistencia._negacaoDano += _dono._poderResistencia._negacaoDano * aumentoPercentualAtributos / 100f;
        _dono._poderResistencia._negacaoRepulsao += _dono._poderResistencia._negacaoRepulsao * aumentoPercentualAtributos / 100f;
        yield return new WaitForSeconds(tempoEscudo);
        Debug.Log("Final furia");
        _dono._poderResistencia._negacaoDano = resistenciaPadrao;
        _dono._poderResistencia._negacaoRepulsao = resistenciaRepulsaoPadrao;
    }

    IEnumerator FuriaCoroutine()
    {
        // Transição para a cor do escudo
        yield return StartCoroutine(TransicaoCor(corPadrao, corCorpoEscudo, tempoTransicaoCor));

        // Buffa atributos
        yield return StartCoroutine(AumentarAtributos());

        // Transição de volta para a cor normal
        yield return StartCoroutine(TransicaoCor(corCorpoEscudo, corPadrao, tempoTransicaoCor));

        foreach (GameObject habilidade in _dono._mao.GetComponent<Mao>()._ataques)
        {
            if (habilidade.GetComponent<Ataque>()._idAtaque == _idAtaque)
            {
                _dono._mao.GetComponent<Mao>().StartCoroutine(_dono._mao.GetComponent<Mao>().RecarregarAtaque(_tempoRecargaTotal, habilidade));
            }
        }
        /*var ps = GetComponentInChildren<ParticleSystem>();
        var emission = ps.emission;
        emission.rateOverTime = 0f;*/

        Destroy(gameObject, 3f);
        //ControlarRecarga();
    }

    IEnumerator TransicaoCor(Color corInicial, Color corFinal, float duracao)
    {
        float t = 0f;

        while (t < duracao)
        {
            t += Time.deltaTime;
            Color corAtual = Color.Lerp(corInicial, corFinal, t / duracao);

            foreach (var sr in spritesMembros)
                sr.color = corAtual;

            yield return null;
        }

        // Garante cor final exata
        foreach (var sr in spritesMembros)
            sr.color = corFinal;
    }

    void MagnitudeVisualFuria()
    {
        var ps = GetComponentInChildren<ParticleSystem>();
        var emission = ps.emission;

        float fatorMagnitude = Mathf.Clamp01(
            (float)nivelHabilidade / nivelMaximoMagnitudeVisual
        );

        // Intensidade das partículas
        emission.rateOverTime = Mathf.Lerp(3f, 30f, fatorMagnitude);

        // Cor alvo da fúria baseada na magnitude
        corCorpoEscudo = Color.Lerp(corMinimaCorpoEscudo, corCorpoEscudo, fatorMagnitude);
    }
}
