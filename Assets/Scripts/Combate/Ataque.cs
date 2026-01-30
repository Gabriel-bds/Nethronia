using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using System;
using TMPro;
using UnityEngine.AI;
using System.Threading.Tasks;
using FMODUnity;

public enum Spawn_Atq { Spawn, Mao, Spawn1, Mouse}

[Serializable]
public class Ataque : MonoBehaviour
{
    public Sprite _icone;
    public int _idAtaque;
    protected Efeito _efeitoAplicado;
    public Tipo_Dano _tipoDano;
    [Range(0f, 100f)] [SerializeField] protected float _dano;
    [Range(0f, 100f)] [SerializeField] protected float _repulsao;
    public float _tempoRecargaTotal;
    [SerializeField] protected float _tempoDeVida;
    protected float _tempoDeVidaAtual;


    [SerializeField]protected LayerMask _alvos;
    [SerializeField] protected Color _corDano;
    [HideInInspector] public Ser_Vivo _dono;
    [SerializeField] protected AudioSource _somHit;
    [SerializeField] protected string _tagSomHit;
    [SerializeField] protected string _tagSomInstanciar;
    [HideInInspector] public Vector3 _spawnPosicao;
    [HideInInspector] public Quaternion _spawnRotacao;
    [SerializeField] Spawn_Atq _localSpawn;
    [SerializeField] GameObject _particulasAoInstanciar;
    [SerializeField] protected int nivelMaximoMagnitudeVisual;
    public float _decaimentoRecarga;
    [Header("Inimigos:")]
    public float _distanciaMin;
    public float _distanciaMax;
    public float _distanciaPerfeita;
    protected virtual void Start()
    {
        _efeitoAplicado = GetComponent<Efeito>();
        //ControlarParticulas();
        ControleParticulasAoInstanciarAtaque();
        ControlarMagnitudadeVisual();
        /*float maximoShakeCamera = 20f;
        Camera_Controller _camera = FindObjectOfType<Camera_Controller>();
        _camera.Tremer(Utilidades.LimitadorNumero(0, maximoShakeCamera, (float)Utilidades.NivelAtualTipoDano(_tipoDano, _dono) / nivelMaximoMagnitudeVisual * maximoShakeCamera));*/
        ControlarRecarga();
        if(_tempoDeVida > 0) 
        {
            Destroy(gameObject, _tempoDeVida);
        }
        SomIntanciar();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        AplicarAtaque(collision);
    }

