using UnityEngine;
using FMODUnity;

public class Som_SerVivo : MonoBehaviour
{
    [SerializeField] string _tagSomDanoRecebido;

    Ser_Vivo _dono;

    void Start()
    {
        _dono = GetComponentInParent<Ser_Vivo>();
        _dono.OnDanoRecebido += TocarSomDanoRecebido;
    }

    void OnDestroy()
    {
        if (_dono != null)
            _dono.OnDanoRecebido -= TocarSomDanoRecebido;
    }

    void TocarSomDanoRecebido(float intensidade)
    {
        if (string.IsNullOrEmpty(_tagSomDanoRecebido)) return;
        try
        {
            var instancia = RuntimeManager.CreateInstance(_tagSomDanoRecebido);
            instancia.setParameterByName("intensidadeDanoRecebido", Mathf.Clamp01(intensidade));
            instancia.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            instancia.start();
            instancia.release();
        }
        catch { }
    }
}
