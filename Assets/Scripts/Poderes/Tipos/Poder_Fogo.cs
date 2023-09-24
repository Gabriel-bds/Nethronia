using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Poder_Fogo : Poder_Efeito
{
    public Poder_Fogo(Tipo_Dano _tipoDano)
    {
        this._tipoDano = _tipoDano;
    }
}
