using System;
using System.Collections.Generic;
using UnityEngine;

public struct InformacaoDano
{
    public Ser_Vivo Atacante;
    public Ser_Vivo Vitima;
    public float Dano;
    public Tipo_Dano TipoDano;
    public Ataque Origem;
}

public delegate float ModificadorDano(Ser_Vivo atacante, Ser_Vivo vitima, float dano, Tipo_Dano tipoDano, Ataque origem);

// Ponto central de comunicacao do combate: quem aplica dano avisa aqui, e as habilidades passivas
// escutam os eventos ou registram modificadores, sem precisar alterar Ser_Vivo, Player ou Inimigo.
public static class EventosCombate
{
    public static event Action<InformacaoDano> AoCausarDano;
    public static event Action<Ser_Vivo, Ser_Vivo> AoMorrer; // vitima, ultimo atacante

    private static readonly List<ModificadorDano> _modificadoresDano = new List<ModificadorDano>();
    private static readonly Dictionary<Ser_Vivo, Ser_Vivo> _ultimoAtacantePorVitima = new Dictionary<Ser_Vivo, Ser_Vivo>();
    private static readonly Dictionary<Ser_Vivo, Action<Ser_Vivo, float, float>> _observadoresMorte = new Dictionary<Ser_Vivo, Action<Ser_Vivo, float, float>>();
    private static readonly HashSet<Ser_Vivo> _mortesNotificadas = new HashSet<Ser_Vivo>();

    public static void RegistrarModificador(ModificadorDano modificador)
    {
        if (!_modificadoresDano.Contains(modificador))
            _modificadoresDano.Add(modificador);
    }

    public static void RemoverModificador(ModificadorDano modificador)
    {
        _modificadoresDano.Remove(modificador);
    }

    public static float ModificarDano(Ser_Vivo atacante, Ser_Vivo vitima, float dano, Tipo_Dano tipoDano, Ataque origem)
    {
        float danoModificado = dano;
        foreach (ModificadorDano modificador in _modificadoresDano.ToArray())
            danoModificado = modificador(atacante, vitima, danoModificado, tipoDano, origem);
        return Mathf.Max(0f, danoModificado);
    }

    public static void NotificarDanoCausado(Ser_Vivo atacante, Ser_Vivo vitima, float dano, Tipo_Dano tipoDano, Ataque origem)
    {
        if (vitima == null) return;

        if (atacante != null)
        {
            _ultimoAtacantePorVitima[vitima] = atacante;
            ObservarMorte(vitima);
        }

        AoCausarDano?.Invoke(new InformacaoDano
        {
            Atacante = atacante,
            Vitima = vitima,
            Dano = dano,
            TipoDano = tipoDano,
            Origem = origem
        });

        // Cobre os casos em que a vida ja chegou a zero antes de o observador ser registrado
        if (vitima.VidaAtual <= 0)
            NotificarMorte(vitima);
    }

    public static Ser_Vivo UltimoAtacante(Ser_Vivo vitima)
    {
        return vitima != null && _ultimoAtacantePorVitima.TryGetValue(vitima, out Ser_Vivo atacante) ? atacante : null;
    }

    private static void ObservarMorte(Ser_Vivo vitima)
    {
        // Um ser vivo que voltou a ter vida (ex.: Reserva Biologica) pode morrer de novo
        if (_mortesNotificadas.Contains(vitima) && vitima.VidaAtual > 0)
            _mortesNotificadas.Remove(vitima);

        if (_observadoresMorte.ContainsKey(vitima) || _mortesNotificadas.Contains(vitima)) return;

        LimparReferenciasDestruidas();

        Action<Ser_Vivo, float, float> observador = (serVivo, vidaAtual, vidaMaxima) =>
        {
            if (vidaAtual <= 0)
                NotificarMorte(serVivo);
        };
        _observadoresMorte[vitima] = observador;
        vitima.OnVidaAlterada += observador;
    }

    private static void NotificarMorte(Ser_Vivo vitima)
    {
        if (_mortesNotificadas.Contains(vitima)) return;
        _mortesNotificadas.Add(vitima);

        if (_observadoresMorte.TryGetValue(vitima, out Action<Ser_Vivo, float, float> observador))
        {
            vitima.OnVidaAlterada -= observador;
            _observadoresMorte.Remove(vitima);
        }

        Ser_Vivo atacante = UltimoAtacante(vitima);
        _ultimoAtacantePorVitima.Remove(vitima);

        AoMorrer?.Invoke(vitima, atacante);
    }

    private static void LimparReferenciasDestruidas()
    {
        List<Ser_Vivo> destruidos = new List<Ser_Vivo>();
        foreach (Ser_Vivo serVivo in _observadoresMorte.Keys)
            if (serVivo == null) destruidos.Add(serVivo);

        foreach (Ser_Vivo destruido in destruidos)
        {
            _observadoresMorte.Remove(destruido);
            _ultimoAtacantePorVitima.Remove(destruido);
        }
        _mortesNotificadas.RemoveWhere(serVivo => serVivo == null);
    }

    // Ao trocar de cena os seres vivos sao destruidos, entao o registro precisa comecar limpo
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        AoCausarDano = null;
        AoMorrer = null;
        _modificadoresDano.Clear();
        _ultimoAtacantePorVitima.Clear();
        _observadoresMorte.Clear();
        _mortesNotificadas.Clear();
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= AoDescarregarCena;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += AoDescarregarCena;
    }

    private static void AoDescarregarCena(UnityEngine.SceneManagement.Scene cena)
    {
        LimparReferenciasDestruidas();
    }
}
