using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class Status
{
    public float _acumuloMax;
    [HideInInspector] public float _acumuloAtual;
    public float _infligirAcumulo;
    public float _dano;
    public float _negacaoDano;
    public float _dano2;
    public float _negacaoDano2;
    public float _utilidade1;
    public float _negacaoUtilidade1;
    public float _utilidade2;
    public float _negacaoUtilidade2;
}
public class Efeito : MonoBehaviour{
    protected string _mensagemAviso;
    protected Color _cor;
    protected GameObject _particulaEfeito;
    protected GameObject _particulaExplosao;
    public void Avisar(Ser_Vivo _vitima)
    {
        Utilidades.InstanciarNumeroDano(_mensagemAviso, _vitima.transform, _cor, 10);
    }
    public virtual void Aplicar(Ser_Vivo _atacante, Ser_Vivo _vitima)
    {
        Avisar(_vitima);
    }
}
