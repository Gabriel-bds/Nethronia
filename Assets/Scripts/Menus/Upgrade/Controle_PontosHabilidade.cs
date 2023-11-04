using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controle_PontosHabilidade : MonoBehaviour
{
    public int _pontosDisponiveis;
    private void OnEnable()
    {
        _pontosDisponiveis = FindAnyObjectByType<Player>()._pontosHabilidade;
        AtualizarTexto();
    }
    public void AlterarValorPontos(int _valor)
    {
        _pontosDisponiveis += _valor;
        AtualizarTexto();
    }
    void AtualizarTexto()
    {
        TextMeshProUGUI _texto = GetComponent<TextMeshProUGUI>();
        _texto.text = $"Pontos disponíveis: {_pontosDisponiveis}";
    }
    public void Aplicar()
    {
        FindAnyObjectByType<Player>()._pontosHabilidade = _pontosDisponiveis;
    }
}
