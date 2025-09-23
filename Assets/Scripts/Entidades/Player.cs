using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Player : Ser_Vivo
{
    public Barra_Estamina _barraEstamina;
    public Barra_Mana _barraMana;
    public int _pontosHabilidade;
    public int _pontosHabilidadePermanentes;
    protected override void Awake()
    { 
        
        FindAnyObjectByType<CinemachineVirtualCamera>().Follow = gameObject.transform;
        base.Awake();
        DefinirAtributos(); 
        
    }
    protected override void Start()
    {
        _barraVida = GameObject.FindGameObjectWithTag("Hud").GetComponentInChildren<Barra_Vida>();
        _barraEstamina = FindAnyObjectByType<Barra_Estamina>();
        _barraMana = FindAnyObjectByType<Barra_Mana>();
        base.Start();
        _mao.GetComponent<Mao>().RecarregarTodosAtaques();
        //SalvarDadosPrefab("Prefabs/Entidades/Player");
    }
    protected override void Update()
    {
        base.Update();
        Atacar();
        //Esquivar();
        /*if(Input.GetMouseButtonDown(0)) 
        {
            _animator.SetInteger("Ataque", 1);
            Debug.Log("Clicou");
            //Desmembrar(7, 5);
        }
        else
        {
            _animator.SetInteger("Ataque", 0);
        }*/
        _barraVida.AtualizarVida(_vidaMax, _vidaAtual);
    }
    private void FixedUpdate()
    {
        Mover(GetComponent<PlayerInput>().actions["Movimento"].ReadValue<Vector2>());
        //Debug.Log(GetComponent<PlayerInput>().actions["Movimento"].ReadValue<Vector2>());
    }
    void Atacar()
    {
        float _valorAtaqueAtual = GetComponent<PlayerInput>().actions["Ataques"].ReadValue<float>();
        float _velocidadeAnimator = 1;

        if (_animator.speed > 0)
        {
            //_velocidadeAnimator = _mao.GetComponent<Animator>().speed;
            _velocidadeAnimator = _animator.speed;
        }
        if (_valorAtaqueAtual == 0)
        {
            /*_mao.GetComponent<Animator>().SetInteger("Ataque", 0);
            _mao.GetComponent<Animator>().speed = _velocidadeAnimator;*/
            _animator.SetInteger("Ataque", 0);
            _animator.speed = _velocidadeAnimator;

        }
        else
        {
            if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Contains(_mao.GetComponent<Mao>()._ataques[(int)_valorAtaqueAtual - 1]))
            {
                //_mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[(int)_valorAtaqueAtual - 1].GetComponent<Ataque>()._idAtaque);
                _animator.SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[(int)_valorAtaqueAtual - 1].GetComponent<Ataque>()._idAtaque);
            }
            else
            {
                /*_mao.GetComponent<Animator>().SetInteger("Ataque", 0);
                _mao.GetComponent<Animator>().speed = _velocidadeAnimator;*/
                _animator.SetInteger("Ataque", 0);
                _animator.speed = _velocidadeAnimator;
            }
        }
    }
    void Esquivar()
    {
        _animator.SetBool("Esquivar", false);
        _mao.GetComponent<Animator>().SetBool("Esquivar", false);

        if (Input.GetButtonDown("Esquiva"))
        {
            Knockback(50f, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            _animator.SetBool("Esquivar", true);
            _mao.GetComponent<Animator>().SetBool("Esquivar", true);
        }
    }
    public void AdicionarExperiencia(int _xp, int _ptPermanente)
    {
        _experiencia += _xp;
        if (_experiencia >= _experienciaParaProximoNivel)
        {
            _experiencia -= _experienciaParaProximoNivel;
            _experienciaParaProximoNivel = (int)Math.Round(1.5f * _experienciaParaProximoNivel);
            GetComponent<Player>()._pontosHabilidade += 1;
            FindAnyObjectByType<Numero_BarraXP>().AtualizarNumero();
        }
        _pontosHabilidadePermanentes += _ptPermanente;
        Utilidades.InstanciarNumeroDano($"+{_xp}Exp", transform);
    }
    public void CongelarTempo()
    {
        CinemachineVirtualCamera _camera = FindObjectOfType<CinemachineVirtualCamera>();
        if(Time.timeScale == 1)
        {
            Time.timeScale -= _poderVelocidade._reducaoTempo;
            Time.fixedDeltaTime *= Time.timeScale; 
            _velocidadeMovimento = _velocidadeMovimento / (Time.timeScale / 0.5f) /  (Time.timeScale / 0.5f * 2);
            _animator.speed = 1 / Time.timeScale;
            _mao.GetComponent<Animator>().speed = 1 / Time.timeScale;
            _mao.GetComponent<Mao>()._latenciaMira /= Time.timeScale; 
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime *= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping *=  Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping *= Time.timeScale;
        }
        else
        {
            Time.fixedDeltaTime /= Time.timeScale;
            _mao.GetComponent<Mao>()._latenciaMira *= Time.timeScale;
            _velocidadeMovimento = 10 + _poderVelocidade._acrescimoVelocidadeMovimento;
            GetComponent<Animator>().speed = 1 + _poderVelocidade._acrescimoVelocidadeAnimações;
            _mao.GetComponent<Animator>().speed = 1 + _poderVelocidade._acrescimoVelocidadeAnimações;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime /= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping /= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping /= Time.timeScale;
            Time.timeScale = 1;
        }
    }
}