using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Quadro_Habilidade : MonoBehaviour
{
    public int _numeroQuadro;
    [SerializeField] Image _quadroNegro;
    public async void CarregarHabilidade(float _tempoAtual, float _tempoRecarga)
    {
        for (float t = _tempoAtual; t > 0; t -= Time.deltaTime)
        {
            _quadroNegro.fillAmount = (t - Time.deltaTime) / _tempoRecarga;
            await Task.Delay(1);
        }
    }
}
