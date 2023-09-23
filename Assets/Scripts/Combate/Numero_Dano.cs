using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Numero_Dano : MonoBehaviour
{
    float _velocidadeX;
    float _velocidadeY;
    List<int> _direcaoYLista = new List<int>();
    int _direcaoY;
    float _diminuicaoEscala;
    [SerializeField] int _orderInLayer;
    [HideInInspector] public TextMeshPro _texto;
    [HideInInspector] public string _novoTexto;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        _texto = GetComponent<TextMeshPro>();
        _texto.sortingOrder = _orderInLayer;
        _texto.text = _novoTexto;
        _velocidadeX = Random.RandomRange(1f, 2f);
        _velocidadeY = Random.RandomRange(1f, 2f);
        _diminuicaoEscala = Random.Range(0.5f, 1f);
        _direcaoYLista.Add(1);
        _direcaoYLista.Add(-1);
        _direcaoY = _direcaoYLista[Random.Range(0, 2)];
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x > 0)
        {
            transform.position = new Vector2(transform.position.x + _velocidadeX * Time.deltaTime,
                                        transform.position.y + _velocidadeY * _direcaoY * Time.deltaTime);
            transform.localScale = new Vector2(transform.localScale.x - _diminuicaoEscala * Time.deltaTime,
                                                transform.localScale.y - _diminuicaoEscala * Time.deltaTime);
        }
        else{
            Destroy(gameObject);
        }
        

    }
}
