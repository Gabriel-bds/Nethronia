using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barra_Mana : MonoBehaviour
{
    [SerializeField] private Image _barraAtual;
    [SerializeField] private Image _barraTransicao;
    private void Awake()
    {
        FindAnyObjectByType<Player>()._barraMana = this;
    }
    public void AtualizarMana(float _manaMax, float _manaAtual)
    {
        _barraAtual.fillAmount = _manaAtual / _manaMax;
        StartCoroutine(RegredirBarra(_manaAtual / _manaMax));
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
