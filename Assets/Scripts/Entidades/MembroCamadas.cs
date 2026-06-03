using UnityEngine;

public class MembroCamadas : MonoBehaviour
{
    [SerializeField] private Sprite _esqueleto;

    private SpriteRenderer _renderer;
    private MaterialPropertyBlock _bloco;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _bloco = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_bloco);
        if (_esqueleto != null)
            _bloco.SetTexture("_EsqueletoTex", _esqueleto.texture);
        _bloco.SetFloat("_Dano", 0f);
        _bloco.SetFloat("_EscalaRuido", CalcularEscalaRuido(_renderer.sprite));
        _renderer.SetPropertyBlock(_bloco);
    }

    public void DefinirDano(float valor)
    {
        _renderer.GetPropertyBlock(_bloco);
        _bloco.SetFloat("_Dano", Mathf.Clamp01(valor));
        _renderer.SetPropertyBlock(_bloco);
    }

    private float CalcularEscalaRuido(Sprite sprite)
    {
        float tamanhoMaior = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
        return 4f / tamanhoMaior;
    }
}
