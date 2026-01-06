using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furia : Ataque
{
    [Header("Furia:")]
    [SerializeField] int nivelHabilidade;
    [SerializeField] float aumentoPercentualAtributos;
    [SerializeField] float tempoFuria;
    [SerializeField] float aumentoPercentualAtributosPorNivel;
    [SerializeField] float aumentoTempoFuriaPorNivel;
    //[SerializeField] int nivelForcaMaximoParaAumento;
    [SerializeField] float aumentoMinimoPercentualAtributos;
    [SerializeField] float tempoMinimoFuria;
    [SerializeField] float tempoTransicaoCor = 0.25f;
    [SerializeField] Color32 corCorpoFurioso;

    List<SpriteRenderer> spritesMembros = new();
    Color corPadrao;

    // Start is called before the first frame update
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
        DefinirNivelFuria();
        StartCoroutine(FuriaCoroutine());
        _dono._mao.GetComponent<Mao>().RemoverAtaqueDisponivel(gameObject);
        //ControlarRecarga();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DefinirNivelFuria() 
    { 
        aumentoPercentualAtributos = aumentoMinimoPercentualAtributos + aumentoPercentualAtributosPorNivel * nivelHabilidade;
        tempoFuria = tempoMinimoFuria + aumentoTempoFuriaPorNivel * nivelHabilidade;
    }
    IEnumerator AumentarAtributos()
    {
        float danoPadrao = _dono._poderForca._dano;
        float repulsaoPadrao = _dono._poderForca._repulsao;

        _dono._poderForca._dano += _dono._poderForca._dano * aumentoPercentualAtributos / 100f;
        _dono._poderForca._repulsao += _dono._poderForca._repulsao * aumentoPercentualAtributos / 100f;
        yield return new WaitForSeconds(tempoFuria);
        Debug.Log("Final furia");
        _dono._poderForca._dano = danoPadrao;
        _dono._poderForca._repulsao = repulsaoPadrao;
    }

    IEnumerator FuriaCoroutine()
    {
        // Transição para a cor da fúria
        yield return StartCoroutine(TransicaoCor(corPadrao, corCorpoFurioso, tempoTransicaoCor));

        // Buffa atributos
        yield return StartCoroutine(AumentarAtributos());

        // Transição de volta para a cor normal
        yield return StartCoroutine(TransicaoCor(corCorpoFurioso, corPadrao, tempoTransicaoCor));

        foreach(GameObject habilidade in _dono._mao.GetComponent<Mao>()._ataques)
        {
            if(habilidade.GetComponent<Ataque>()._idAtaque == _idAtaque)
            {
                _dono._mao.GetComponent<Mao>().StartCoroutine(_dono._mao.GetComponent<Mao>().RecarregarAtaque(_tempoRecargaTotal, habilidade));
            }
        }
        

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


}
