using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;
[System.Serializable]

public class Ser_Vivo : MonoBehaviour
{
    [Header("Atributos Base:")]
    public float _vidaMax;
    public float _vidaAtual;
    public float _danoFisicoMax = 10;
    public float _negacaoDano;
    public float _repulsaoFisicaMax = 0.2f;
    public float _velocidadeMovimento;
    public int _experiencia;

    [Header("Poderes:")]
    public Poder_Forca _poderForca = new Poder_Forca(0,0,0,0,Tipo_Dano.Eletricidade);
    public Poder_Resistencia _poderResistencia;
    public Poder_Vitalidade _poderVitalidade;
    public Poder_Velocidade _poderVelocidade;
    public Poder_Fogo _poderFogo;
    public Poder_Gelo _poderGelo;
    public Poder_Veneno _poderVeneno;
    public Poder_Eletricidade _poderEletricidade;

    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    [HideInInspector] public GameObject _mao;

    [Header("Status:")]
    public Status _efeitoIncinerar;
    public Status _efeitoEnvenenar;
    public Status _efeitoCongelar;
    public Status _efeitoFulminar;

    [Header("Outros:")]
    [SerializeField] protected GameObject _alvo;
    [SerializeField] public Barra_Vida _barraVida;
    [SerializeField] public GameObject _sangue;
    [HideInInspector] public int _travar;
    [HideInInspector] public Color _corBase;
    [HideInInspector] public Color _corBaseMao;

