using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Eletricidade : Poder_Efeito
{
    public Poder_Eletricidade(Tipo_Dano _tipoDano)
    {
        this._tipoDano = _tipoDano;
    }
}
