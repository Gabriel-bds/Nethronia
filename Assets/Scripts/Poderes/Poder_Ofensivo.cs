using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Tipo_Dano { Físico, Fogo, Veneno, Gelo, Eletricidade}
public abstract class Poder_Ofensivo : Poder
{
    public Tipo_Dano _tipoDano;
    public float _dano;
    public float _negacaoDano;
    public float _repulsao;
    public float _negacaoRepulsao;
}
