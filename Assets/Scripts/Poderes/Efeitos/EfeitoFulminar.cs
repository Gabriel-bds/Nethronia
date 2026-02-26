using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EfeitoFulminar : Efeito
{
    void Start()
    {
        _mensagemAviso = "Fulminado!";
        _cor = Color.yellow;
        _particulaExplosao = Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Raio");

    }
    public override void Aplicar(Ser_Vivo _atacante, Ser_Vivo _vitima)
    {
        _vitima._poderEletricidade._status._acumuloAtual += _atacante._poderEletricidade._status._infligirAcumulo;
        if(_vitima._poderEletricidade._status._acumuloAtual > _vitima._poderEletricidade._status._acumuloMax)
        {
            base.Aplicar(_atacante, _vitima);
            _vitima._poderEletricidade._status._acumuloAtual = 0;
            CircleCollider2D _area =  CriarArea(_vitima, _atacante);
            GameObject _instanciaParticula =  Instantiate(_particulaExplosao, new Vector2(_vitima.gameObject.GetComponent<Collider2D>().bounds.center.x, _vitima.gameObject.GetComponent<Collider2D>().bounds.center.y), Quaternion.Euler(0, 0, 0));
            var _configuracao = _instanciaParticula.GetComponentInChildren<ParticleSystem>().main;
            Light2D _luz = _instanciaParticula.GetComponentInChildren<Light2D>();
            _luz.pointLightOuterRadius = _area.radius * 1.5f;
            _luz.pointLightInnerRadius = _area.radius * 0.5f;
            _configuracao.startLifetime = _area.radius * 0.05f;
            Destroy(_instanciaParticula, 1.5f);
        }
    }
    CircleCollider2D CriarArea(Ser_Vivo _vitima, Ser_Vivo _atacante)
    {
        GameObject _area = new GameObject();
        GameObject _intanciaArea = Instantiate(_area, new Vector2(_vitima.gameObject.GetComponent<Collider2D>().bounds.center.x, _vitima.gameObject.GetComponent<Collider2D>().bounds.center.y), _vitima.transform.rotation);
        _intanciaArea.AddComponent<CircleCollider2D>();
        _intanciaArea.tag = "Combate/Area eletrica";
        _intanciaArea.GetComponent<CircleCollider2D>().isTrigger = true;
        _intanciaArea.GetComponent<CircleCollider2D>().radius = Utilidades.LimitadorNumero(3, 10,_atacante._poderEletricidade._status._utilidade1 - _vitima._poderEletricidade._status._negacaoUtilidade1);
        _intanciaArea.AddComponent<AreaEletricidade>();
        _intanciaArea.GetComponent<AreaEletricidade>()._atacante = _atacante;
        _intanciaArea.GetComponent<AreaEletricidade>()._vitima = _vitima;
        Destroy(_area);
        Destroy(_intanciaArea, 0.5f);
        return _intanciaArea.GetComponent<CircleCollider2D>();
    }
}

public class AreaEletricidade : MonoBehaviour
{
    public Ser_Vivo _atacante;
    public Ser_Vivo _vitima;
    public Numero_Dano _numeroDano;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _vitima.gameObject.layer)
        {
            Ser_Vivo _atingido = collision.GetComponent<Ser_Vivo>();
            float _danoSofrido = Utilidades.ArredondarNegativo(_atacante._poderEletricidade._status._dano - _atingido._poderEletricidade._status._negacaoDano);
            _atingido.VidaAtual -= _danoSofrido;
            //_atingido._barraVida.AtualizarVida(_atingido._vidaMax, _atingido.VidaAtual);
            _atingido.AnimacaoDanoSofrido(_danoSofrido * 100 / _atingido._vidaMax);
            _atingido.StartCoroutine(_atingido.PiscarCor(new Color(255, 255, 0, 255)));
            Utilidades.InstanciarNumeroDano((-_danoSofrido).ToString(), _atingido.gameObject.transform, Color.yellow);

            ParticleSystem _objSangue = Instantiate(_atingido._sangue, _atingido.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _danoSofrido * 100 / _atingido._vidaMax / 100 * _emissao.rateOverTime.constant;
        }
    }
}
