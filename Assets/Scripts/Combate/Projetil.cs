using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Projetil : Ataque
{
    [SerializeField] float _velocidade;
    [SerializeField] float _tempoDeVida;
    float _tempoDeVidaAtual;
    [SerializeField] LayerMask _colisoes;
    [SerializeField] AudioSource _quebrandoSom;
    protected override void Start()
    {
        base.Start();
        _tempoDeVidaAtual = _tempoDeVida;
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

}
