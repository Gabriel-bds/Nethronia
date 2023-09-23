using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using System;
using UnityEditor.U2D.Aseprite;
using TMPro;
using UnityEngine.AI;

[Serializable]
public class Ataque : MonoBehaviour
{
    public Efeito _efeitoAplicado;
    [Range(0f, 100f)] [SerializeField] float _dano;
    [Range(0f, 100f)] [SerializeField] float _repulsao;
    public float _tempoRecarga;
    [SerializeField] LayerMask _alvos;
    [SerializeField] Color _corDano;
    [HideInInspector] public Ser_Vivo _dono;
    [SerializeField] AudioSource _somHit;

    private void Start()
    {
        _efeitoAplicado = GetComponent<Efeito>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _alvos) != 0)
        {
            Ser_Vivo _atingido = collision.gameObject.GetComponent<Ser_Vivo>();
            float _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * (_dono._danoFisicoMax - _atingido._poderResistencia._negacaoDano));
            _atingido._vidaAtual -= _danoSofrido;
            _atingido._barraVida.AtualizarVida(_atingido._vidaMax, _atingido._vidaAtual);
            _atingido.AnimacaoDanoSofrido(_danoSofrido * 100 / _atingido._vidaMax);
            _atingido.StartCoroutine(_atingido.PiscarCor(_corDano));

            Utilidades.InstanciarNumeroDano((-_danoSofrido).ToString(), _atingido.transform);

            ParticleSystem _objSangue = Instantiate(_atingido._sangue, _atingido.transform.position, _atingido.transform.rotation).GetComponent<ParticleSystem>();
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _danoSofrido * 100 / _atingido._vidaMax / 100 * 2000;

            Vector2 _distancia = (Vector2)(collision.transform.position - _dono.transform.position).normalized;
            _atingido.Knockback(_repulsao / 100 * (_dono._repulsaoFisicaMax - _atingido._poderResistencia._negacaoRepulsao), _distancia);

            if(_efeitoAplicado != null) 
            {
                _efeitoAplicado.Aplicar(_dono, _atingido);
            }

            Camera_Controller _camera = FindObjectOfType<Camera_Controller>();
            _camera.Tremer(_danoSofrido * 100 / _atingido._vidaMax);

            _somHit.Play();
        }
    }

    public void AutoDestruir()
    {
        Destroy(gameObject);
    }
}