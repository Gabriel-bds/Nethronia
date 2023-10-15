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
        List<GameObject> _ataques = FindAnyObjectByType<Player>()._mao.GetComponent<Mao>()._ataques;
        foreach (GameObject o in _ataques)
        {
            if (o.GetComponent<Ataque>()._numeroQuadro == _numeroQuadro)
            {
                _quadroHabilidade.sprite = o.GetComponent<Ataque>()._icone;
                _quadroHabilidade.color = Color.white;
            }
        }
        if (_quadroHabilidade.sprite == null)
        {
            Destroy(_quadroHabilidade.gameObject);
        }
    }
    public async void CarregarHabilidade(float _tempoAtual, float _tempoRecarga)
    {
        for (float t = _tempoAtual; t > 0; t -= Time.deltaTime)
        {
            _quadroNegro.fillAmount = (t - Time.deltaTime) / _tempoRecarga;
            await Task.Delay(1);
        }
    }
}
