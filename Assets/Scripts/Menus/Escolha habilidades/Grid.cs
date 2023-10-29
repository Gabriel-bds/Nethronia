using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int _molduraChamada;
    public void DefinirMoldura(int _numeroMoldura)
    {
        _molduraChamada = _numeroMoldura;
        foreach(Moldura _moldura in GetComponentsInChildren<Moldura>())
        {
            _moldura._numeroMoldura = _numeroMoldura;
        }
    }
}
