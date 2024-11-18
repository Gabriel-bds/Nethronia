using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Numero_BarraXP : MonoBehaviour
{   
    void Start()
    {
        AtualizarNumero();
    }
    public void AtualizarNumero()
    {
        Player _player = FindAnyObjectByType<Player>();
        GetComponent<TextMeshProUGUI>().text = string.Format("{0}/{1}", _player._experiencia, _player._experienciaParaProximoNivel);
    }
}
