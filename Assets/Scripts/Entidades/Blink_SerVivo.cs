using System.Collections;
using UnityEngine;

public class Blink_SerVivo : MonoBehaviour
{
    [SerializeField] Material _materialBlink;
    [SerializeField] float _duracaoBlink = 0.08f;

    Ser_Vivo _dono;
    bool _emBlink;

    void Start()
    {
        _dono = GetComponentInParent<Ser_Vivo>();
        _dono.OnDanoRecebido += AcionarBlink;
    }

    void OnDestroy()
    {
        if (_dono != null)
            _dono.OnDanoRecebido -= AcionarBlink;
    }

    void AcionarBlink(float intensidade)
    {
        if (_emBlink) return;
        StartCoroutine(ExecutarBlink());
    }

    IEnumerator ExecutarBlink()
    {
        _emBlink = true;

        SpriteRenderer[] membros = _dono.GetComponentsInChildren<SpriteRenderer>();
        Material[] materiaisOriginais = new Material[membros.Length];

        for (int indice = 0; indice < membros.Length; indice++)
        {
            materiaisOriginais[indice] = membros[indice].material;
            membros[indice].material = _materialBlink;
        }

        yield return new WaitForSecondsRealtime(_duracaoBlink);

        for (int indice = 0; indice < membros.Length; indice++)
        {
            if (membros[indice] != null)
                membros[indice].material = materiaisOriginais[indice];
        }

        _emBlink = false;
    }
}
