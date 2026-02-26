using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Numero_BarraVida : MonoBehaviour
{
    void Awake()
    {

        FindAnyObjectByType<Player>().OnVidaAlterada += AtualizarNumero;
    }
    public void AtualizarNumero(Ser_Vivo dono, float vidaAtual, float vidaMax)
    {
        Player _player = FindAnyObjectByType<Player>();
        GetComponent<TextMeshProUGUI>().text = string.Format("{0}/{1}", vidaAtual, vidaMax);
    }
}
