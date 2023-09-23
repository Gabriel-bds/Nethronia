using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Projetil : MonoBehaviour
{
    
    [Range(0f, 100f)][SerializeField] float _dano;
    [Range(-100f, 100f)] [SerializeField] float _repulsao;
    [SerializeField] float _velocidade;
    public float _tempoRecarga;
    [SerializeField] float _tempoDeVida;
    float _tempoDeVidaAtual;
    [SerializeField] LayerMask _alvos;
    [SerializeField] LayerMask _colisoes;
    [SerializeField] Color _corDano;
    [SerializeField] AudioSource _quebrandoSom;
    //o dono é atribuido no script da mão, quando o projetil é instanciado
    [HideInInspector] public Ser_Vivo _dono;
    Vector3 _posInicialDono;
    // Start is called before the first frame update
    void Start()
    {
        _tempoDeVidaAtual = _tempoDeVida;
        _posInicialDono = _dono.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Movimentar();
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
            _objSangue.gameObject.GetComponent<AudioSource>().volume = _danoSofrido * 100 / _atingido._vidaMax / 100 * 10;
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _danoSofrido * 100 / _atingido._vidaMax / 100 * 2000;
            
            
            Vector2 _distancia = (Vector2)(collision.transform.position - _posInicialDono).normalized;
            _atingido.Knockback(_repulsao / 100 * (_dono._repulsaoFisicaMax - _atingido._poderResistencia._negacaoRepulsao), _distancia);

            Camera_Controller _camera = FindObjectOfType<Camera_Controller>();
            _camera.Tremer(_danoSofrido * 100 / _atingido._vidaMax);

        }
        if (((1 << collision.gameObject.layer) & _colisoes) != 0)
        {
            _quebrandoSom.Play();
            IniciarDestuicao();
        }
    }
    void Movimentar()
    {
        if(_tempoDeVidaAtual  > 0)
        { 
            transform.Translate(Vector3.right * _velocidade * Time.deltaTime);
            _tempoDeVidaAtual -= Time.deltaTime;
        }
        else
        {
            IniciarDestuicao();
        }
        
    }
    void IniciarDestuicao()
    {
        _velocidade = 0;
        Animator _animator = GetComponent<Animator>();
        _animator.SetBool("Destruir", true);
    }
    public void AutoDestruir()
    {
        Destroy(gameObject);
    }

}
