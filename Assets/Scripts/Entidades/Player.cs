using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Ser_Vivo
{
    public Barra_Estamina _barraEstamina;
    public Barra_Mana _barraMana;
    public int _pontosHabilidade;
    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
        Atacar();
        _barraEstamina.AtualizarEstamina(_estaminaMax, _estaminaAtual);
        _barraMana.AtualizarMana(_manaMax, _manaAtual);
    }
    private void FixedUpdate()
    {
        Mover(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void Atacar()
    {
        _mao.GetComponent<Animator>().SetInteger("Ataque", 0);
        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            if (_mao.GetComponent<Mao>()._ataques[0].GetComponent<Ataque>()._consumoEstamina <= _estaminaAtual &&
                _mao.GetComponent<Mao>()._ataques[0].GetComponent<Ataque>()._consumoMana <= _manaAtual)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[0].GetComponent<Ataque>()._idAtaque);
            }
        }
        if(Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(0)) 
        {
            if (_mao.GetComponent<Mao>()._ataques[1].GetComponent<Ataque>()._consumoEstamina <= _estaminaAtual &&
                _mao.GetComponent<Mao>()._ataques[1].GetComponent<Ataque>()._consumoMana <= _manaAtual)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[1].GetComponent<Ataque>()._idAtaque);
            }
        }
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            if (_mao.GetComponent<Mao>()._ataques[2].GetComponent<Ataque>()._consumoEstamina <= _estaminaAtual &&
                _mao.GetComponent<Mao>()._ataques[2].GetComponent<Ataque>()._consumoMana <= _manaAtual)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[2].GetComponent<Ataque>()._idAtaque);
            }
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
