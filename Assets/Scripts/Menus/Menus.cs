using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menus : MonoBehaviour
{
    [SerializeField] GameObject _menuPersonagem;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(_menuPersonagem.activeInHierarchy) 
            { 
                _menuPersonagem.SetActive(false);
            }
            else
            {
                _menuPersonagem.SetActive(true);
            }
        }
    }
}