    public void AutoDestruir()
    {
        Destroy(gameObject);
    }
    void ControlarParticulas()
    {
        float _minParticulas = 0.3f;
        if (GetComponent<ParticleSystem>() != null)
        {
            var _emissao = GetComponent<ParticleSystem>().emission;
            switch (_tipoDano)
            {
                case Tipo_Dano.Fogo:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderFogo._nivel / 100));
                    break;

                case Tipo_Dano.Gelo:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderGelo._nivel / 100));
                    break;

                case Tipo_Dano.Veneno:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderVeneno._nivel / 100));
                    break;

                case Tipo_Dano.Eletricidade:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderEletricidade._nivel / 100));
                    break;
            }
        }
        else
        {
            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                ParticleSystem[] _particulas = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem _particula in _particulas)
                {
                    var _emissao = _particula.emission;
                    switch (_tipoDano)
                    {
                        case Tipo_Dano.Fogo:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderFogo._nivel / 100));
                            break;

                        case Tipo_Dano.Gelo:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderGelo._nivel / 100));
                            break;

                        case Tipo_Dano.Veneno:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderVeneno._nivel / 100));
                            break;

                        case Tipo_Dano.Eletricidade:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderEletricidade._nivel / 100));
                            break;
                    }
                }
            }
        }
    void ControlarParticulas()
    {
        float _minParticulas = 0.3f;
        if (GetComponent<ParticleSystem>() != null)
        {
            var _emissao = GetComponent<ParticleSystem>().emission;
            switch (_tipoDano)
            {
                case Tipo_Dano.Fogo:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderFogo._nivel / 100));
                    break;

                case Tipo_Dano.Gelo:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderGelo._nivel / 100));
                    break;

                case Tipo_Dano.Veneno:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderVeneno._nivel / 100));
                    break;

                case Tipo_Dano.Eletricidade:
                    _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderEletricidade._nivel / 100));
                    break;
            }
        }
        else
        {
            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                ParticleSystem[] _particulas = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem _particula in _particulas)
                {
                    var _emissao = _particula.emission;
                    switch (_tipoDano)
                    {
                        case Tipo_Dano.Fogo:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderFogo._nivel / 100));
                            break;

                        case Tipo_Dano.Gelo:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderGelo._nivel / 100));
                            break;

                        case Tipo_Dano.Veneno:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderVeneno._nivel / 100));
                            break;

                        case Tipo_Dano.Eletricidade:
                            _emissao.rateOverTime = new ParticleSystem.MinMaxCurve(Utilidades.LimitadorNumero(_emissao.rateOverTime.constant * _minParticulas, _emissao.rateOverTime.constant, _emissao.rateOverTime.constant * _minParticulas + (_emissao.rateOverTime.constant - _emissao.rateOverTime.constant * _minParticulas) * _dono._poderEletricidade._nivel / 100));
                            break;
                    }
                }
            }
        }
    }
    }
    protected void ControlarMagnitudadeVisual()
    {
        if(GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().SetFloat("Forca", Utilidades.LimitadorNumero(0, 1, (float)Utilidades.NivelAtualTipoDano(_tipoDano, _dono) / nivelMaximoMagnitudeVisual));
        }
    }
    public void DefinirSpawn()
    {
        switch(_localSpawn) 
        {
            case Spawn_Atq.Spawn:
                var _spawns = GameObject.FindGameObjectsWithTag("SpawnAtaques");
                foreach (var _spawn in _spawns)
                {
                    if (_spawn.GetComponentInParent<Ser_Vivo>() == _dono)
                    {
                        _spawnPosicao = _spawn.transform.position;
                        _spawnRotacao = _spawn.transform.rotation;
                        break;
                    }
                }
                break;

            case Spawn_Atq.Mao:

                _spawnPosicao = _dono.GetComponentInChildren<Mao>().transform.position;
                _spawnRotacao = _dono.GetComponentInChildren<Mao>().transform.rotation;
                break;

            case Spawn_Atq.Spawn1:
                var _spawns1 = GameObject.FindGameObjectsWithTag("Spawn_Ataque/Spawn1");
                foreach(var _spawn in _spawns1)
                {
                    if(_spawn.GetComponentInParent<Ser_Vivo>() == _dono)
                    {
                        _spawnPosicao = _spawn.transform.position;
                        _spawnRotacao = _spawn.transform.rotation;
                        break;
                    }
                }
                break;

            case Spawn_Atq.Mouse:
                _spawnPosicao = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                _spawnRotacao = Quaternion.Euler(0, 0, 0);
                break;
        }
        transform.position = _spawnPosicao;
        transform.rotation = _spawnRotacao;
    }
    public static Quadro_Habilidade QuadroDoAtaque(GameObject _dono,GameObject _atq)
    {
        Quadro_Habilidade[] _quadros = FindObjectsByType<Quadro_Habilidade>(FindObjectsSortMode.InstanceID);
        foreach (Quadro_Habilidade _quadro in _quadros)
        {
            if (_quadro._numeroQuadro == _dono.GetComponentInChildren<Mao>()._ataques.IndexOf(_atq))
            {
                return _quadro;
            }
        }
        return null;
        
    }
    public Quadro_Habilidade QuadroDoAtaque()
    {
        foreach (GameObject _atq in _dono.GetComponentInChildren<Mao>()._ataques)
        {
            if(_atq.GetComponent<Ataque>()._idAtaque == _idAtaque)
            {
                Quadro_Habilidade[] _quadros = FindObjectsByType<Quadro_Habilidade>(FindObjectsSortMode.InstanceID);
                foreach (Quadro_Habilidade _quadro in _quadros)
                {
                    if (_quadro._numeroQuadro == _dono.GetComponentInChildren<Mao>()._ataques.IndexOf(_atq))
                    {
                        return _quadro;
                    }
                }
            }
        }  
        return null;
    }
    public void ControleParticulasAoInstanciarAtaque()
    {
        if(_particulasAoInstanciar != null)
        {
            //Debug.Log(Utilidades.LimitadorNumero(0, 1, (float)Utilidades.NivelAtualTipoDano(_tipoDano, _dono) / 100));
            Instantiate(_particulasAoInstanciar, _spawnPosicao, _spawnRotacao).GetComponent<Animator>().SetFloat("Forca", Utilidades.LimitadorNumero(0, 1, (float)Utilidades.NivelAtualTipoDano(_tipoDano, _dono) / nivelMaximoMagnitudeVisual));
        }
    }
    protected virtual void AplicarAtaque(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _alvos) != 0 && !collision.GetComponent<Ser_Vivo>()._invulneravel)
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
            _atingido._vidaAtual = _atingido._vidaAtual - _danoSofrido > 0
                ? _atingido._vidaAtual -= _danoSofrido
                : _atingido._vidaAtual = 0;
            _atingido._barraVida.AtualizarVida(_atingido._vidaMax, _atingido._vidaAtual);
            _atingido.AnimacaoDanoSofrido(_danoSofrido / _atingido._vidaMax);
            //_atingido.StartCoroutine(_atingido.PiscarCor(_corDano));

            Utilidades.InstanciarNumeroDano((-_danoSofrido).ToString(), _atingido.transform);

            ParticleSystem _objSangue = Instantiate(_atingido._sangue, _atingido.transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<ParticleSystem>();
            var _emissao = _objSangue.emission;
            _emissao.rateOverTime = _danoSofrido * 100 / _atingido._vidaMax / 100 * _emissao.rateOverTime.constant;

            if (_efeitoAplicado != null)
            {
                _efeitoAplicado.Aplicar(_dono, _atingido);
            }
            Camera_Controller _camera = FindObjectOfType<Camera_Controller>();
            _camera.Tremer(_danoSofrido * 100 / _atingido._vidaMax);

            //_somHit.Play();
            //RuntimeManager.PlayOneShot(_tagSomHit);
            SomHit(_danoSofrido / _atingido._vidaMax);

            if (_atingido.GetComponent<Player>() != null)
            {
                FindAnyObjectByType<Numero_BarraVida>().AtualizarNumero();
            }
        }
    }
    protected void ControlarRecarga()
    {
        foreach(GameObject habilidade in _dono._mao.GetComponent<Mao>()._ataquesDisponiveis)
        {
            if(habilidade.GetComponent<Ataque>()._idAtaque == _idAtaque) 
            {
                _dono._mao.GetComponent<Mao>().StartCoroutine(_dono._mao.GetComponent<Mao>().RecarregarAtaque(habilidade.GetComponent<Ataque>()._tempoRecargaTotal,habilidade));
                break;
            }
        }  
    }

    public void TocarSom(string tag)
    {
        var instance = RuntimeManager.CreateInstance(tag);
        instance.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform.position)
        );
        instance.start();
        instance.release();
    }

    public void SomHit(float danoProporcionalSofrido)
    {
        //Debug.Log(danoProporcionalSofrido);
        if (string.IsNullOrEmpty(_tagSomHit)) return;
        var instance = RuntimeManager.CreateInstance(_tagSomHit);
        instance.setParameterByName("hitProjetil", Mathf.Clamp(danoProporcionalSofrido, 0f, 1f));
        instance.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform.position)
        );
        instance.start();
        instance.release();
    }
    public void SomIntanciar()
    {
        //Debug.Log(danoProporcionalSofrido);
        if (!String.IsNullOrEmpty(_tagSomInstanciar))
        {
            var instance = RuntimeManager.CreateInstance(_tagSomInstanciar);
            try
            {
                instance.setParameterByName("somIntanciarAtaque", Mathf.Clamp((float)Utilidades.NivelAtualTipoDano(_tipoDano, _dono) / nivelMaximoMagnitudeVisual, 0f, 1f));
            }
            catch { }
            instance.set3DAttributes(
                FMODUnity.RuntimeUtils.To3DAttributes(transform.position)
            );
            instance.start();
            instance.release();
        }
       
    }
}