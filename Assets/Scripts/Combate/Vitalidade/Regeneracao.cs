using System.Collections;
using UnityEngine;

public class Regeneracao
{
    private Ser_Vivo _alvo;
    private float _valorPorTick;
    private float _intervalo;
    private MonoBehaviour _executor;
    private Coroutine _coroutine;

    public Regeneracao(Ser_Vivo alvo, float valorPorTick, float intervalo, MonoBehaviour executor)
    {
        _alvo = alvo;
        _valorPorTick = valorPorTick;
        _intervalo = intervalo;
        _executor = executor;
    }

    public void Iniciar()
    {
        if (_coroutine != null)
            return;

        _coroutine = _executor.StartCoroutine(Regenerar());
    }

    public void Parar()
    {
        if (_coroutine != null)
        {
            _executor.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Regenerar()
    {
        while (true)
        {
            yield return new WaitForSeconds(_intervalo);

            if (_alvo.VidaAtual < _alvo._vidaMax)
            {
                _alvo.VidaAtual += _valorPorTick;
                Utilidades.InstanciarNumeroDano(_valorPorTick.ToString(), _alvo.transform, Color.green);
            }
        }
    }
}