using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Inimigos : MonoBehaviour
{
    CircleCollider2D _area;
    [SerializeField] GameObject[] _inimigos;
    [SerializeField] int _quantidade;
    // Start is called before the first frame update
    private void Awake()
    {
        _area = GetComponent<CircleCollider2D>();
        SpawnarInimigos();
    }
    void SpawnarInimigos()
    {
        for(int n = 0; n < _quantidade; n++)
        {
            Instantiate(_inimigos[Random.Range(0, _inimigos.Length)], SortearArea(_area), transform.localRotation);
        }
    }
    Vector2 SortearArea(CircleCollider2D _area)
    {
        return new Vector2(Random.Range(_area.bounds.min.x * _area.radius, _area.bounds.max.x * _area.radius),
                           Random.Range(_area.bounds.min.y * _area.radius, _area.bounds.max.y * _area.radius));
    }
}
