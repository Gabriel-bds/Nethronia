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

    private void Start()
    {
        AtualizarQuadro();
    }
    public async void CarregarHabilidade(float _tempoAtual, float _tempoRecarga)
    {
        for (float t = _tempoAtual; t > 0; t -= Time.deltaTime)
        {
            _quadroNegro.fillAmount = (t - Time.deltaTime) / _tempoRecarga;
            await Task.Delay(1);
        }
    }
    public void AtualizarQuadro()
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
