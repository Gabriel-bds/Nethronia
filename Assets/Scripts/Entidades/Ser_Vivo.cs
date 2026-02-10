using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using FMODUnity;
using static UnityEngine.RuleTile.TilingRuleOutput;
using FMOD;
[System.Serializable]

public class Ser_Vivo : MonoBehaviour
{
    [Header("Atributos Base:")]
    [ReadOnly(true)]
    public float _vidaMax;
    public float _vidaAtual;
    public float _velocidadeMovimento;
    public int _nivelGeral = 0;
    public int _experiencia;
    public int _experienciaParaProximoNivel;
    [HideInInspector] public List<Rigidbody2D> _membros;

    [Header("Poderes:")]
    public Poder_Forca _poderForca = new Poder_Forca(Tipo_Dano.Físico);
    public Poder_Resistencia _poderResistencia = new Poder_Resistencia();
    public Poder_Vitalidade _poderVitalidade = new Poder_Vitalidade();
    public Poder_Velocidade _poderVelocidade = new Poder_Velocidade();
    public Poder_Fogo _poderFogo = new Poder_Fogo(Tipo_Dano.Fogo);
    public Poder_Gelo _poderGelo = new Poder_Gelo(Tipo_Dano.Gelo);
    public Poder_Veneno _poderVeneno = new Poder_Veneno(Tipo_Dano.Veneno);
    public Poder_Eletricidade _poderEletricidade = new Poder_Eletricidade(Tipo_Dano.Eletricidade);

    public Rigidbody2D _rigidbody;
    public Animator _animator;
    public GameObject _mao;

    [Header("Outros:")]
    public GameObject _alvo;
    public Barra_Vida _barraVida;
    public GameObject _sangue;
    public int _travar;
    [HideInInspector] public Color _corBase;
    [HideInInspector] public Color _corBaseMao;
    public bool _invulneravel;

    [Header("Sons:")]
    [SerializeField] List<AudioSource> _sons = new List<AudioSource>();
    [SerializeField] ControleSons controleSons;

