using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public enum Tipo_Dano { Físico, Fogo, Veneno, Gelo, Eletricidade}
public class Poder_Ofensivo : Poder
{
    public Tipo_Dano _tipoDano;
    public float _dano;
    [ReadOnly(true)] public float _negacaoDano;
    [ReadOnly(true)] public float _repulsao;
    [ReadOnly(true)] public float _negacaoRepulsao;
}
