using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class Mao : MonoBehaviour
{
    [SerializeField] Ser_Vivo _dono;
    public float _latenciaMira;
    [SerializeField] protected GameObject _alvo;
    public List<GameObject> _ataques = new List<GameObject>();
     public List<GameObject> _ataquesDisponiveis = new List<GameObject>();
     public bool _mirandoAlvo;
    public int _travar;
    [HideInInspector] public Animator _animator;
    [SerializeField] List<AudioSource> _sons = new List<AudioSource>();
    GameObject _ultimoAtq;
    void Start()
    {
        _dono = GetComponentInParent<Ser_Vivo>();
        _alvo = _dono._alvo;
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        ControlarMao();
    }
    private void ApontarMao()
    {
        Vector3 _posicaoAlvo;
        if (_alvo == null)
        {
            _posicaoAlvo = Input.mousePosition;
        }
        else
        {
            _posicaoAlvo = Camera.main.WorldToScreenPoint(_alvo.transform.position);
        }

        Vector3 _posicaoMao = Camera.main.WorldToScreenPoint(transform.position);

        Vector3 _distancia = new Vector3(_posicaoAlvo.x - _posicaoMao.x, _posicaoAlvo.y - _posicaoMao.y);

        float _angulo = Mathf.Atan2(_distancia.y, _distancia.x) * Mathf.Rad2Deg;

        Quaternion _rotacaoAtual = transform.rotation;
        Quaternion _rotacaoAlvo = Quaternion.Euler(transform.rotation.x, transform.rotation.y, _angulo);
        Quaternion _novaRotacao = Quaternion.Lerp(_rotacaoAtual, _rotacaoAlvo, _latenciaMira * Time.deltaTime);
        transform.rotation = _novaRotacao;

        if(Math.Round(Mathf.Abs(transform.rotation.x), 1) == Math.Round(Mathf.Abs(_rotacaoAlvo.x), 1) &&
            Math.Round(Mathf.Abs(transform.rotation.y), 1) == Math.Round(Mathf.Abs(_rotacaoAlvo.y), 1) &&
            Math.Round(Mathf.Abs(transform.rotation.w), 1) == Math.Round(Mathf.Abs(_rotacaoAlvo.w), 1) &&
            Math.Round(Mathf.Abs(transform.rotation.z), 1) == Math.Round(Mathf.Abs(_rotacaoAlvo.z), 1))
        {
            _mirandoAlvo = true;
        }
        else
        {
            _mirandoAlvo = false;
        }
    }
    private void ControlarMao()
    {
        if (_travar == 0)
        {
            ApontarMao();
        }
    }
    public void InstanciarAtaque(int _numeroAtaque)
    {
        int _indiceAtq = 0;
        foreach (GameObject o in _ataques)
        {
            if (o.GetComponent<Ataque>()._idAtaque == _numeroAtaque)
            {
                Ataque _atq = Instantiate(_ataques[_indiceAtq], _ataques[_indiceAtq].GetComponent<Ataque>()._spawnPosicao, _ataques[_indiceAtq].GetComponent<Ataque>()._spawnRotacao).GetComponent<Ataque>();
                _atq._dono = _dono;
                _atq.gameObject.transform.localScale = _dono.transform.localScale;
                _atq.DefinirSpawn();
                _ultimoAtq = _atq.gameObject;
                /*if(_atq.gameObject.GetComponent<Rajada>() == null)
                {
                    _ataquesDisponiveis.Remove(o);
                    Ataque.QuadroDoAtaque(_dono.gameObject, o)._recargaAtual = 0; // funciona tanto p/ player quanto inimigo
                    StartCoroutine(RecarregarAtaque(o.GetComponent<Ataque>()._tempoRecargaTotal, o)); // <- chama a coroutine/async
                    //Debug.Log("Recarregou");
                    if (_dono.GetComponent<Player>() != null)
                    {
                        Ataque.QuadroDoAtaque(_dono.gameObject ,o)._recargaAtual = 0;
                    }
                }*/
                break;
            }
            _indiceAtq += 1;
        }
        //Debug.Log("Ataques disponíveis: " + _ataquesDisponiveis.Count);
    }
    public void ForcarDestruicaoAtaque(float _tempo)
    {
        Destroy(_ultimoAtq, _tempo);
        if(_ultimoAtq.GetComponent<Rajada>() != null)
        {
            _ultimoAtq.GetComponent<Rajada>().PararRajada();
        }
    }
    public void ResetarMao()
    {
        transform.rotation *= new Quaternion(1, 1, 0, 0);
        GetComponent<SpriteRenderer>().flipX = _dono.GetComponent<SpriteRenderer>().flipX;
        GetComponent<SpriteRenderer>().flipY = _dono.GetComponent<SpriteRenderer>().flipY;
    }
    public void AtualizarAtaque(int _index, GameObject _ataqueNovo)
    {
        _ataqueNovo.GetComponent<Ataque>()._dono = _dono;
        _ataques[_index] = _ataqueNovo;
    }
    public void IniciarSom(int _numeroNaLista)
    {
        _sons[_numeroNaLista].Play();
    }
    public void ControleVelocidadeAnimacao(float _velocidadeAnimacao)
    {
        GetComponent<Animator>().speed = _velocidadeAnimacao;
    }
    public IEnumerator RecarregarAtaque(float tempo, GameObject ataque)
    {
        Debug.Log("Chamou recarga");
        if (_ataquesDisponiveis.Contains(ataque))
        {
            _ataquesDisponiveis.Remove(ataque);
        }
        if (_dono.GetComponent<Player>() != null)
        {
            var quadro = Ataque.QuadroDoAtaque(_dono.gameObject, ataque);
            if (quadro != null)
                StartCoroutine(quadro.CarregarHabilidade(tempo));
        }

        yield return new WaitForSeconds(tempo);

        _ataquesDisponiveis.Add(ataque);
    }

    public void RecarregarTodosAtaques()
    {
        foreach(GameObject ataque in _ataques)
        {
            StartCoroutine(RecarregarAtaque(ataque.GetComponent<Ataque>()._tempoRecargaTotal, ataque));
            //Debug.Log("Começou");
        }
    }

    public void RemoverAtaqueDisponivel(GameObject ataque)
    {
        //Debug.Log("Atauqe removido");
        foreach(GameObject habilidade in _ataquesDisponiveis)
        {
            if(habilidade.GetComponent<Ataque>()._idAtaque == ataque.GetComponent<Ataque>()._idAtaque)
            {
                _ataquesDisponiveis.Remove(habilidade);
                if (_dono.GetComponent<Player>() != null)
                {
                    var quadro = Ataque.QuadroDoAtaque(_dono.gameObject, habilidade);
                    if (quadro != null)
                        quadro._recargaAtual = 0;
                }
                return;
            }
        }
        
    }
}
