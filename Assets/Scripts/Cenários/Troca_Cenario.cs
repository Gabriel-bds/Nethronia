using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Troca_Cenario : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            FindAnyObjectByType<Controle_Cenarios>().SortearCenario();
            FindAnyObjectByType<Player>().SalvarDadosPrefab("Prefabs/Entidades/Player");
        }
    }
}
