using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Rajada : Ataque
{
    GameObject _spawn;
    bool _alvoDentro;
    [SerializeField] float _intervaloDano;
    protected override void Start()
    {
        base.Start();
        var _spawns1 = GameObject.FindGameObjectsWithTag("Spawn_Ataque/Spawn1");
        foreach (var _spawn in _spawns1)
        {
            if (_spawn.GetComponentInParent<Ser_Vivo>() == _dono)
            {
                this._spawn = _spawn;
                break;
            }
        }
        //ControleCusto();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        GerarDano(collision);
    }
    private void Update()
    {
        transform.rotation = _spawn.transform.rotation;
        transform.position = _spawn.transform.position;
        ControleCusto();
    }
    public void PararRajada()
    {
        GetComponent<ParticleSystem>().Stop();
    }
    async void GerarDano(Collider2D collision)
    {
        if(collision.GetComponent<Ser_Vivo>() != null)
        {
            if (GetComponent<Collider2D>().IsTouching(collision) && !collision.GetComponent<Ser_Vivo>()._invulneravel)
            {
                if (((1 << collision.gameObject.layer) & _alvos) != 0)
                {
                    Ser_Vivo _atingido = collision.gameObject.GetComponent<Ser_Vivo>();
                    float _danoSofrido = 0;

                    Vector2 _distancia = (Vector2)(collision.transform.position - _dono.transform.position).normalized;

                    switch (_tipoDano)
                    {
                        case Tipo_Dano.F�sico:
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

                    if (_efeitoAplicado != null)
                    {
                        _efeitoAplicado.Aplicar(_dono, _atingido);
                    }

                    Camera_Controller _camera = FindObjectOfType<Camera_Controller>();
                    _camera.Tremer(_danoSofrido * 100 / _atingido._vidaMax);

                    _somHit.Play();

                    await Task.Delay((int)_intervaloDano * 1000);
                    GerarDano(collision);
                }
            }
        
        }
    }
     void ControleCusto()
     {
        if(QuadroDoAtaque()._recargaAtual > 0)
        {
            QuadroDoAtaque()._recargaAtual -= _decaimentoRecarga * Time.deltaTime;
        }
        else
        {
            _dono.GetComponentInChildren<Mao>()._ataquesDisponiveis.Remove(_dono.GetComponentInChildren<Mao>()._ataques[QuadroDoAtaque()._numeroQuadro]);
            PararRajada();
        }
     }
}
