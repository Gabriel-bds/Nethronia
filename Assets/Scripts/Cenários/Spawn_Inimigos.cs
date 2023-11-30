using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Inimigos : MonoBehaviour
{
    [SerializeField] GameObject[] _inimigos;
    [SerializeField] float _tamanhoArea;
    [SerializeField] int _qtdSpawns;

    private void Awake()
    {
        SpawnarInimigos();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _tamanhoArea);
    }
    void SpawnarInimigos()
    {
        for (int n = 0; n < _qtdSpawns; n++)
        {
            Instantiate(_inimigos[Random.Range(0, _inimigos.Length)], SortearArea(), transform.localRotation);
        }
    }

    Vector2 SortearArea()
    {
        return transform.position + Random.onUnitSphere * Random.Range(1, _tamanhoArea);
    }
}
