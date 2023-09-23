using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Forca : Poder_Ofensivo
{
    public Poder_Forca(float dano, float negacaoDano, float repulsao, float negacaoRepulsao, Tipo_Dano tipoDano)
    {
        _tipoDano = tipoDano;
        _dano = dano;
        _negacaoDano = negacaoDano;
        _repulsao = repulsao;
        _negacaoRepulsao = negacaoRepulsao;
    }
}
