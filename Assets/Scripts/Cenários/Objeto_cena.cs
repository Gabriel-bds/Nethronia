using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto_cena : MonoBehaviour
{
    [SerializeField] Color _corContato;
    bool _estaDentro = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            _estaDentro = true;
            StartCoroutine(AtualizarCor(2f, 0.03f));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        { 
            _estaDentro = false;
            StartCoroutine(AtualizarCor(2f, 0.03f));      
        }
    }
    IEnumerator AtualizarCor(float _tempo, float _taxa)
    {
        Color _corInicial = GetComponent<SpriteRenderer>().color;
        Color _novaCor;
        float _tempoCorrido = 0;
        while (_tempoCorrido < _tempo)
        {
            if (_estaDentro)
            {
                _novaCor = _corContato;
            }
            else
            {
                _novaCor = Color.white;
            }
            _tempoCorrido += Time.deltaTime;
            Color _corAtual = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(Mathf.Lerp(_corAtual.r, _novaCor.r, _taxa / _tempo), _novaCor.g, _novaCor.b, _novaCor.a);
            GetComponent<SpriteRenderer>().color = new Color(_novaCor.r, Mathf.Lerp(_corAtual.g, _novaCor.g, _taxa / _tempo), _novaCor.b, _novaCor.a);
            GetComponent<SpriteRenderer>().color = new Color(_novaCor.r, _novaCor.g, Mathf.Lerp(_corAtual.b, _novaCor.b, _taxa / _tempo), _novaCor.a);
            GetComponent<SpriteRenderer>().color = new Color(_novaCor.r, _novaCor.g, _novaCor.b, Mathf.Lerp(_corAtual.a, _novaCor.a, _taxa / _tempo));
            yield return null;
        }
    }
}
