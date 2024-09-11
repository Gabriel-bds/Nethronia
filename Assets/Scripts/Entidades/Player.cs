using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Player : Ser_Vivo
{
    public Barra_Estamina _barraEstamina;
    public Barra_Mana _barraMana;
    public int _pontosHabilidade;
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
        SalvarDadosPrefab("Prefabs/Entidades/Player");
    }
    protected override void Update()
    {
        base.Update();
        Atacar();
        Esquivar();
        _barraEstamina.AtualizarEstamina(_estaminaMax, _estaminaAtual);
        _barraMana.AtualizarMana(_manaMax, _manaAtual);
        _barraVida.AtualizarVida(_vidaMax, _vidaAtual);
        /*Debug.Log(Input.GetAxis("Horizontal"));
        Debug.Log(Input.GetAxis("Vertical"));*/
    }
    private void FixedUpdate()
    {
        Mover(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    void Atacar()
    {
        float _velocidadeMaoAnimator = 1;
        if(_mao.GetComponent<Animator>().speed > 0)
        {
            _velocidadeMaoAnimator = _mao.GetComponent<Animator>().speed;
        }
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            if (_mao.GetComponent<Mao>()._ataques[0].GetComponent<Ataque>()._consumoEstamina <= _estaminaAtual &&
                _mao.GetComponent<Mao>()._ataques[0].GetComponent<Ataque>()._consumoMana <= _manaAtual)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[0].GetComponent<Ataque>()._idAtaque);
            }
            else
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", 0);
                _mao.GetComponent<Animator>().speed = _velocidadeMaoAnimator;
            }
        }
        if(Input.GetMouseButton(1) && !Input.GetMouseButton(0)) 
        {
            if (_mao.GetComponent<Mao>()._ataques[1].GetComponent<Ataque>()._consumoEstamina <= _estaminaAtual &&
                _mao.GetComponent<Mao>()._ataques[1].GetComponent<Ataque>()._consumoMana <= _manaAtual)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[1].GetComponent<Ataque>()._idAtaque);
            }
        }
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            if (_mao.GetComponent<Mao>()._ataques[2].GetComponent<Ataque>()._consumoEstamina <= _estaminaAtual &&
                _mao.GetComponent<Mao>()._ataques[2].GetComponent<Ataque>()._consumoMana <= _manaAtual)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[2].GetComponent<Ataque>()._idAtaque);
            }
        }
        if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            _mao.GetComponent<Animator>().SetInteger("Ataque", 0);
            _mao.GetComponent<Animator>().speed = _velocidadeMaoAnimator;
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