using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using System;
using TMPro;
using UnityEngine.AI;
using System.Threading.Tasks;

[Serializable]
public class Ataque : MonoBehaviour
{
    public Sprite _icone;
    public int _idAtaque;
    private Efeito _efeitoAplicado;
    public Tipo_Dano _tipoDano;
    [Range(0f, 100f)] [SerializeField] float _dano;
    [Range(0f, 100f)] [SerializeField] float _repulsao;
    public float _tempoRecarga;
    [HideInInspector] public float _tempoAtual;
    [SerializeField] LayerMask _alvos;
    [SerializeField] Color _corDano;
    [HideInInspector] public Ser_Vivo _dono;
    public int _numeroQuadro;
    [SerializeField] AudioSource _somHit;

    protected virtual void Start()
    {
        _efeitoAplicado = GetComponent<Efeito>();
        DefinirTempoRecarga();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _alvos) != 0)
        {
            Ser_Vivo _atingido = collision.gameObject.GetComponent<Ser_Vivo>();
            float _danoSofrido = 0;

            Vector2 _distancia = (Vector2)(collision.transform.position - _dono.transform.position).normalized;

            switch (_tipoDano)
            {
                case Tipo_Dano.Físico:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderForca._dano - _atingido._poderResistencia._negacaoDano);
                    _atingido.Knockback(_repulsao / 100 * (_dono._poderForca._repulsao - _atingido._poderResistencia._negacaoRepulsao), _distancia);
                    break;

                case Tipo_Dano.Fogo:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderFogo._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderFogo._negacaoDano);
                    _atingido.Knockback(_repulsao / 100 * (_dono._poderFogo._repulsao - _atingido._poderFogo._negacaoRepulsao), _distancia);

                    break;

                case Tipo_Dano.Gelo:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderGelo._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderGelo._negacaoDano);
                    _atingido.Knockback(_repulsao / 100 * (_dono._poderGelo._repulsao - _atingido._poderGelo._negacaoRepulsao), _distancia);
                    break;

                case Tipo_Dano.Veneno:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderVeneno._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderVeneno._negacaoDano);
                    _atingido.Knockback(_repulsao / 100 * (_dono._poderVeneno._repulsao - _atingido._poderVeneno._negacaoRepulsao), _distancia);
                    break;

                case Tipo_Dano.Eletricidade:
                    _danoSofrido = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderEletricidade._dano - _atingido._poderResistencia._negacaoDano / 2 - _atingido._poderEletricidade._negacaoDano);
                    _atingido.Knockback(_repulsao / 100 * (_dono._poderEletricidade._repulsao - _atingido._poderEletricidade._negacaoRepulsao), _distancia);
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

            if(_efeitoAplicado != null) 
            {
                _efeitoAplicado.Aplicar(_dono, _atingido);
            }

            Camera_Controller _camera = FindObjectOfType<Camera_Controller>();
            _camera.Tremer(_danoSofrido * 100 / _atingido._vidaMax);

            _somHit.Play();
        }
    }
    async void DefinirTempoRecarga()
    {
        if (_dono.GetComponent<Player>() != null)
        {
            _tempoAtual = _tempoRecarga;
            Quadro_Habilidade[] _quadros = FindObjectsOfType<Quadro_Habilidade>();
            Quadro_Habilidade _quadro = new Quadro_Habilidade();
            foreach (Quadro_Habilidade q in _quadros)
            {
                if (q._numeroQuadro == _numeroQuadro)
                {
                    _quadro = q;
                }
            }
            _quadro.CarregarHabilidade(_tempoAtual, _tempoRecarga);
            for (float t = _tempoAtual; t > 0; t -= Time.deltaTime)
            {
                _tempoAtual = t;
                await Task.Delay(1);
            }
        }
    }

    public void AutoDestruir()
    {
        Destroy(gameObject);
    }
}