using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Uma passiva que o ser vivo possui: qual e (asset), em que nivel e se e permanente
[Serializable]
public class HabilidadeEquipada
{
    [SerializeField] private DadosHabilidadePassiva _dados;
    [SerializeField] private int _nivel;
    [Tooltip("Desbloqueada de forma permanente (ex.: no lobby). Nao e perdida quando o jogador morre")]
    [SerializeField] private bool _permanente;

    public DadosHabilidadePassiva Dados => _dados;
    public bool Permanente
    {
        get => _permanente;
        set => _permanente = value;
    }
    public int Nivel
    {
        get => _nivel;
        set => _nivel = Mathf.Max(0, value);
    }

    public HabilidadeEquipada(DadosHabilidadePassiva dados, int nivel, bool permanente)
    {
        _dados = dados;
        _nivel = nivel;
        _permanente = permanente;
    }
}

// Unico componente de passivas do ser vivo: guarda a lista de habilidades, escuta o combate
// uma vez so e repassa cada momento apenas para as passivas que assinaram aquele contrato.
public class GerenciadorHabilidades : MonoBehaviour
{
    [Tooltip("Passivas que o ser vivo possui. As cartas adicionam aqui; ao morrer, as nao permanentes sao removidas. Mudancas no inspector valem durante o Play")]
    [SerializeField] private List<HabilidadeEquipada> _habilidades = new List<HabilidadeEquipada>();

    private Ser_Vivo _dono;
    private readonly Dictionary<HabilidadeEquipada, IHabilidadePassiva> _instanciasPorHabilidade = new Dictionary<HabilidadeEquipada, IHabilidadePassiva>();
    private IHabilidadePassiva[] _ativasPorPrioridade = new IHabilidadePassiva[0];
    private bool _inscrito;

    private List<SpriteRenderer> _spritesMembros;
    private Color _corPadraoMembros = Color.white;
    private Coroutine _rotinaCor;

    public Ser_Vivo Dono => _dono;
    public IReadOnlyList<HabilidadeEquipada> Habilidades => _habilidades;

    public static GerenciadorHabilidades De(Ser_Vivo serVivo)
    {
        // TryGetComponent devolve null de verdade (permite usar ?. com seguranca)
        return serVivo != null && serVivo.TryGetComponent(out GerenciadorHabilidades gerenciador) ? gerenciador : null;
    }

    private void Awake()
    {
        _dono = GetComponentInParent<Ser_Vivo>();
    }

    private void OnEnable()
    {
        if (_dono == null) return;

        EventosCombate.AoCausarDano += AoCausarDano;
        EventosCombate.AoMorrer += AoMorrer;
        EventosCombate.RegistrarModificador(ModificarDano);
        Ataque.AoInstanciarAtaque += AoInstanciarAtaque;
        _dono.OnVidaAlterada += VerificarMorteDono;
        _inscrito = true;

        Sincronizar();
    }

    private void OnDisable()
    {
        if (_inscrito)
        {
            EventosCombate.AoCausarDano -= AoCausarDano;
            EventosCombate.AoMorrer -= AoMorrer;
            EventosCombate.RemoverModificador(ModificarDano);
            Ataque.AoInstanciarAtaque -= AoInstanciarAtaque;
            if (_dono != null) _dono.OnVidaAlterada -= VerificarMorteDono;
            _inscrito = false;
        }

        foreach (IHabilidadePassiva instancia in _instanciasPorHabilidade.Values)
            instancia.Desativar();
        _instanciasPorHabilidade.Clear();
        _ativasPorPrioridade = new IHabilidadePassiva[0];
    }

    // Mudancas feitas no inspector durante o Play passam a valer na hora
    private void OnValidate()
    {
        if (Application.isPlaying && _inscrito)
            Sincronizar();
    }

    private void Update()
    {
        float deltaTempo = Time.deltaTime;
        foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
            if (passiva is IAtualizavel atualizavel)
                atualizavel.Atualizar(deltaTempo);
    }

    #region Lista de habilidades

    // Desbloqueia a habilidade (ex.: carta). Se o dono ja a possui, so atualiza o nivel
    // (e a torna permanente, se for o caso; uma permanente nunca volta a ser temporaria)
    public HabilidadeEquipada Adicionar(DadosHabilidadePassiva dados, int nivel = 0, bool permanente = false)
    {
        if (dados == null) return null;

        HabilidadeEquipada existente = Buscar(dados);
        if (existente != null)
        {
            existente.Nivel = nivel;
            existente.Permanente |= permanente;
            return existente;
        }

        HabilidadeEquipada nova = new HabilidadeEquipada(dados, nivel, permanente);
        _habilidades.Add(nova);
        if (_inscrito) Sincronizar();
        return nova;
    }

    public void Remover(DadosHabilidadePassiva dados)
    {
        HabilidadeEquipada existente = Buscar(dados);
        if (existente == null) return;
        _habilidades.Remove(existente);
        if (_inscrito) Sincronizar();
    }

    // Chamado quando o jogador morre: perde tudo que foi conquistado na run e mantem as permanentes
    public void RemoverTemporarias()
    {
        _habilidades.RemoveAll(habilidade => habilidade == null || !habilidade.Permanente);
        if (_inscrito) Sincronizar();
    }

    public bool Possui(DadosHabilidadePassiva dados) => Buscar(dados) != null;

    public int NivelDe(DadosHabilidadePassiva dados) => Buscar(dados)?.Nivel ?? -1;

    public void DefinirNivel(DadosHabilidadePassiva dados, int nivel)
    {
        HabilidadeEquipada existente = Buscar(dados);
        if (existente != null) existente.Nivel = nivel;
    }

