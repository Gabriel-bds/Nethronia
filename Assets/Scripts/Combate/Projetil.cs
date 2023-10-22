using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projetil : Ataque
{
    [SerializeField] float _velocidade;
    [SerializeField] float _tempoDeVida;
    float _tempoDeVidaAtual;
    [SerializeField] LayerMask _colisoes;
    [SerializeField] AudioSource _quebrandoSom;
    private void Awake()
    {
        
    }
    protected override void Start()
    {
        base.Start();
        _tempoDeVidaAtual = _tempoDeVida;
        ControlarParticulas();
    }

    // Update is called once per frame
    void Update()
    {
        Movimentar();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _colisoes) != 0)
        {
            _quebrandoSom.Play();
            IniciarDestruicao();
        }
        base.OnTriggerEnter2D(collision);
    }
    void Movimentar()
    {
        if(_tempoDeVidaAtual  > 0)
        { 
            transform.Translate(Vector3.right * _velocidade * Time.deltaTime);
            _tempoDeVidaAtual -= Time.deltaTime;
        }
        else
        {
            IniciarDestruicao();
        }
        
    }
    void IniciarDestruicao()
    {
        _velocidade = 0;
        if(GetComponent<Animator>() != null)
        {
            Animator _animator = GetComponent<Animator>();
            _animator.SetBool("Destruir", true);
        }
        if(GetComponent<ParticleSystem>() != null)
        {
            ParticleSystem _particula = GetComponent<ParticleSystem>();
            _particula.Stop();
        }
        if(GetComponentInChildren<ParticleSystem>() != null)
        {
            ParticleSystem[] _particulas = GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem _particula in _particulas)
            {
                _particula.Stop();
            }
            AutoDestruir();
        }
    }
    public void AutoDestruir()
    {
        Destroy(gameObject);
    }
    void ControlarParticulas()
    {
        float _minParticulas = 0.3f;
        if (GetComponent<ParticleSystem>() != null)
        {
            var _emissao = GetComponent<ParticleSystem>().emission;
            switch (_tipoDano)
            {
                case Tipo_Dano.Fogo:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderFogo._nivel / 100));
                    break;

                case Tipo_Dano.Gelo:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderGelo._nivel / 100));
                    break;

                case Tipo_Dano.Veneno:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderVeneno._nivel / 100));
                    break;

                case Tipo_Dano.Eletricidade:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderEletricidade._nivel / 100));
                    break;
            }
        }
        else
        {
            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                ParticleSystem[] _particulas = GetComponentsInChildren<ParticleSystem>();
                foreach(ParticleSystem _particula in _particulas)
                {
                    var _emissao = _particula.emission;
                    switch (_tipoDano)
                    {
                        case Tipo_Dano.Fogo:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderFogo._nivel / 100));
                            break;

                        case Tipo_Dano.Gelo:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderGelo._nivel / 100));
                            break;

                        case Tipo_Dano.Veneno:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderVeneno._nivel / 100));
                            break;

                        case Tipo_Dano.Eletricidade:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderEletricidade._nivel / 100));
                            break;
                    }
                }
            }
        }
    }

}
