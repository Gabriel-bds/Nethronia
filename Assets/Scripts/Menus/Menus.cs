using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menus : MonoBehaviour
{
    [SerializeField] GameObject _menuPersonagem;
    float _tempoAtual;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(_menuPersonagem.activeInHierarchy) 
            {
                PausarJogo(false);
                _menuPersonagem.SetActive(false);
            }
            else
            {
                PausarJogo(true);
                _menuPersonagem.SetActive(true);
            }
        }
    }

    void PausarJogo(bool _pausado)
    {
        if(_pausado) 
        {
            _tempoAtual = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = _tempoAtual;
        }
    }
}
