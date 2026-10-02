using UnityEngine;

// Definicao de uma habilidade passiva (asset). Guarda o que a habilidade E: nome, raridade, poder,
// escalas por nivel e visuais. A logica fica na instancia criada por CriarInstancia().
public abstract class DadosHabilidadePassiva : ScriptableObject
{
    [Header("Identificacao:")]
    [SerializeField] private string _nome;
    [TextArea(2, 5)] [SerializeField] private string _descricao;
    [SerializeField] private Sprite _icone;
    [SerializeField] private Raridade_carta _raridade;
    [SerializeField] private Tipo_Poder _poder;

    [Header("Comportamento:")]
    [Tooltip("Ordem em que os modificadores de dano sao aplicados (menor primeiro)")]
    [SerializeField] private int _prioridade;
    [SerializeField] private int _nivelMaximoMagnitudeVisual = 100;
    [Tooltip("Camadas atingidas pelos efeitos de area. Vazio = inimigos do dono")]
    [SerializeField] private LayerMask _camadasAlvo;

    public string Nome => string.IsNullOrEmpty(_nome) ? name : _nome;
    public string Descricao => _descricao;
    public Sprite Icone => _icone;
    public Raridade_carta Raridade => _raridade;
    public Tipo_Poder Poder => _poder;
    public int Prioridade => _prioridade;
    public int NivelMaximoMagnitudeVisual => Mathf.Max(1, _nivelMaximoMagnitudeVisual);
    public LayerMask CamadasAlvo => _camadasAlvo;

    public abstract IHabilidadePassiva CriarInstancia();

#if UNITY_EDITOR
    // Usado pela ferramenta de editor ao criar os assets
    public void PreencherIdentificacao(string nome, string descricao, Raridade_carta raridade, Tipo_Poder poder)
    {
        _nome = nome;
        _descricao = descricao;
        _raridade = raridade;
        _poder = poder;
    }
#endif
}
