using UnityEngine;

public class GhostSprite : MonoBehaviour
{
    private SpriteRenderer _sr;
    private float _tempoVida;
    private float _tempoAtual;
    private float _alphaInicial;

    public void Inicializar(SpriteRenderer sr, float tempoVida)
    {
        _sr = sr;
        _tempoVida = tempoVida;
        _alphaInicial = sr.color.a;
    }

    void Update()
    {
        _tempoAtual += Time.deltaTime;

        float t = _tempoAtual / _tempoVida;
        float alpha = Mathf.Lerp(_alphaInicial, 0f, t);

        Color c = _sr.color;
        c.a = alpha;
        _sr.color = c;

        if (_tempoAtual >= _tempoVida)
        {
            Destroy(gameObject);
        }
    }
}
