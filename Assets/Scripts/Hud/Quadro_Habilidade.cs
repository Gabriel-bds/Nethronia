using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Quadro_Habilidade : MonoBehaviour
{
    public int _numeroQuadro;
    [SerializeField] Image _quadroHabilidade;
    [SerializeField] Image _quadroNegro;
    public float _recargaMaxima;
    public float _recargaAtual;
    Ataque _atq;
    bool _jaAdicionouAtaque;

    private void Start()
    {
        DefinirIconeQuadro();
        ConectarHabilidadeComQuadro();
        _recargaMaxima = _atq._tempoRecargaTotal;
    }
    private void Update()
    {
        CarregarHabilidade();
        AtualizarQuadro();
    }

    void ConectarHabilidadeComQuadro()
    {
        _atq = FindAnyObjectByType<Player>()._mao.GetComponent<Mao>()._ataques[_numeroQuadro].GetComponent<Ataque>();
    }
    public void CarregarHabilidade()
    {
        if(_recargaAtual <= _recargaMaxima)
        {
            _recargaAtual += Time.deltaTime;
            _jaAdicionouAtaque = false;
        }
        else if (_recargaAtual < 0)
        {
            _recargaAtual = 0;
        }
        else if(!_jaAdicionouAtaque)
        {
            FindAnyObjectByType<Player>()._mao.GetComponent<Mao>()._ataquesDisponiveis.Add(_atq.gameObject);
            _jaAdicionouAtaque = true;
        }
    }
    void AtualizarQuadro()
    {
        _quadroNegro.fillAmount = 1 - _recargaAtual / _recargaMaxima;
    }
    public void DefinirIconeQuadro()
    {
        _quadroHabilidade.sprite = FindAnyObjectByType<Player>()._mao.GetComponent<Mao>()._ataques[_numeroQuadro].GetComponent<Ataque>()._icone;
        if(_quadroHabilidade.sprite != null)
        {
            _quadroHabilidade.color = Color.white;
        }
        else
        {
            _quadroHabilidade.color = new Color32(255, 255, 255, 0);
        }
    }
}
