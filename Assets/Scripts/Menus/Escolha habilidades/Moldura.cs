using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Moldura : MonoBehaviour
{
    public GameObject _habilidade;
    [HideInInspector] public int _numeroMoldura;
    void Start()
    {
        _numeroMoldura = GetComponentInParent<Grid>()._molduraChamada;
    }
    public void AtualizarPoder()
    {
        FindObjectOfType<Player>()._mao.GetComponent<Mao>().AtualizarAtaque(_numeroMoldura, _habilidade);
        foreach (Quadro_Habilidade _quadro in FindObjectsOfType<Quadro_Habilidade>())
        {
            if(_quadro._numeroQuadro == _numeroMoldura)
            {
                _quadro.AtualizarQuadro();
            }
        }
    }
}
