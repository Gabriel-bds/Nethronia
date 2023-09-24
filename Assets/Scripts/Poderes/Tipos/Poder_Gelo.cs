using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Gelo : Poder_Efeito
{
    public Poder_Gelo(Tipo_Dano _tipoDano)
    {
        this._tipoDano = _tipoDano;
    }
}
