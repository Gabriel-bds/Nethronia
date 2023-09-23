using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Utilidades : MonoBehaviour
{
    public static float ArredondarNegativo(float _valor)
    {
        if( _valor < 0)
        {
            return 0;
        }
        else
        {
            return _valor;
        }
        
    }
    public static float ArredondarNegativo(int _valor)
    {
        if (_valor < 0)
        {
            return 0;
        }
        else
        {
            return _valor;
        }
    }
    public static float LimitadorNumero(float _min, float _max, float _valor)
    {
        if(_valor > _max)
        {
            return _max;
        }
        else if(_valor < _min)
        {
            return _min;
        }
        else
        {
            return _valor;
        }
    }
    public static void InstanciarNumeroDano(string _dano, Transform _origem)
    {
        Numero_Dano _numeroDano = Resources.Load<Numero_Dano>("Prefabs/Combate/Numero dano");
        Numero_Dano _instanciaNumeroDano =  Instantiate(_numeroDano, _origem.transform.position, Quaternion.Euler(0, 0, 0));
        _instanciaNumeroDano._novoTexto = _dano;
    }
    public static void InstanciarNumeroDano(string _dano, Transform _origem, float _tamanho)
    {
        Numero_Dano _numeroDano = Resources.Load<Numero_Dano>("Prefabs/Combate/Numero dano");
        Numero_Dano _instanciaNumeroDano = Instantiate(_numeroDano, _origem.transform.position, Quaternion.Euler(0, 0, 0));
        _instanciaNumeroDano._novoTexto = _dano;
        _instanciaNumeroDano.GetComponent<TextMeshPro>().fontSize = _tamanho;
    }
    public static void InstanciarNumeroDano(string _dano, Transform _origem, Color32 _cor)
    {
        Numero_Dano _numeroDano = Resources.Load<Numero_Dano>("Prefabs/Combate/Numero dano");
        Numero_Dano _instanciaNumeroDano = Instantiate(_numeroDano, _origem.transform.position, Quaternion.Euler(0, 0, 0));
        _instanciaNumeroDano._novoTexto = _dano;
        _instanciaNumeroDano.GetComponent<TextMeshPro>().color = _cor;
    }
    public static void InstanciarNumeroDano(string _dano, Transform _origem, Color32 _cor, float _tamanho)
    {
        Numero_Dano _numeroDano = Resources.Load<Numero_Dano>("Prefabs/Combate/Numero dano");
        Numero_Dano _instanciaNumeroDano = Instantiate(_numeroDano, _origem.transform.position, Quaternion.Euler(0, 0, 0));
        _instanciaNumeroDano._novoTexto = _dano;
        _instanciaNumeroDano.GetComponent<TextMeshPro>().color = _cor;
        _instanciaNumeroDano.GetComponent<TextMeshPro>().fontSize = _tamanho;
    }
    public static async void AplicarDano(Ser_Vivo _vitima, float _dano, int _duracao, int _intervalo) 
    {
        for(int _vez=0; _vez <_duracao /_intervalo; _vez++)
        {
            _vitima._vidaAtual -= _dano;
            _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
            InstanciarNumeroDano((-_dano).ToString(), _vitima.transform);
            await Task.Delay(_intervalo * 1000);
        }
    }
    public static async void AplicarDano(Ser_Vivo _vitima, float _dano, int _duracao, int _intervalo, Color _corDano)
    {
        for (int _vez = 0; _vez < _duracao / _intervalo; _vez++)
        {
            _vitima._vidaAtual -= _dano;
            _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
            InstanciarNumeroDano((-_dano).ToString(), _vitima.transform, _corDano);
            await Task.Delay(_intervalo * 1000);
        }
    }
}
