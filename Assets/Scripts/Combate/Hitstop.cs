using System.Collections;
using UnityEngine;

public class Hitstop : MonoBehaviour
{
    [SerializeField] float _duracaoMinima;
    [SerializeField] float _duracaoMaxima;
    [Range(0f, 1f)] [SerializeField] float _reducaoVelocidadeMinima;
    [Range(0f, 1f)] [SerializeField] float _reducaoVelocidadeMaxima;

    bool _emExecucao;

    public void Aplicar(float porcentagemDano)
    {
        if (_emExecucao) return;

        float fator = Mathf.Clamp01(porcentagemDano);
        float duracao = Mathf.Lerp(_duracaoMinima, _duracaoMaxima, fator);
        float reducaoVelocidade = Mathf.Lerp(_reducaoVelocidadeMinima, _reducaoVelocidadeMaxima, fator);

        StartCoroutine(ExecutarHitstop(duracao, reducaoVelocidade));
    }

    IEnumerator ExecutarHitstop(float duracao, float reducaoVelocidade)
    {
        _emExecucao = true;
        float escalaTempoAnterior = Time.timeScale;
        float deltaFixoAnterior = Time.fixedDeltaTime;

        Time.timeScale = 1f - reducaoVelocidade;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(duracao);

        Time.timeScale = escalaTempoAnterior;
        Time.fixedDeltaTime = deltaFixoAnterior;
        _emExecucao = false;
    }
}
