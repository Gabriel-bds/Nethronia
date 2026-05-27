using UnityEngine;

[System.Serializable]
public struct EscalaValor
{
    [SerializeField] private float _valorMinimo;
    [SerializeField] private float _valorMaximo;
    [SerializeField] private int _nivelMaximo;

    public float Avaliar(int nivel)
    {
        if (_nivelMaximo <= 0) return _valorMinimo;
        return Mathf.Lerp(_valorMinimo, _valorMaximo, Mathf.Clamp01((float)nivel / _nivelMaximo));
    }
}
