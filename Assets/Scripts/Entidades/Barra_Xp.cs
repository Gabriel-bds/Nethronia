using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barra_Xp : MonoBehaviour
{
    [SerializeField] private Image _barraAtual;
    Player _player;
    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        
    }
    public void AtualizarBarra()
    {
        StartCoroutine(MovimentarBarra((float)_player._experiencia / _player._experienciaParaProximoNivel));
    }
    IEnumerator MovimentarBarra(float _novoValor)
    {
        yield return new WaitForSeconds(0.5f);
        float _escalaBarra = _barraAtual.fillAmount;
        do
        {
            yield return new WaitForSeconds(0.005f);
            _escalaBarra += Time.deltaTime * 1f;
            _barraAtual.fillAmount = _escalaBarra;
        }
        while (_barraAtual.fillAmount < _novoValor);
        _barraAtual.fillAmount = _novoValor;
    }
}
