using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Velocidade : Poder
{
    [Tooltip("Valor que ser� acrescentado � velocidade de movimento")]
    public float _acrescimoVelocidadeMovimento;
    [Tooltip("Valor que ser� acrescentado � velocidade das anima��es")]
    public float _acrescimoVelocidadeAnima��es;
    [Tooltip("Quando o tempo desacelerar, quanto ser� reduzido da velocidade padr�o dele")]
    public float _reducaoTempo;
}
