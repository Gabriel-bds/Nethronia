using System.Collections;
using UnityEngine;

public class RitoDeCura : Ataque
{
    [Header("Rito Cura")]
    [SerializeField] private float _valorPorTick = 2f;
    [SerializeField] private float _intervalo = 1f;
    private Regeneracao _regeneracao;
    private bool _ativo;

    private void Start()
    {

        _regeneracao = new Regeneracao(_dono, _valorPorTick, _intervalo, this);

        _dono.OnVidaAlterada += VerificarRegeneracao;

        StartCoroutine(ControleDuracao());
    }

    private IEnumerator ControleDuracao()
    {
        _ativo = true;

        yield return new WaitForSeconds(_tempoDeVida);

        EncerrarRito();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_ativo) return;

        if (other.GetComponent<Ser_Vivo>() == _dono)
        {
            if (_dono.VidaAtual < _dono._vidaMax)
                _regeneracao.Iniciar();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Ser_Vivo>() == _dono)
        {
            _regeneracao.Parar();
        }
    }

    private void VerificarRegeneracao(Ser_Vivo sv, float vidaAtual, float vidaMax)
    {
        if (!_ativo)
            return;

        if (vidaAtual < vidaMax)
            _regeneracao.Iniciar();
        else
            _regeneracao.Parar();
    }

    private void EncerrarRito()
    {
        _ativo = false;

        _regeneracao.Parar();

        if (_dono != null)
            _dono.OnVidaAlterada -= VerificarRegeneracao;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_dono != null)
            _dono.OnVidaAlterada -= VerificarRegeneracao;

        _regeneracao?.Parar();
    }
}