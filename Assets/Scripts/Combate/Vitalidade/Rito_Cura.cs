using System;
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
        //base.Start();
        ControlarRecarga();
        _valorPorTick = _dono._poderVitalidade._valorCura;
        _intervalo = _dono._poderVitalidade._intervaloCura;
        _regeneracao = new Regeneracao(_dono, _valorPorTick, _intervalo, this);
        _tempoDeVida = Mathf.Clamp(Utilidades.Escala(_dono._poderVitalidade._nivel, 7f, 0.23f), 7f, 30f);
        _ativo = true;
        StartCoroutine(ControleDuracao());
        ControlarEscalaVisualVitalidade();
        gameObject.layer = default;
    }

    private void ControlarEscalaVisualVitalidade()
    {
        if (GetComponent<Animator>() != null)
        {
            float forca = Utilidades.LimitadorNumero(0, 1,
                (float)_dono._poderVitalidade._nivel / nivelMaximoMagnitudeVisual);
            GetComponent<Animator>().SetFloat("Forca", forca);
        }
    }

    private IEnumerator ControleDuracao()
    {
        Debug.Log(_tempoDeVida);
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
        foreach(ParticleSystem particula in GetComponentsInChildren<ParticleSystem>())
        {
            particula.Stop();
        }
        _ativo = false;
        Destroy(gameObject, 3f);
    }

    private void OnDestroy()
    {
        _regeneracao?.Parar();
    }
}