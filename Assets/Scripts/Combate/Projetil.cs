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
    [SerializeField] bool _progressaoEscala;
    [SerializeField] LayerMask _colisoes;
    [SerializeField] AudioSource _quebrandoSom;
    protected override void Start()
    {
        if (_progressaoEscala)
        {
            switch (_tipoDano)
            {
                case Tipo_Dano.Físico:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderForca._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderForca._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Fogo:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderFogo._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderFogo._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Gelo:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderGelo._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderGelo._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Veneno:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderVeneno._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderVeneno._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Eletricidade:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderEletricidade._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderEletricidade._nivel, 1, 0.033f)));
                    break;
            }
        }
        base.Start();
        _tempoDeVidaAtual = _tempoDeVida;
    }
    void Update()
    {
        Movimentar();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _colisoes) != 0)
        {
            try
            { 
                _quebrandoSom.Play();
            }
            catch 
            {
                Debug.Log("Projetil sem som de hit anexado");
            }
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
    public void AutoDestruir(float _tempo)
    {
        Destroy(gameObject, _tempo);
    }

}
