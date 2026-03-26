using System.Collections;
using UnityEngine;

public class RitoDeCura : Ataque
{
    [Header("Rito Cura")]
    [SerializeField] private float _valorPorTick = 2f;
    [SerializeField] private float _intervalo = 1f;

    private Regeneracao _regeneracao;
    private bool _ativo;

    protected override void Start()
    {
        base.Start();
        _regeneracao = new Regeneracao(_dono, _valorPorTick, _intervalo, this);
        _ativo = true;
        StartCoroutine(ControleDuracao());
    }

    private IEnumerator ControleDuracao()
    {
        yield return new WaitForSeconds(_tempoDeVida);
        EncerrarRito();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_ativo) return;
        if (other.GetComponent<Ser_Vivo>() == _dono)
        {
            if (_dono.VidaAtual < _dono._vidaMax)
                _regeneracao.Iniciar();
            else
                _regeneracao.Parar();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Ser_Vivo>() == _dono)
            _regeneracao.Parar();
    }

    private void EncerrarRito()
    {
        _ativo = false;
        _regeneracao.Parar();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _regeneracao?.Parar();
    }
}