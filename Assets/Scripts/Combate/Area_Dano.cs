using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area_Dano : Ataque
{
    [SerializeField] float _delayInicialArea;
    protected override void Start()
    {
        StartCoroutine(DelayInicial());
        base.Start();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    IEnumerator DelayInicial()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(_delayInicialArea);
        GetComponent<Collider2D>().enabled = true;
    }
}
