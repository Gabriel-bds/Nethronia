using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controle_Status : MonoBehaviour
{
    public int _addNivel;
    public Tipo_Poder _tipoPoder;
    public Poder _poder;

    private void Start()
    {
        if (GetComponent<TextMeshProUGUI>() != null)
        {
            AtualizarTexto();
        }
    }
    public void AumentarNivel()
    {
        Controle_PontosHabilidade _controle = FindAnyObjectByType<Controle_PontosHabilidade>();
        if (_controle._pontosDisponiveis > 0)
        {
            _addNivel += 1;
            _controle.AlterarValorPontos(-1);
        }
    }
    public void DiminuirNivel()
    {
        Controle_PontosHabilidade _controle = FindAnyObjectByType<Controle_PontosHabilidade>();
        if(_controle._pontosDisponiveis < FindAnyObjectByType<Player>()._pontosHabilidade && _addNivel > 0)
        {
            _addNivel = (int)Utilidades.ArredondarNegativo(_addNivel - 1);
            _controle.AlterarValorPontos(1);
        }
    }
    public void AtualizarTexto()
    {
        Player _player = FindAnyObjectByType<Player>();
        TextMeshProUGUI _texto = GetComponent<TextMeshProUGUI>();
        switch (_tipoPoder)
        {
            case Tipo_Poder.Forca:
                _texto.text = $"Força: {_player._poderForca._nivel + _addNivel}";
                break;

            case Tipo_Poder.Resistencia:
                _texto.text = $"Resistência: {_player._poderResistencia._nivel + _addNivel}";
                break;

            case Tipo_Poder.Vitalidade:
                _texto.text = $"Vitalidade: {_player._poderVitalidade._nivel + _addNivel}";
                break;

            case Tipo_Poder.Velocidade:
                _texto.text = $"Velocidade: {_player._poderVelocidade._nivel + _addNivel}";
                break;

            case Tipo_Poder.Fogo:
                _texto.text = $"Fogo: {_player._poderFogo._nivel + _addNivel}";
                break;

            case Tipo_Poder.Gelo:
                _texto.text = $"Gelo: {_player._poderGelo._nivel + _addNivel}";
                break;

            case Tipo_Poder.Veneno:
                _texto.text = $"Veneno: {_player._poderVeneno._nivel + _addNivel}";
                break;

            case Tipo_Poder.Eletricidade:
                _texto.text = $"Eletricidade: {_player._poderEletricidade._nivel + _addNivel}";
                break;
        }
    }
    public void AplicarNiveis()
    {
        Player _player = FindAnyObjectByType<Player>();
        Controle_Status[] _controlesStatus = FindObjectsOfType<Controle_Status>();
        foreach(Controle_Status _controle in _controlesStatus)
        {
            switch (_controle._tipoPoder) 
            {
                case Tipo_Poder.Forca:
                    _player._poderForca._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Resistencia:
                    _player._poderResistencia._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Vitalidade:
                    _player._poderVitalidade._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Velocidade:
                    _player._poderVelocidade._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Fogo:
                    _player._poderFogo._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Gelo:
                    _player._poderGelo._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Veneno:
                    _player._poderVeneno._nivel += _controle._addNivel;
                    break;

                case Tipo_Poder.Eletricidade:
                    _player._poderEletricidade._nivel += _controle._addNivel;
                    break;
            }
            _controle._addNivel = 0;
        }
        _player.DefinirNivelGeral();
        _player.DefinirAtributos();
    }
}
