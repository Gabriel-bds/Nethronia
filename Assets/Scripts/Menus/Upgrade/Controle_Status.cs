using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controle_Status : MonoBehaviour
{
    public int _addNivel;
    public Tipo_Poder _tipoPoder;

    public void AumentarNIvel()
    {
        _addNivel += 1;
    }
    public void DiminuirNivel()
    {
        _addNivel = (int)Utilidades.ArredondarNegativo(_addNivel - 1);
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
        _player.DefinirAtributos();
    }
}
