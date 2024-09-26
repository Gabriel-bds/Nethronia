using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public enum Raridade_carta { Comum, Raro, Epico, Lendario}
public class Carta : MonoBehaviour
{
    public Raridade_carta _raridade = new Raridade_carta();

    public string _atributos;
    public float _taxaAlteracao;
    public void AlteracaoAtributos()
    {
        Ser_Vivo _player = FindAnyObjectByType<Player>();
        Type _tipoVariaveis = _player.GetType();

        string[] _campos = _atributos.Split('.');
        object _instanciaSecundaria = _player;

        for (int index = 0;  index < _campos.Length; index++)
        {
            FieldInfo _informacao = _instanciaSecundaria.GetType().GetField(_campos[index]);
            if(index + 1 == _campos.Length)
            {
                _informacao.SetValue(_instanciaSecundaria, (float)_informacao.GetValue(_instanciaSecundaria) * _taxaAlteracao);
                break;
            }
            _instanciaSecundaria = _informacao.GetValue(_instanciaSecundaria);

        }
        Controle_Cartas.DestruirCartas();
    }
}
