using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Forca : Poder_Ofensivo
{
    public Poder_Forca(Tipo_Dano _tipoDano)
    {
        this._tipoDano = _tipoDano;
    }
}
