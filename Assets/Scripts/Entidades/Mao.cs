using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mao : MonoBehaviour
{
    [SerializeField] Ser_Vivo _dono;
    public float _latenciaMira;
    [SerializeField] protected GameObject _alvo;
    public List<GameObject> _ataques = new List<GameObject>();
    public List<GameObject> _ataquesDisponiveis = new List<GameObject>();
    [HideInInspector] public bool _mirandoAlvo;
    [SerializeField] public Transform[] _spawnsAtaques;
    public int _travar;
    [SerializeField] List<AudioSource> _sons = new List<AudioSource>();
    // Start is called before the first frame update
    void Start()
    {
        _dono = GetComponentInParent<Ser_Vivo>();
        foreach(GameObject _obj in _ataques)
        {
            _ataquesDisponiveis.Add(_obj);
            _obj.GetComponent<Ataque>()._dono = _dono;
        }
    }

    // Update is called once per frame
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
        Quaternion _novaRotacao = Quaternion.Lerp(_rotacaoAtual, _rotacaoAlvo, _latenciaMira * Time.fixedDeltaTime);
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
        Ataque _atq = new Ataque();
        int _indiceAtq = 0;
        foreach (GameObject o in _ataques)
        {
            if (o.GetComponent<Ataque>()._idAtaque == _numeroAtaque)
            {
                _atq = Instantiate(_ataques[_indiceAtq], _spawnsAtaques[_indiceAtq].position, _spawnsAtaques[_indiceAtq].rotation).GetComponent<Ataque>();
            }
            _indiceAtq += 1;
        }
        _atq.gameObject.transform.localScale = _dono.transform.localScale;
        StartCoroutine(ResetarAtaque(_numeroAtaque, _atq._tempoRecarga));
    }
    public void TravarMao(int _travarMao)
    {
        _travar = _travarMao;
        
    }
    public void ResetarMao()
    {
        transform.rotation = new Quaternion(0,0,0,0);
        GetComponent<SpriteRenderer>().flipX = _dono.GetComponent<SpriteRenderer>().flipX;
        GetComponent<SpriteRenderer>().flipY = _dono.GetComponent<SpriteRenderer>().flipY;
    }
    IEnumerator ResetarAtaque(int _numeroAtaque, float _tempoReset)
    {
        int _indiceAtq = 0;
        foreach (GameObject o in _ataques)
        {
            if (o.GetComponent<Ataque>()._idAtaque == _numeroAtaque)
            {
                _ataquesDisponiveis.Remove(o);
                yield return new WaitForSeconds(_tempoReset);
                _ataquesDisponiveis.Add(o);
            }
            _indiceAtq += 1;
        }
    }
    public void IniciarSom(int _numeroNaLista)
    {
        _sons[_numeroNaLista].Play();
    }
}
