using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barra_Vida : MonoBehaviour
{
    [SerializeField] private Image _barraAtual;
    [SerializeField] private Image _barraTransicao;
    private void Awake()
    {
        FindAnyObjectByType<Player>()._barraVida = this;
    }
    private void Start()
    {
        
    }
    public void AtualizarVida(float _vidaMax, float _vidaAtual)
    {
        _barraAtual.fillAmount = _vidaAtual / _vidaMax;
        StartCoroutine(RegredirBarra(_vidaAtual / _vidaMax));
    }
    IEnumerator RegredirBarra(float _novoValor)
    {
        yield return new WaitForSeconds(0.5f);
        float _escalaBarra = _barraTransicao.fillAmount;
        do
        {
            yield return new WaitForSeconds(0.005f);
            _escalaBarra -= Time.deltaTime * 1f;
            _barraTransicao.fillAmount = _escalaBarra;
        }
        while (_barraTransicao.fillAmount > _novoValor);
        _barraTransicao.fillAmount = _novoValor;
    }
}
