using NavMeshPlus.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Inimigo : Ser_Vivo
{
    private NavMeshAgent _agent;
    [Range(0f, 100f)]
    public float _taxaDropPontoHabilidadePermanente;
    public int _minimoDropPontoHabilidadePermanente;
    public int _maximoDropPontoHabilidadePermanente;
    protected override void Awake()
    {
        base.Awake();
        _alvo = FindAnyObjectByType<Player>().gameObject;
    }
    protected override void Start()
    {
        if (_barraVida == null)
        {
            try
            {
                _barraVida = GetComponentInChildren<Barra_Vida>();
            }
            finally
            {
                _barraVida = GameObject.FindGameObjectWithTag("Barra_vida_Boss").GetComponent<Barra_Vida>();
            }
        }
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        DefinirAtributos();
        _mao.GetComponent<Mao>().RecarregarTodosAtaques();
    }
    protected override void Update()
    {
        base.Update();
        Andar();
        AtacarPlayer();
        
    }
    public void TravarAgent(bool _travar)
    {
        if(_travar)
        {
            _agent.enabled = false;
        }
        else
        {
            _agent.enabled = true;
            _agent.SetDestination(_alvo.transform.position);
        }
    }
    void Andar()
    {
        _agent.SetDestination(_alvo.transform.position);
    }
    void AtacarPlayer()
    {
        _mao.GetComponent<Animator>().SetInteger("Ataque", 0);
        if (_mao.GetComponent<Mao>()._mirandoAlvo)
        {
            int _numeroAtq = SortearAtaque(
                (float)Mathf.Sqrt(
                    (float)Mathf.Pow(transform.position.x - FindAnyObjectByType<Player>().transform.position.x, 2) +
                    (float)Mathf.Pow(transform.position.y - FindAnyObjectByType<Player>().transform.position.y, 2)
                    )); ;
            //_mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[_numeroAtq].GetComponent<Ataque>()._idAtaque);
            var ataqueEscolhido = _mao.GetComponent<Mao>()._ataques[_numeroAtq];

            if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Contains(ataqueEscolhido))
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque",
                    ataqueEscolhido.GetComponent<Ataque>()._idAtaque);
            }

        }
    }

    public int SortearAtaque(float _distanciAlvo)
    {
        //o int representa o indice do ataque na lista de ataques da mão, o float é a chance dele de ser executado em percentual
        var _ataque = new List<(int, float)> { };
        int _index = 0;
        foreach(var _atq in _mao.GetComponent<Mao>()._ataques)
        {
            float _distanciaMin = _atq.GetComponent<Ataque>()._distanciaMin;
            float _distanciaMax = _atq.GetComponent<Ataque>()._distanciaMax;
            float _distanciaPerfeita = _atq.GetComponent<Ataque>()._distanciaPerfeita;

            if (_distanciAlvo <= _distanciaPerfeita)
            {
                _ataque.Add((_index, Utilidades.LimitadorNumero(0, 100, (_distanciaPerfeita - _distanciaMin + 1 - (_distanciaPerfeita - _distanciAlvo)) * 100 / (_distanciaPerfeita - _distanciaMin + 1))));
            }
            else
            {
                _ataque.Add((_index, Utilidades.LimitadorNumero(0, 100, (_distanciaMax - _distanciaPerfeita + 1 - (_distanciAlvo - _distanciaPerfeita)) * 100 / (_distanciaMax - _distanciaPerfeita + 1))));
            }
            _index++;
        }

        float _totalPercentual = 0;
        foreach(var _atq in _ataque)
        {
            _totalPercentual += _atq.Item2;
        }
        for(int index = 0;  index < _ataque.Count; index++)
        {
            _ataque[index] = (_ataque[index].Item1, _ataque[index].Item2 * 100 / _totalPercentual);
            //Debug.Log("Ataque " + _ataque[index].Item1 + "tem " + _ataque[index].Item2 + "de chance de acontecer");
        }

        float _sorteio = UnityEngine.Random.Range(0f, 100f);
        float _acumulo = 0;
        int _retorno = -1;
        foreach (var _atq in _ataque)
        {
            _acumulo += _atq.Item2;
            if(_sorteio < _acumulo)
            {
                _retorno = _atq.Item1;
                break;
            }
        }
        return _retorno;
    }

    public override void Morte(float _tempo)
    {
        GetComponent<NavMeshAgent>().enabled = false;
        Player _player = FindAnyObjectByType<Player>();
        _mao.GetComponent<Mao>().ResetarMao();
        _mao.GetComponent<Mao>().TravarMao(1);
        Utilidades.AplicarDano(_player, -_player._poderVitalidade._rouboVida, Color.green);
        _player.AdicionarExperiencia(_experiencia, UnityEngine.Random.Range(_minimoDropPontoHabilidadePermanente, _maximoDropPontoHabilidadePermanente +1));
        base.Morte(_tempo);
    }
}
