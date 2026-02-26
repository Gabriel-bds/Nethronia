using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barra_Vida : MonoBehaviour
{
    [SerializeField] private Image _barraAtual;
    [SerializeField] private Image _barraTransicao;
    [SerializeField] private Ser_Vivo _dono;
    private void Awake()
    {
        //FindAnyObjectByType<Player>()._barraVida = this;
        if(_dono == null)
        {
            _dono = FindAnyObjectByType<Player>();
        }
        _dono.OnVidaAlterada += AtualizarVida;
    }
    private void AtualizarVida(Ser_Vivo dono, float _vidaAtual, float _vidaMax)
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
