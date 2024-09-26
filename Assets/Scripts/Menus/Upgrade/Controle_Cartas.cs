using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controle_Cartas : MonoBehaviour
{
    public List<GameObject> _todasCartas;
    public List<GameObject> _cartasDisponiveis;
    public int _quantidadeCartas;

    private void Awake()
    {
        _cartasDisponiveis = new List<GameObject>(_todasCartas);
    }
    private void OnEnable()
    {
        SortearCartas();
    }
    public void SortearCartas()
    {
        for(int index = 0; index < 4; index++) 
        {
            GameObject _cartaAtual = Instantiate(_cartasDisponiveis[Random.Range(0, _cartasDisponiveis.Count - 1)], transform);
            List<GameObject> _novaLista = new List<GameObject>(_cartasDisponiveis);
            foreach(GameObject _carta in _cartasDisponiveis)
            {
                if(_carta.GetComponent<Carta>()._atributos == _cartaAtual.GetComponent<Carta>()._atributos)
                {
                    _novaLista.Remove(_carta);
                }
            }
            _cartasDisponiveis = new List<GameObject>(_novaLista);
        }
    }
    public static void DestruirCartas()
    {
        Carta[] _cartas = FindObjectsOfType<Carta>();
        foreach(Carta _carta in _cartas)
        {
            Destroy(_carta.gameObject);
        }
    }

}
