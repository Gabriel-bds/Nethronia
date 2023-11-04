using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controle_NivelGeral : MonoBehaviour
{
    void Start()
    {
        AtualizarTexto();
    }

    public void AtualizarTexto()
    {
        TextMeshProUGUI _texto = GetComponent<TextMeshProUGUI>();
        _texto.text = $"Nivel geral: {FindAnyObjectByType<Player>()._nivelGeral}";
    }
}
