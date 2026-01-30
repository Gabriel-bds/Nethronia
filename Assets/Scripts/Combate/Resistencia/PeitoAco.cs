using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeitoAco : Ataque
{
    [Header("Escudo:")]
    public bool ativo;
    [SerializeField] int nivelHabilidade;
    [SerializeField] float tempoPeitoAco;
    [SerializeField] float negacaoDano;
    [SerializeField] float aumentonegacaoDanoPorNivel;
    [SerializeField] float aumentoTempoPeitoAcoPorNivel;
    [SerializeField] float negacaoDanoMinima;
    [SerializeField] float tempoMinimoPeitoAco;
    [SerializeField] float tempoTransicaoCor = 0.25f;
    public Color corCorpoPeitoAco;
    public Color corMinimaCorpoPeitoAco;

    List<SpriteRenderer> spritesMembros = new();
    Color corPadrao;
    // Start is called before the first frame update
    void Start()
    {
        ativo = true;
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
        DefinirNivelPeitoAco();
        StartCoroutine(PeitoAcoVisual());
        _dono._mao.GetComponent<Mao>().RemoverAtaqueDisponivel(gameObject);
        //ControlarRecarga();
    }

    IEnumerator PeitoAcoVisual()
    {
        // Transição para a cor do escudo
        yield return StartCoroutine(TransicaoCor(corPadrao, corCorpoPeitoAco, tempoTransicaoCor));

        // Buffa atributos
        yield return new WaitForSeconds(tempoPeitoAco);

        // Transição de volta para a cor normal
        yield return StartCoroutine(TransicaoCor(corCorpoPeitoAco, corPadrao, tempoTransicaoCor));

        foreach (GameObject habilidade in _dono._mao.GetComponent<Mao>()._ataques)
        {
            if (habilidade.GetComponent<Ataque>()._idAtaque == _idAtaque)
            {
                _dono._mao.GetComponent<Mao>().StartCoroutine(_dono._mao.GetComponent<Mao>().RecarregarAtaque(_tempoRecargaTotal, habilidade));
            }
        }
        ativo = false;
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
    void DefinirNivelPeitoAco()
    {
        negacaoDano = negacaoDanoMinima + aumentonegacaoDanoPorNivel * nivelHabilidade;
        tempoPeitoAco = tempoMinimoPeitoAco + aumentoTempoPeitoAcoPorNivel * nivelHabilidade;
    }

    public float CalcularNegacao(float danoRecebido)
    {
        return Mathf.Min(
            danoRecebido,
            danoRecebido * negacaoDano / 100f
        );
    }
}
