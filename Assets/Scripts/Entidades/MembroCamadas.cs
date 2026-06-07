using UnityEngine;

public class MembroCamadas : MonoBehaviour
{
    [SerializeField] private Sprite _esqueleto;
    [SerializeField] private Sprite _musculo;
    [SerializeField] private float _tempoTransicao = 0.1f;

    private SpriteRenderer _renderer;
    private MaterialPropertyBlock _bloco;
    private float _intensidadeAtual = 1f;
    private float _intensidadeAlvo  = 1f;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _bloco = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_bloco);
        if (_esqueleto != null)
            _bloco.SetTexture("_EsqueletoTex", _esqueleto.texture);
        if (_musculo != null)
            _bloco.SetTexture("_MusculoTex", _musculo.texture);
        _bloco.SetFloat("_Intensidade", 1f);
        _renderer.SetPropertyBlock(_bloco);
    }

    private void Update()
    {
        if (_intensidadeAtual == _intensidadeAlvo) return;

        float passo = _tempoTransicao > 0f ? Time.deltaTime / _tempoTransicao : 1f;
        _intensidadeAtual = Mathf.MoveTowards(_intensidadeAtual, _intensidadeAlvo, passo);

        _renderer.GetPropertyBlock(_bloco);
        _bloco.SetFloat("_Intensidade", _intensidadeAtual);
        _renderer.SetPropertyBlock(_bloco);
    }

    public void DefinirDano(float valor)
    {
        _intensidadeAlvo = 1f - Mathf.Clamp01(valor);
    }
}
