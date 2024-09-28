using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

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
    public static float NegativoParaPositivo(float _valor)
    {
        if(_valor < 0)
        {
            return _valor * -1;
        }
        else 
        {   
            return _valor;
        }
    }
    public static float Escala(int _nivel, float _valorInicial, float _progressao)
    {
        return _valorInicial + _nivel * _progressao;
    }
    public static int Escala(int _nivel, int _valorInicial, int _progressao)
    {
        return _valorInicial + _nivel / _progressao;
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
    public static int LimitadorNumero(int _min, int _max, int _valor)
    {
        if (_valor > _max)
        {
            return _max;
        }
        else if (_valor < _min)
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
    public static void AplicarDano(Ser_Vivo _vitima, float _dano)
    {
        _vitima._vidaAtual -= _dano;
        _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
        InstanciarNumeroDano((-_dano).ToString(), _vitima.transform);

        ParticleSystem _objSangue = Instantiate(_vitima._sangue, _vitima.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
        var _emissao = _objSangue.emission;
        _emissao.rateOverTime = _dano * 100 / _vitima._vidaMax / 100 * _emissao.rateOverTime.constant;

    }
    public static void AplicarDano(Ser_Vivo _vitima, float _dano, Color _corNumeroDano)
    {
        _vitima._vidaAtual -= _dano;
        _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
        InstanciarNumeroDano((-_dano).ToString(), _vitima.transform, _corNumeroDano);

        ParticleSystem _objSangue = Instantiate(_vitima._sangue, _vitima.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
        var _emissao = _objSangue.emission;
        _emissao.rateOverTime = _dano * 100 / _vitima._vidaMax / 100 * _emissao.rateOverTime.constant;

    }
    public static async void AplicarDano(Ser_Vivo _vitima, float _dano, int _duracao, int _intervalo) 
    {
        for(int _vez=0; _vez <_duracao /_intervalo; _vez++)
        {
            _vitima._vidaAtual -= _dano;
            _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
            InstanciarNumeroDano((-_dano).ToString(), _vitima.transform);

            ParticleSystem _objSangue = Instantiate(_vitima._sangue, _vitima.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _dano * 100 / _vitima._vidaMax / 100 * _emissao.rateOverTime.constant;

            await Task.Delay(_intervalo * 1000);


        }
    }
    public static async void AplicarDano(Ser_Vivo _vitima, float _dano, float _duracao, float _intervalo)
    {
        for (int _vez = 0; _vez < _duracao / _intervalo; _vez++)
        {
            _vitima._vidaAtual -= _dano;
            _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
            InstanciarNumeroDano((-_dano).ToString(), _vitima.transform);

            ParticleSystem _objSangue = Instantiate(_vitima._sangue, _vitima.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _dano * 100 / _vitima._vidaMax / 100 * _emissao.rateOverTime.constant;
            
            await Task.Delay((int)Math.Ceiling(_intervalo * 1000));
        }
    }
    public static async void AplicarDano(Ser_Vivo _vitima, float _dano, int _duracao, int _intervalo, Color _corDano)
    {
        for (int _vez = 0; _vez < _duracao / _intervalo; _vez++)
        {
            _vitima._vidaAtual -= _dano;
            _vitima._barraVida.AtualizarVida(_vitima._vidaMax, _vitima._vidaAtual);
            InstanciarNumeroDano((-_dano).ToString(), _vitima.transform, _corDano);

            ParticleSystem _objSangue = Instantiate(_vitima._sangue, _vitima.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _dano * 100 / _vitima._vidaMax / 100 * _emissao.rateOverTime.constant;

            await Task.Delay(_intervalo * 1000);
        }
    }
    public static void CopiarPropriedades(object _origem, object _destino)
    {
        Type type = _origem.GetType();
        FieldInfo[] fields = type.GetFields();

        // Copia os valores dos campos
        foreach (var field in fields)
        {
            field.SetValue(_destino, field.GetValue(_origem));
        }
    }
    public static List<object> CopiarListaDe(List<object> lista)
    {
        List<object> copia = new List<object>();
        foreach (object obj in lista)
        {
            copia.Add(obj);
        }
        return copia;
    }
}