    [Header("Sons:")]
    [SerializeField] List<AudioSource> _sons = new List<AudioSource>();


    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _mao = GetComponentInChildren<Mao>().gameObject;
        _vidaAtual = _vidaMax;
        if (_barraVida == null)
        {
            _barraVida = GetComponentInChildren<Barra_Vida>();
        }
        DefinirAtributos();
        _corBase = Color.white;
        _corBaseMao = Color.white;
    }
    protected virtual void Update()
    {
        ControleAnimacoesMovimento();
        Virar();
        TemVida();
        DefinirAtributos();
    }

    protected void Mover(float _direcaoX, float _direcaoY)
    {
        if (_travar == 0)
        {
            Vector2 _ultimaPosicao = transform.position;
            _rigidbody.velocity = new Vector2(_velocidadeMovimento * _direcaoX,
            _velocidadeMovimento * _direcaoY);
        }

    }
    private void ControleAnimacoesMovimento()
    {
        NavMeshAgent _agent = GetComponent<NavMeshAgent>();
        if (_animator != null)
        {
            if (_agent != null)
            {
                _animator.SetFloat("Velocidade_Movimento", _agent.velocity.x + _agent.velocity.y + _agent.velocity.x * _agent.velocity.y);
                _mao.GetComponent<Animator>().SetFloat("Velocidade_Movimento", _agent.velocity.x + _agent.velocity.y);
            }
            else
            {
                _animator.SetFloat("Velocidade_Movimento", _rigidbody.velocity.x + _rigidbody.velocity.y + _rigidbody.velocity.x * _rigidbody.velocity.y);
                _mao.GetComponent<Animator>().SetFloat("Velocidade_Movimento", _rigidbody.velocity.x + _rigidbody.velocity.y);
            }
        }
    }
    public void AnimacaoDanoSofrido(float _danoSofrido)
    {
        if (_danoSofrido <= 0)
        {
            _travar = 0;
            _mao.GetComponent<Mao>()._travar = 0;
        }
        else
        {
            _travar = 1;
            _mao.GetComponent<Mao>()._travar = 1;
        }
        _animator.SetFloat("Dano_Sofrido", _danoSofrido);
        _mao.GetComponent<Animator>().SetFloat("Dano_Sofrido", _danoSofrido);
    }
    public IEnumerator PiscarCor(Color _novaCor)
    {
        SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
        SpriteRenderer _spriteMao = _mao.GetComponent<SpriteRenderer>();
        _sprite.color = _novaCor;
        _spriteMao.color = _novaCor;
        yield return new WaitForSeconds(0.5f);
        _sprite.color = _corBase;
        _spriteMao.color = _corBaseMao;
    }
    private void Virar()
    {
        _barraVida.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (_travar == 0)
        {
            Vector2 _posicaoAlvo;
            if (_alvo != null)
            {
                _posicaoAlvo = Camera.main.WorldToScreenPoint(_alvo.transform.position);
            }
            else
            {
                _posicaoAlvo = Input.mousePosition;
            }
            if (_posicaoAlvo.x < Camera.main.WorldToScreenPoint(transform.position).x)
            {
                transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
                _mao.transform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                _mao.transform.localScale = new Vector3(1, 1, 1);
            }
        }

    }
    public void TravarCorpo(int _travarCorpo)
    {
        _travar = _travarCorpo;
    }
    void TemVida()
    {
        _animator.SetFloat("Vida", _vidaAtual);
        _mao.GetComponent<Animator>().SetFloat("Vida", _vidaAtual);
        if (_vidaAtual < 1e-05)
        {
            _mao.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public void Knockback(float _knockback, Vector2 _distancia)
    {
        if (GetComponent<NavMeshAgent>() != null)
        {
            GetComponent<Inimigo>().TravarAgent(true);
            _rigidbody.AddForce(_distancia * _knockback, ForceMode2D.Impulse);
            GetComponent<Inimigo>().TravarAgent(false);
        }
        else
        {
            _rigidbody.AddForce(_distancia * _knockback, ForceMode2D.Impulse);
        }

    }
    public void Morte(float _tempo)
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        _mao.GetComponent<SpriteRenderer>().sortingOrder = 1;
        if (GetComponentInChildren<Canvas>() != null)
        {
            Destroy(GetComponentInChildren<Canvas>().gameObject);
        }
        if (GetComponent<NavMeshAgent>() != null)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }
        StartCoroutine(DestruirCorpo(_tempo));
    }
    IEnumerator DestruirCorpo(float _tempo)
    {
        yield return new WaitForSeconds(_tempo);
        if (GetComponent<Inimigo>() != null)
        {
            FindAnyObjectByType(typeof(Player)).GetComponent<Player>()._experiencia += _experiencia;
        }
        Destroy(gameObject);
    }
    void DefinirAtributos()
    {
        _danoFisicoMax = ControleAtributos(_poderForca._nivel, 10f, 1.5f);
        _repulsaoFisicaMax = ControleAtributos(_poderForca._nivel, 10f, 1.5f);
        /*_negacaoDano = ControleAtributos(_poderResistencia._nivel, 0f, 1.5f);
        _negacaoRepulsao = ControleAtributos(_poderResistencia._nivel, 0f, 1.5f);*/
    }
    float ControleAtributos(int _nivel, float _valorInicial, float _progressao)
    {
        return _valorInicial + _nivel * _progressao;
    }
    float ControleAtributos(int _nivel, float _valorInicial, float _progressao, float _limite)
    {
        if (_valorInicial + _nivel * _progressao > _limite)
        {
            return _limite;
        }
        else
        {
            return _valorInicial + _nivel * _progressao;
        }
    }
    float ControleAtributos(int _nivel, int _nivelAlvo, float _valorAlvo)
    {
        return _nivel * 100 / _nivelAlvo * _valorAlvo / 100;
    }
    float ControleAtributos(int _nivel, int _nivelAlvo, float _valorAlvo, float _valorLimite)
    {
        if (_nivel * 100 / _nivelAlvo * _valorAlvo / 100 > _valorLimite)
        {
            return _valorLimite;
        }
        else
        {
            return _nivel * 100 / _nivelAlvo * _valorAlvo / 100;
        }
    }
    public void IniciarSom(int _numeroNaLista)
    {
        _sons[_numeroNaLista].Play();
    }
}
