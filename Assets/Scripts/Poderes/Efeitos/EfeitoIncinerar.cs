using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EfeitoIncinerar : Efeito
{ 
    void Start()
    {
        _mensagemAviso = "Incinerado!";
        _cor = new Color32(255, 104, 0, 255);
        _particulaEfeito = Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Incineracao");
        _particulaExplosao = Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Explosao inicinerar");
    }
    public override void Aplicar(Ser_Vivo _atacante, Ser_Vivo _vitima)
    {
        _vitima._poderFogo._status._acumuloAtual += _atacante._poderFogo._status._infligirAcumulo;
        if(_vitima._poderFogo._status._acumuloAtual >= _vitima._poderFogo._status._acumuloMax)
        {
            base.Aplicar(_atacante, _vitima);
            Utilidades.AplicarDano(_vitima, Utilidades.ArredondarNegativo(_atacante._poderFogo._status._dano - _vitima._poderFogo._status._negacaoDano), 10, 1, _cor);
            _vitima._poderFogo._status._acumuloAtual = 0;
            InstanciarParticulaExplosao(_vitima, _atacante);
            InstanciarParticulaEfeito(10, _vitima, _atacante);
        }
    }
    void InstanciarParticulaEfeito(float _duracao, Ser_Vivo _vitima, Ser_Vivo _atacante)
    {
        GameObject _instanciaParticula = Instantiate(_particulaEfeito, _vitima.gameObject.transform);
        _instanciaParticula.transform.position = new Vector2(_vitima.gameObject.GetComponent<Collider2D>().bounds.center.x, _vitima.gameObject.GetComponent<Collider2D>().bounds.center.y - _vitima.gameObject.GetComponent<Collider2D>().bounds.extents.y);
        Light2D _luz = _instanciaParticula.GetComponentInChildren<Light2D>();
        _luz.intensity = Utilidades.LimitadorNumero(0, _luz.intensity, (_atacante._poderFogo._status._dano - _vitima._poderFogo._status._negacaoDano) / (_vitima._vidaMax * 0.5f) * _luz.intensity);
        ParticleSystem[] _particulas =  _instanciaParticula.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem _particula in _particulas)
        {
            var _config = _particula.main;
            var _emissao = _particula.emission;
            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(0, _emissao.rateOverTime.constant, (_atacante._poderFogo._status._dano - _vitima._poderFogo._status._negacaoDano) / (_vitima._vidaMax * 0.5f) * _emissao.rateOverTime.constant));
            _config.duration = _duracao;
        }
        Destroy(_instanciaParticula, _duracao);
    }
    void InstanciarParticulaExplosao(Ser_Vivo _vitima, Ser_Vivo _atacante)
    {
        GameObject _instanciaParticula = Instantiate(_particulaExplosao, _vitima.transform.position, Quaternion.Euler(0,0,0));
        _instanciaParticula.transform.position = new Vector2(_vitima.gameObject.GetComponent<Collider2D>().bounds.center.x, _vitima.gameObject.GetComponent<Collider2D>().bounds.center.y);
        ParticleSystem[] _particulas = _instanciaParticula.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem _particula in _particulas)
        {
            var _config = _particula.main;
            var _emissao = _particula.emission;
            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(0, _emissao.rateOverTime.constant, (_atacante._poderFogo._status._dano - _vitima._poderFogo._status._negacaoDano) / (_vitima._vidaMax * 0.5f) * _emissao.rateOverTime.constant));
        }
        Destroy(_instanciaParticula, 2);
    }
}