    protected virtual void Awake()
    {
        _mao = GetComponentInChildren<Mao>().gameObject;
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _membros = new List<Rigidbody2D>(GetComponentsInChildren<Rigidbody2D>(false));
    }
    protected virtual void Start()
    {
        _corBase = Color.white;
        _corBaseMao = Color.white;
    }
    protected virtual void Update()
    {
        //ControleAnimacoesMovimento();
        Virar();
        TemVida();
    }
    public void Mover(Vector2 _vetor)
    {
        if (_travar == 0)
        {
            _rigidbody.velocity += _vetor  * _velocidadeMovimento;
            _animator.SetFloat("Velocity", Math.Abs(_vetor.x) + Math.Abs(_vetor.y) / 2);
            if (Math.Abs(_vetor.x) >= 0.05f || Math.Abs(_vetor.y) >= 0.05f)
            {
                _animator.SetBool("Run", true);
                return;
            }
            _animator.SetBool("Run", false);
        }

    }
    protected void ControleAnimacoesMovimento()
    {
        NavMeshAgent _agent = GetComponent<NavMeshAgent>();
        if (_animator != null)
        {
            if (_agent != null)
            {
                if(_agent.velocity.magnitude > 0) 
                { 
                    _animator.SetBool("Run", true);
                    //Debug.Log(_agent.velocity.magnitude);
                }
                else
                {
                    _animator.SetBool("Run", false);
                }
                //_mao.GetComponent<Animator>().SetFloat("Velocidade_Movimento", _agent.velocity.x + _agent.velocity.y);
            }
            else
            {
                _animator.SetFloat("Velocity", _rigidbody.velocity.x + _rigidbody.velocity.y + _rigidbody.velocity.x * _rigidbody.velocity.y);
                //_mao.GetComponent<Animator>().SetFloat("Velocidade_Movimento", _rigidbody.velocity.x + _rigidbody.velocity.y);
            }
        }
    }
    public void AnimacaoDanoSofrido(float _danoSofrido)
    {
        /*if (_danoSofrido <= 0)
        {
            _travar = 0;
            _mao.GetComponent<Mao>()._travar = 0;
        }
        else
        {
            _travar = 1;
            _mao.GetComponent<Mao>()._travar = 1;
        }*/

        _animator.SetTrigger("Receber_Dano");
        _animator.SetFloat("Dano_Sofrido", _danoSofrido);
        //Debug.Log(_danoSofrido);
        //_mao.GetComponent<Animator>().SetFloat("Dano_Sofrido", _danoSofrido);
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
    protected void Virar()
    {
        if(_vidaAtual > 0)
        {
            // posição do personagem na tela
            Vector3 posTela = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 _posicaoAlvo;
            if (_alvo != null)
            {
                _posicaoAlvo = Camera.main.WorldToScreenPoint(_alvo.transform.position);
            }
            else
            {
                _posicaoAlvo = Input.mousePosition;
            }

            bool olharParaEsquerda = _posicaoAlvo.x < posTela.x;

            if (olharParaEsquerda)
            {
                // gira SOMENTE no eixo Y, de forma válida
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                if (_mao != null)
                    _mao.transform.localScale = new Vector3(1f, -1f, 1f);
            }
            else
            {
                // rotação neutra (Quaternion válido)
                transform.rotation = Quaternion.identity;

                if (_mao != null)
                    _mao.transform.localScale = Vector3.one;
            }
        }
        
    }

    public void TravarCorpo(int _travarCorpo)
    {
        _travar = _travarCorpo;
    }
    public void TravarMao(int _travarMao)
    {
        _mao.GetComponent<Mao>()._travar = _travarMao;
    }
    public void TravarCorpoMao(int _travar)
    {
        this._travar = _travar;
        _mao.GetComponent<Mao>()._travar = _travar;

    }
    void TemVida()
    {
        _animator.SetFloat("Vida", _vidaAtual);

        /*if(GetComponent<Player>() == null) 
        {
            _mao.GetComponent<Animator>().SetFloat("Vida", _vidaAtual);
        }    */
        if (_vidaAtual < _vidaMax)
        {
            if (!_poderVitalidade._estaRegenerando)
            {
                RegenerarVida();
            }
        }
        else
        {
            _poderVitalidade._estaRegenerando = false;
        }
        if (_vidaAtual > _vidaMax)
        {
            _vidaAtual = _vidaMax;
        }
        if (_vidaAtual < 1e-05)
        {
            _mao.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    protected void Desmembrar(int qtdeMembrosSoltos, float intensidade)
    {
        if (_animator == null) return;

        for (int i = 0; i < qtdeMembrosSoltos; i++)
        {
            if (_membros.Count == 0) break;

            int indice = UnityEngine.Random.Range(0, _membros.Count);
            Rigidbody2D rb = _membros[indice];
            UnityEngine.Transform t = rb.transform;

            // --- 1) Salva pose WORLD antes de qualquer coisa
            Vector3 posWorld = t.position;
            Quaternion rotWorld = t.rotation;

            // --- 2) Quebra hierarquia animada
            t.SetParent(null, true);

            // --- 3) Congela física antes do Rebind
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            rb.simulated = false;

            // --- 4) Força Animator a esquecer o bone
            _animator.Rebind();
            _animator.Update(0f);

            // --- 5) Restaura pose correta
            t.position = posWorld;
            t.rotation = rotWorld;

            // --- 6) Reativa física com segurança
            rb.simulated = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.gravityScale = 0f;

            // --- 7) Aplica forças reais
            Vector2 direcao = UnityEngine.Random.insideUnitCircle.normalized;
            rb.AddForce(direcao * intensidade, ForceMode2D.Impulse);

            float torque = UnityEngine.Random.Range(-intensidade, intensidade);
            rb.AddTorque(torque, ForceMode2D.Impulse);

            // --- 8) Remove da lista para não tentar desmembrar de novo
            _membros.RemoveAt(indice);
        }
    }

    public async void RegenerarVida()
    {
        for (float _vidaAtual = this._vidaAtual; _vidaAtual < _vidaMax; _vidaAtual = this._vidaAtual)
        {
            _poderVitalidade._estaRegenerando = true;
            await System.Threading.Tasks.Task.Delay((int)Math.Ceiling(_poderVitalidade._intervaloCura * 1000));
            this._vidaAtual += _poderVitalidade._valorCura;
            _barraVida.AtualizarVida(_vidaMax, this._vidaAtual);
            Utilidades.InstanciarNumeroDano((_poderVitalidade._valorCura).ToString(), transform, Color.green);
            _poderVitalidade._estaRegenerando = false;
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
    
    public void Invulneravel(int _estado)
    {
        if (_estado == 0)
        {
            _invulneravel = false;
        }
        else
        {
            _invulneravel = true;
        }
    }
    public virtual IEnumerator Morte(float _tempo)
    {
        yield return null;
        GetComponent<Collider2D>().enabled = false;
        if(GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().sortingOrder = 1;
        if(_mao.GetComponent<SpriteRenderer>() != null) _mao.GetComponent<SpriteRenderer>().sortingOrder = 1;
        if (GetComponentInChildren<Canvas>() != null)
        {
            Destroy(GetComponentInChildren<Canvas>().gameObject);
        }
        Destroy(gameObject, _tempo);
    }
    IEnumerator DestruirCorpo(float _tempo)
    {
        yield return new WaitForSeconds(_tempo);
        Destroy(gameObject);
    }
    public void DefinirAtributos()
    {

        #region Força
        _poderForca._dano = Utilidades.Escala(_poderForca._nivel, 5, 1);
        _poderForca._repulsao = Utilidades.Escala(_poderForca._nivel, 7.5f, 0.75f);
        #endregion

        #region Resistência
        _poderResistencia._negacaoDano = Utilidades.Escala(_poderResistencia._nivel, 0, 1);
        _poderResistencia._negacaoRepulsao = Utilidades.Escala(_poderResistencia._nivel, 0, 0.75f);
        #endregion

        #region Vitalidade
        _poderVitalidade._acrescimoVidaMax = Utilidades.Escala(_poderVitalidade._nivel, 0, 0.5f);
        _poderVitalidade._valorCura = Utilidades.Escala(_poderVitalidade._nivel, 0, 0.01f);
        _poderVitalidade._intervaloCura = Utilidades.LimitadorNumero(0.1f, 5f, Utilidades.Escala(_poderVitalidade._nivel, 5, -0.01f));
        _poderVitalidade._rouboVida = Utilidades.Escala(_poderVitalidade._nivel, 0, 0.05f);
        _vidaMax += _poderVitalidade._acrescimoVidaMax;
        _vidaAtual = _vidaMax;
        #endregion

        #region Velocidade
        _poderVelocidade._acrescimoVelocidadeMovimento = Utilidades.LimitadorNumero(0, 30, Utilidades.Escala(_poderVelocidade._nivel, 0, 0.4f));
        _poderVelocidade._acrescimoVelocidadeAnimações = Utilidades.LimitadorNumero(0, 2, Utilidades.Escala(_poderVelocidade._nivel, 0, 0.02f));
        _poderVelocidade._reducaoTempo = Utilidades.LimitadorNumero(0.5f, 0.99f, Utilidades.Escala(_poderVelocidade._nivel, 0.5f, 0.0025f));
        _velocidadeMovimento += _poderVelocidade._acrescimoVelocidadeMovimento;
        GetComponent<Animator>().speed += _poderVelocidade._acrescimoVelocidadeAnimações;
        //_mao.GetComponent<Animator>().speed += _poderVelocidade._acrescimoVelocidadeAnimações;
        #endregion

        #region Fogo
        _poderFogo._dano = Utilidades.Escala(_poderFogo._nivel, 7.5f, 0.75f);
        _poderFogo._negacaoDano = Utilidades.Escala(_poderFogo._nivel, 0, 0.75f);
        _poderFogo._repulsao = Utilidades.Escala(_poderFogo._nivel, 7.5f, 0.75f);
        _poderFogo._negacaoRepulsao = Utilidades.Escala(_poderFogo._nivel, 0, 0.75f);
        _poderFogo._status._dano = Utilidades.Escala(_poderFogo._nivel, 0, 0.75f);
        _poderFogo._status._negacaoDano = Utilidades.Escala(_poderFogo._nivel, 0, 0.75f);
        _poderFogo._status._infligirAcumulo = Utilidades.Escala(_poderFogo._nivel, 0f, 1f);
        _poderFogo._status._acumuloMax = Utilidades.Escala(_poderFogo._nivel, 0f, 5f);
        #endregion

        #region Gelo    
        _poderGelo._dano = Utilidades.Escala(_poderGelo._nivel, 7.5f, 0.75f);
        _poderGelo._negacaoDano = Utilidades.Escala(_poderGelo._nivel, 0, 0.75f);
        _poderGelo._repulsao = Utilidades.Escala(_poderGelo._nivel, 7.5f, 0.75f);
        _poderGelo._negacaoRepulsao = Utilidades.Escala(_poderGelo._nivel, 0, 0.75f);
        _poderGelo._status._utilidade1 = Utilidades.Escala(_poderGelo._nivel, 30, 1.5f);
        _poderGelo._status._negacaoUtilidade1 = Utilidades.Escala(_poderGelo._nivel, 0, 1.5f);
        _poderGelo._status._infligirAcumulo = Utilidades.Escala(_poderGelo._nivel, 0f, 1f);
        _poderGelo._status._acumuloMax = Utilidades.Escala(_poderGelo._nivel, 0f, 5f);
        #endregion

        #region Veneno
        _poderVeneno._dano = Utilidades.Escala(_poderVeneno._nivel, 7.5f, 0.75f);
        _poderVeneno._negacaoDano = Utilidades.Escala(_poderVeneno._nivel, 0f, 0.75f);
        _poderVeneno._repulsao = Utilidades.Escala(_poderVeneno._nivel, 7.5f, 0.75f);
        _poderVeneno._negacaoRepulsao = Utilidades.Escala(_poderVeneno._nivel, 0f, 0.75f);
        _poderVeneno._status._dano = Utilidades.Escala(_poderVeneno._nivel, 0, 1f);
        _poderVeneno._status._negacaoDano = Utilidades.Escala(_poderVeneno._nivel, 0, 1f);
        _poderVeneno._status._infligirAcumulo = Utilidades.Escala(_poderVeneno._nivel, 0, 1f);
        _poderVeneno._status._acumuloMax = Utilidades.Escala(_poderVeneno._nivel, 0, 5f);
        #endregion

        #region Eletricidade
        _poderEletricidade._dano = Utilidades.Escala(_poderEletricidade._nivel, 7.5f, 0.75f);
        _poderEletricidade._negacaoDano = Utilidades.Escala(_poderEletricidade._nivel, 0, 0.75f);
        _poderEletricidade._repulsao = Utilidades.Escala(_poderEletricidade._nivel, 7.5f, 0.75f);
        _poderEletricidade._negacaoRepulsao = Utilidades.Escala(_poderEletricidade._nivel, 0, 0.75f);
        _poderEletricidade._status._dano = Utilidades.Escala(_poderEletricidade._nivel, 3.5f, 0.35f);
        _poderEletricidade._status._negacaoDano = Utilidades.Escala(_poderEletricidade._nivel, 0, 0.35f);
        _poderEletricidade._status._utilidade1 = Utilidades.Escala(_poderEletricidade._nivel, 1, 0.28f);
        _poderEletricidade._status._negacaoUtilidade1 = Utilidades.Escala(_poderEletricidade._nivel, 0, 0.28f);
        _poderEletricidade._status._infligirAcumulo = Utilidades.Escala(_poderEletricidade._nivel, 0, 1f);
        _poderEletricidade._status._acumuloMax = Utilidades.Escala(_poderEletricidade._nivel, 0, 5f);
        #endregion

    }
    public void DefinirNivelGeral()
    {
        _nivelGeral = _poderForca._nivel +
                    _poderResistencia._nivel +
                    _poderVitalidade._nivel +
                    _poderVelocidade._nivel +
                    _poderFogo._nivel +
                    _poderGelo._nivel +
                    _poderVeneno._nivel +
                    _poderEletricidade._nivel;
    }
    public void IniciarSom(int _numeroNaLista)
    {
        _sons[_numeroNaLista].Play();
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
    public void SalvarDadosPrefab(string _caminhoPrefab)
    {
        Ser_Vivo _prefab = Resources.Load<GameObject>(_caminhoPrefab).GetComponent<Ser_Vivo>();
        Utilidades.CopiarPropriedades(GetComponent<Ser_Vivo>(), _prefab);
    }
    public void InstanciarAtaque(int indiceAtaque)
    {
        _mao.GetComponent<Mao>().InstanciarAtaque(indiceAtaque);
    }
    public void AplicarDano(float _danoSofrido)
    {
        _vidaAtual = _vidaAtual - _danoSofrido > 0
                ? _vidaAtual -= _danoSofrido
                : _vidaAtual = 0;
        if( _vidaAtual <= 0 ) 
        {
            //Debug.Log("Morreu");
            //TravarCorpoMao(1);
            Desmembrar((int)Math.Clamp(_danoSofrido / _vidaMax * _membros.Count, 1, _membros.Count), Math.Clamp(_danoSofrido / _vidaMax * 30, 3, 30));
            InterromperEfeitos();
            //_mao.GetComponent<Mao>().ResetarMao();
            //_animator.enabled = false;
            //Debug.Log($"Quantidade de membros a soltar: {(int)Math.Clamp(_danoSofrido / _vidaMax * _membros.Count, 1, _membros.Count)}");
            _animator.SetTrigger("Morrer");
            StartCoroutine(Morte(5f));
        }
        else 
        {
            AnimacaoDanoSofrido(_danoSofrido / _vidaMax);
        }
    }

    void InterromperEfeitos()
    {
        foreach(var efeito in GetComponentsInChildren<ParticleSystem>())
        {
            Destroy(efeito.gameObject);
        }
    }
    
}
