using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// O que uma passiva recebe ao ser ativada: quem e o dono, em que nivel ela esta e quem roda as corrotinas
public class ContextoHabilidade
{
    public Ser_Vivo Dono { get; }
    public GerenciadorHabilidades Gerenciador { get; }
    private readonly HabilidadeEquipada _equipada;

    // Lido sempre na hora, para mudancas de nivel (cartas, inspector) valerem na mesma partida
    public int Nivel => _equipada.Nivel;

    public ContextoHabilidade(Ser_Vivo dono, GerenciadorHabilidades gerenciador, HabilidadeEquipada equipada)
    {
        Dono = dono;
        Gerenciador = gerenciador;
        _equipada = equipada;
    }
}

// Base da logica das passivas, com as ferramentas que elas usam em comum.
// Cada passiva herda desta classe e assina so os contratos de que precisa.
public abstract class HabilidadePassiva<TDados> : IHabilidadePassiva where TDados : DadosHabilidadePassiva
{
    protected readonly TDados _dados;
    protected ContextoHabilidade _contexto;
    private readonly List<Coroutine> _rotinas = new List<Coroutine>();
    private readonly List<GameObject> _objetosCriados = new List<GameObject>();

    public DadosHabilidadePassiva Dados => _dados;

    protected HabilidadePassiva(TDados dados)
    {
        _dados = dados;
    }

    protected Ser_Vivo Dono => _contexto.Dono;
    protected int Nivel => _contexto.Nivel;
    protected bool DonoVivo => Dono != null && Dono.VidaAtual > 0;
    protected float FatorMagnitudeVisual => Mathf.Clamp01((float)Nivel / _dados.NivelMaximoMagnitudeVisual);

    protected LayerMask CamadasAlvo
    {
        get
        {
            if (_dados.CamadasAlvo.value != 0) return _dados.CamadasAlvo;
            return Dono is Player ? LayerMask.GetMask("Inimigo") : LayerMask.GetMask("Player");
        }
    }

    public void Ativar(ContextoHabilidade contexto)
    {
        _contexto = contexto;
        AoAtivar();
    }

    public void Desativar()
    {
        AoDesativar();
        foreach (Coroutine rotina in _rotinas)
            if (rotina != null && _contexto.Gerenciador != null)
                _contexto.Gerenciador.StopCoroutine(rotina);
        _rotinas.Clear();
        foreach (GameObject objeto in _objetosCriados)
            if (objeto != null) Object.Destroy(objeto);
        _objetosCriados.Clear();
    }

    protected virtual void AoAtivar() { }
    protected virtual void AoDesativar() { }

    protected bool EhAlvo(Ser_Vivo serVivo)
    {
        return serVivo != null && serVivo != Dono && ((1 << serVivo.gameObject.layer) & CamadasAlvo) != 0;
    }

    protected Coroutine IniciarRotina(IEnumerator rotina)
    {
        Coroutine iniciada = _contexto.Gerenciador.StartCoroutine(rotina);
        _rotinas.RemoveAll(existente => existente == null);
        _rotinas.Add(iniciada);
        return iniciada;
    }

    protected void PararRotina(Coroutine rotina)
    {
        if (rotina == null) return;
        _contexto.Gerenciador.StopCoroutine(rotina);
        _rotinas.Remove(rotina);
    }

    // Objeto auxiliar que vive enquanto a passiva estiver ativa (destruido no Desativar)
    protected T CriarComponenteAuxiliar<T>(string nome) where T : Component
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.SetParent(_contexto.Gerenciador.transform, false);
        _objetosCriados.Add(objeto);
        return objeto.AddComponent<T>();
    }

    protected void Curar(float valor, Color cor)
    {
        if (!DonoVivo || valor <= 0) return;
        Dono.VidaAtual += valor;
        Utilidades.InstanciarNumeroDano($"+{valor:0.#}", Dono.transform, cor);
    }

    protected GameObject InstanciarParticula(GameObject particula, Vector3 posicao, float duracao, Transform pai = null)
    {
        if (particula == null) return null;

        GameObject instancia = pai != null
            ? Object.Instantiate(particula, posicao, Quaternion.identity, pai)
            : Object.Instantiate(particula, posicao, Quaternion.identity);

        Animator animador = instancia.GetComponent<Animator>();
        if (animador != null)
            animador.SetFloat("Forca", FatorMagnitudeVisual);

        if (duracao > 0)
            Object.Destroy(instancia, duracao);
        return instancia;
    }

    protected static Vector2 CentroDe(Ser_Vivo serVivo)
    {
        Collider2D colisor = serVivo.GetComponent<Collider2D>();
        return colisor != null ? (Vector2)colisor.bounds.center : (Vector2)serVivo.transform.position;
    }

    protected static List<Ser_Vivo> SeresVivosNaArea(Vector2 centro, float raio, LayerMask camadas)
    {
        List<Ser_Vivo> encontrados = new List<Ser_Vivo>();
        foreach (Collider2D colisor in Physics2D.OverlapCircleAll(centro, raio, camadas))
        {
            Ser_Vivo serVivo = colisor.GetComponent<Ser_Vivo>();
            if (serVivo != null && serVivo.VidaAtual > 0 && !encontrados.Contains(serVivo))
                encontrados.Add(serVivo);
        }
        return encontrados;
    }

    // Dano "de habilidade": passa pelo sistema de combate (modificadores, numeros, sangue e eventos)
    protected void AplicarDanoHabilidade(Ser_Vivo vitima, float dano, Tipo_Dano tipoDano, Color corNumero)
    {
        if (vitima == null || vitima.VidaAtual <= 0 || vitima._invulneravel) return;

        float danoFinal = EventosCombate.ModificarDano(Dono, vitima, dano, tipoDano, null);
        if (danoFinal <= 0) return;

        vitima.AplicarDano(danoFinal);
        Utilidades.InstanciarNumeroDano((-danoFinal).ToString("0.#"), vitima.transform, corNumero);

        if (vitima._sangue != null)
        {
            ParticleSystem sangue = Object.Instantiate(vitima._sangue, vitima.transform).GetComponent<ParticleSystem>();
            sangue.transform.localPosition = Vector3.zero;
            sangue.transform.rotation = Quaternion.identity;
            var emissao = sangue.emission;
            emissao.rateOverTime = danoFinal / vitima._vidaMax * emissao.rateOverTime.constant;
        }

        EventosCombate.NotificarDanoCausado(Dono, vitima, danoFinal, tipoDano, null);
    }

    protected void TingirMembros(Color cor, float duracaoTransicao)
    {
        _contexto.Gerenciador.TingirMembros(cor, duracaoTransicao);
    }

    protected void RestaurarCorMembros(float duracaoTransicao)
    {
        _contexto.Gerenciador.RestaurarCorMembros(duracaoTransicao);
    }
}
