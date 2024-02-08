using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parar_Tempo : Ataque
{
    protected override void Start()
    {
        CongelarTempo();
    }
    public void CongelarTempo()
    {
        CinemachineVirtualCamera _camera = FindObjectOfType<CinemachineVirtualCamera>();
        if (Time.timeScale == 1)
        {
            Time.timeScale -= _dono._poderVelocidade._reducaoTempo;
            Time.fixedDeltaTime *= Time.timeScale;
            _dono._velocidadeMovimento = _dono._velocidadeMovimento / (Time.timeScale / 0.5f) / (Time.timeScale / 0.5f * 2);
            _dono._animator.speed = 1 / Time.timeScale;
            _dono._mao.GetComponent<Mao>()._animator.speed = 1 / Time.timeScale;
            _dono._mao.GetComponent<Mao>()._latenciaMira /= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime *= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping *= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping *= Time.timeScale;
        }
        else
        {
            Time.fixedDeltaTime /= Time.timeScale;
            _dono._mao.GetComponent<Mao>()._latenciaMira *= Time.timeScale;
            _dono._velocidadeMovimento = 10 + _dono._poderVelocidade._acrescimoVelocidadeMovimento;
            _dono.GetComponent<Animator>().speed = 1 + _dono._poderVelocidade._acrescimoVelocidadeAnimações;
            _dono._mao.GetComponent<Animator>().speed = 1 + _dono._poderVelocidade._acrescimoVelocidadeAnimações;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_LookaheadTime /= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping /= Time.timeScale;
            _camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping /= Time.timeScale;
            Time.timeScale = 1;
        }
    }
}
