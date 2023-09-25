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
    private Efeito _efeitoAplicado;
    public Tipo_Dano _tipoDano;
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
            float _danoSofrido = 0;
            switch(_tipoDano)
            {
                case Tipo_Dano.Físico:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._danoFisicoMax - _atingido._poderResistencia._negacaoDano);
                    break;

                case Tipo_Dano.Fogo:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderFogo._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderFogo._negacaoDano);
                    break;

                case Tipo_Dano.Gelo:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderGelo._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderGelo._negacaoDano);
                    break;

                case Tipo_Dano.Veneno:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderVeneno._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderVeneno._negacaoDano);
                    break;

                case Tipo_Dano.Eletricidade:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderEletricidade._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderEletricidade._negacaoDano);
                    break;
            }
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