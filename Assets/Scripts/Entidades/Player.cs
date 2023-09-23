using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player : Ser_Vivo
{
    protected override void Update()
    {
        base.Update();
        Atacar();
    }
    private void FixedUpdate()
    {
        Mover(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void Atacar()
    {
        _mao.GetComponent<Animator>().SetInteger("Ataque", 0);
        if (Input.GetMouseButtonDown(0))
        {
            if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Count != 0)
            {
                _mao.GetComponent<Animator>().SetInteger("Ataque", 1);
            }

        }
    }
}