    private HabilidadeEquipada Buscar(DadosHabilidadePassiva dados)
    {
        return _habilidades.FirstOrDefault(habilidade => habilidade != null && habilidade.Dados == dados);
    }

    // Deixa as instancias ativas iguais a lista: cria as novas e desliga as removidas
    private void Sincronizar()
    {
        foreach (KeyValuePair<HabilidadeEquipada, IHabilidadePassiva> par in _instanciasPorHabilidade.ToList())
        {
            HabilidadeEquipada habilidade = par.Key;
            bool continuaValida = _habilidades.Contains(habilidade) && habilidade.Dados == par.Value.Dados;
            if (continuaValida) continue;

            par.Value.Desativar();
            _instanciasPorHabilidade.Remove(habilidade);
        }

        foreach (HabilidadeEquipada habilidade in _habilidades)
        {
            if (habilidade == null || habilidade.Dados == null) continue;
            if (_instanciasPorHabilidade.ContainsKey(habilidade)) continue;

            IHabilidadePassiva instancia = habilidade.Dados.CriarInstancia();
            _instanciasPorHabilidade[habilidade] = instancia;
            instancia.Ativar(new ContextoHabilidade(_dono, this, habilidade));
        }

        _ativasPorPrioridade = _instanciasPorHabilidade.Values.OrderBy(instancia => instancia.Dados.Prioridade).ToArray();
    }

    #endregion

    #region Repasse dos eventos de combate

    private float ModificarDano(Ser_Vivo atacante, Ser_Vivo vitima, float dano, Tipo_Dano tipoDano, Ataque origem)
    {
        if (_dono == null || atacante == vitima) return dano;

        InformacaoDano informacao = new InformacaoDano { Atacante = atacante, Vitima = vitima, Dano = dano, TipoDano = tipoDano, Origem = origem };

        if (atacante == _dono)
        {
            foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
                if (passiva is IModificaDanoCausado modificador)
                    informacao.Dano = modificador.ModificarDanoCausado(informacao);
        }
        else if (vitima == _dono)
        {
            foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
                if (passiva is IModificaDanoRecebido modificador)
                    informacao.Dano = modificador.ModificarDanoRecebido(informacao);
        }
        return informacao.Dano;
    }

    private void AoCausarDano(InformacaoDano informacao)
    {
        if (_dono == null || informacao.Atacante == informacao.Vitima) return;

        if (informacao.Atacante == _dono)
        {
            foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
                if (passiva is IReageDanoCausado reacao)
                    reacao.AoCausarDano(informacao);
        }
        else if (informacao.Vitima == _dono)
        {
            foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
                if (passiva is IReageDanoRecebido reacao)
                    reacao.AoReceberDano(informacao);
        }
    }

    private void AoMorrer(Ser_Vivo vitima, Ser_Vivo atacante)
    {
        if (_dono == null || atacante != _dono || vitima == _dono) return;

        foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
            if (passiva is IReageAbate reacao)
                reacao.AoMatar(vitima);
    }

    private void AoInstanciarAtaque(Ataque ataque)
    {
        if (ataque == null || ataque._dono != _dono) return;

        foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
            if (passiva is IReageAtaqueLancado reacao)
                reacao.AoLancarAtaque(ataque);
    }

    private void VerificarMorteDono(Ser_Vivo serVivo, float vidaAtual, float vidaMaxima)
    {
        if (vidaAtual > 0) return;

        foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
            if (passiva is IReageMorteDono reacao && reacao.AoDonoMorrer())
                return;
    }

    public bool PermiteUsoSemRecarga(Ataque ataque)
    {
        foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
            if (passiva is IControlaRecarga controle && controle.UsarSemRecarregar(ataque))
                return true;
        return false;
    }

    public void NotificarProjetilRefletido(Projetil projetil)
    {
        foreach (IHabilidadePassiva passiva in _ativasPorPrioridade)
            if (passiva is IReageProjetilRefletido reacao)
                reacao.AoRefletirProjetil(projetil);
    }

    #endregion

    #region Cor dos membros (feedback visual compartilhado entre as passivas)

    public void TingirMembros(Color corAlvo, float duracaoTransicao)
    {
        if (!isActiveAndEnabled) return;
        ColetarSpritesMembros();
        if (_rotinaCor != null) StopCoroutine(_rotinaCor);
        _rotinaCor = StartCoroutine(TransicaoCor(corAlvo, duracaoTransicao));
    }

    public void RestaurarCorMembros(float duracaoTransicao)
    {
        ColetarSpritesMembros();
        TingirMembros(_corPadraoMembros, duracaoTransicao);
    }

    private void ColetarSpritesMembros()
    {
        if (_spritesMembros != null || _dono == null) return;

        _spritesMembros = new List<SpriteRenderer>();
        foreach (Rigidbody2D membro in _dono._membros)
        {
            SpriteRenderer sprite = membro != null ? membro.GetComponent<SpriteRenderer>() : null;
            if (sprite != null) _spritesMembros.Add(sprite);
        }
        if (_spritesMembros.Count > 0)
            _corPadraoMembros = _spritesMembros[0].color;
    }

    private IEnumerator TransicaoCor(Color corAlvo, float duracao)
    {
        _spritesMembros.RemoveAll(sprite => sprite == null);
        Color[] coresIniciais = _spritesMembros.Select(sprite => sprite.color).ToArray();

        float tempo = 0f;
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            for (int indice = 0; indice < _spritesMembros.Count; indice++)
                if (_spritesMembros[indice] != null)
                    _spritesMembros[indice].color = Color.Lerp(coresIniciais[indice], corAlvo, tempo / duracao);
            yield return null;
        }

        foreach (SpriteRenderer sprite in _spritesMembros)
            if (sprite != null) sprite.color = corAlvo;
        _rotinaCor = null;
    }

    #endregion
}
