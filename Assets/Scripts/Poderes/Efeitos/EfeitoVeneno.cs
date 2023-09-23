using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfeitoVeneno : Efeito
{
    void Start()
    {
        _mensagemAviso = "Envenenado!";
        _cor = new Color32(15, 43, 19, 255);
        _particulaEfeito = Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Envenenado");
    }
    public override void Aplicar(Ser_Vivo _atacante, Ser_Vivo _vitima)
    {
        _vitima._efeitoEnvenenar._acumuloAtual += _atacante._efeitoEnvenenar._infligirAcumulo;
        if(_vitima._efeitoEnvenenar._acumuloAtual >= _vitima._efeitoEnvenenar._acumuloMax)
        {
            base.Aplicar(_atacante, _vitima);
            Utilidades.AplicarDano(_vitima, Utilidades.ArredondarNegativo(_atacante._efeitoEnvenenar._dano - _vitima._efeitoEnvenenar._negacaoDano), 45, 3, _cor);
            _vitima._efeitoEnvenenar._acumuloAtual = 0;
            InstanciarParticulaEfeito(45, _vitima, _atacante);
        }
    }
    void InstanciarParticulaEfeito(float _duracao, Ser_Vivo _vitima, Ser_Vivo _atacante)
    {
        GameObject _instanciaParticula = Instantiate(_particulaEfeito, _vitima.gameObject.transform);
        _instanciaParticula.transform.position = new Vector2(_vitima.gameObject.GetComponent<Collider2D>().bounds.center.x, _vitima.gameObject.GetComponent<Collider2D>().bounds.center.y - _vitima.gameObject.GetComponent<Collider2D>().bounds.extents.y);
        ParticleSystem[] _particulas = _instanciaParticula.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem _particula in _particulas)
        {
            var _config = _particula.main;
            var _emissao = _particula.emission;
            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(0, _emissao.rateOverTime.constant, (_atacante._efeitoEnvenenar._dano - _vitima._efeitoEnvenenar._negacaoDano) / (_vitima._vidaMax * 0.07f) * _emissao.rateOverTime.constant));
            _config.duration = _duracao;
        }
        Destroy(_instanciaParticula, _duracao);
    }
}
