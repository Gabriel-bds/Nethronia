using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Poder_Velocidade : Poder
{
    [Tooltip("Valor que sera acrescentado a velocidade de movimento")]
    public float _acrescimoVelocidadeMovimento;
    [Tooltip("Valor que sera acrescentado a velocidade das animacoes")]
    public float _acrescimoVelocidadeAnimacoes;
    [Tooltip("Quando o tempo desacelerar, quanto sera reduzido da velocidade padrao dele")]
    public float _reducaoTempo;
}
