using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum Tipo_Poder {Forca, Resistencia, Vitalidade, Velocidade, Fogo, Gelo, Veneno, Eletricidade}
public abstract class Poder
{
    public int _nivel;
}
