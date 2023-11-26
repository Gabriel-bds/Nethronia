using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barra_Estamina : MonoBehaviour
{
    [SerializeField] private Image _barraAtual;
    [SerializeField] private Image _barraTransicao;
    public void AtualizarEstamina(float _estaminaMax, float _estaminaAtual)
    {
        _barraAtual.fillAmount = _estaminaAtual / _estaminaMax;
        StartCoroutine(RegredirBarra(_estaminaAtual / _estaminaMax));
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
