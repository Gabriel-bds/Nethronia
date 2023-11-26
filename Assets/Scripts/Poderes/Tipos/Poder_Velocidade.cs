using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Velocidade : Poder
{
    [Tooltip("Valor que será acrescentado à velocidade de movimento")]
    public float _acrescimoVelocidadeMovimento;
    [Tooltip("Valor que será acrescentado à velocidade das animações")]
    public float _acrescimoVelocidadeAnimações;
    [Tooltip("Quando o tempo desacelerar, quanto será reduzido da velocidade padrão dele")]
    public float _reducaoTempo;
}
