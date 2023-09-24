using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Veneno : Poder_Efeito
{
    public Poder_Veneno(Tipo_Dano _tipoDano)
    {
        this._tipoDano = _tipoDano;
    }
}
