using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class EfeitoCongelar : Efeito
{
    void Start()
    {
        _mensagemAviso = "Congelar!";
        _cor = new Color32(0, 145, 255, 255);
        _particulaEfeito = Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Flocos");
    }

    async void Congelar(Ser_Vivo _vitima, Ser_Vivo _atacante)
    {
        //Atributos antes do congelamento:
        Color _cor = Color.white;
        Color _corMao = Color.white;
        Color _corBase = _vitima._corBase;
        Color _corMaoBase = _vitima._corBaseMao;
        float _velocidadeAnimacoes = _vitima.GetComponent<Animator>().speed;
        float _velocidadeAnimacoesMao = _vitima._mao.GetComponent<Animator>().speed;
        float _velocidadeNavMesh = _vitima.gameObject.GetComponent<NavMeshAgent>().speed;
        float _latenciaMira = _vitima._mao.GetComponent<Mao>()._latenciaMira;


        //Congelamento:
        _vitima.GetComponent<SpriteRenderer>().color = new Color(0, 145, 255, 255);
        _vitima._mao.GetComponent<SpriteRenderer>().color = new Color(0, 145, 255, 255);
        _vitima._corBase = new Color(0, 145, 255, 255);
        _vitima._corBaseMao = new Color(0, 145, 255, 255);

        _vitima.GetComponent<Animator>().speed = 1 - (Utilidades.ArredondarNegativo((_atacante._efeitoCongelar._utilidade1 - _vitima._efeitoCongelar._negacaoUtilidade1) / 50));
        _vitima._mao.GetComponent<Animator>().speed = 1 - (Utilidades.ArredondarNegativo((_atacante._efeitoCongelar._utilidade1 - _vitima._efeitoCongelar._negacaoUtilidade1) / 50));
        if (_vitima.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            float _velocidadePadrao = _vitima.gameObject.GetComponent<NavMeshAgent>().speed;
            _vitima.gameObject.GetComponent<NavMeshAgent>().speed = _velocidadePadrao * (1 - (Utilidades.ArredondarNegativo((_atacante._efeitoCongelar._utilidade1 - _vitima._efeitoCongelar._negacaoUtilidade1) / 50)));
        }
        _vitima._mao.GetComponent<Mao>()._latenciaMira = _latenciaMira * (1 - (Utilidades.ArredondarNegativo((_atacante._efeitoCongelar._utilidade1 - _vitima._efeitoCongelar._negacaoUtilidade1) / 50)));

        //tempo para descongelar:
        await Task.Delay(5000);

        //Descongelamento:
        _vitima.GetComponent<SpriteRenderer>().color = _cor;
        _vitima._mao.GetComponent<SpriteRenderer>().color = _corMao;
        _vitima._corBase = _corBase;
        _vitima._corBaseMao = _corMaoBase;
        _vitima.GetComponent<Animator>().speed = _velocidadeAnimacoes;
        _vitima._mao.GetComponent<Animator>().speed = _velocidadeAnimacoesMao;
        _vitima.gameObject.GetComponent<NavMeshAgent>().speed = _velocidadeNavMesh;
        _vitima._mao.GetComponent<Mao>()._latenciaMira = _latenciaMira;
    }
    public override void Aplicar(Ser_Vivo _atacante, Ser_Vivo _vitima)
    {
        _vitima._efeitoCongelar._acumuloAtual += _atacante._efeitoCongelar._infligirAcumulo;
        if(_vitima._efeitoCongelar._acumuloAtual >= _vitima._efeitoCongelar._acumuloMax)
        {
            base.Aplicar(_atacante, _vitima);
            _vitima._efeitoCongelar._acumuloAtual = 0;
            Congelar(_vitima, _atacante);
            InstanciarParticulaEfeito(7, _vitima, _atacante);
        }
    }
    void InstanciarParticulaEfeito(float _duracao, Ser_Vivo _vitima, Ser_Vivo _atacante)
    {
        GameObject _instanciaParticula = Instantiate(_particulaEfeito, _vitima.gameObject.transform);
        _instanciaParticula.transform.position = new Vector2(_vitima.gameObject.GetComponent<Collider2D>().bounds.center.x, _vitima.gameObject.GetComponent<Collider2D>().bounds.center.y - _vitima.gameObject.GetComponent<Collider2D>().bounds.extents.y);
        var _emissao = _instanciaParticula.GetComponent<ParticleSystem>().emission;
        _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(0, _emissao.rateOverTime.constant, (_atacante._efeitoCongelar._utilidade1 - _vitima._efeitoCongelar._negacaoUtilidade1) / 50 * _emissao.rateOverTime.constant));
        Destroy(_instanciaParticula, _duracao);
    }
}
