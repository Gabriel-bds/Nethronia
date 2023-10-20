using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Ser_Vivo
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
        Atacar();
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
            Ataque _atq = new Ataque();
            List<GameObject> _ataques = _mao.GetComponent<Mao>()._ataques;
            foreach (GameObject o in _ataques)
            {
                if (o.GetComponent<Ataque>()._numeroQuadro == 0)
                {
                    if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Count != 0)
                    {
                        _mao.GetComponent<Animator>().SetInteger("Ataque", o.GetComponent<Ataque>()._idAtaque);
                    }
                }
            }
        }
        if(Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(0)) 
        {
            Ataque _atq = new Ataque();
            List<GameObject> _ataques = _mao.GetComponent<Mao>()._ataques;
            foreach (GameObject o in _ataques)
            {
                if (o.GetComponent<Ataque>()._numeroQuadro == 1)
                {
                    if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Count != 0)
                    {
                        _mao.GetComponent<Animator>().SetInteger("Ataque", o.GetComponent<Ataque>()._idAtaque);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            Ataque _atq = new Ataque();
            List<GameObject> _ataques = _mao.GetComponent<Mao>()._ataques;
            foreach (GameObject o in _ataques)
            {
                if (o.GetComponent<Ataque>()._numeroQuadro == 2)
                {
                    if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Count != 0)
                    {
                        _mao.GetComponent<Animator>().SetInteger("Ataque", o.GetComponent<Ataque>()._idAtaque);
                    }
                }
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
