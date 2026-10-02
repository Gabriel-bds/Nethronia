using System.Collections.Generic;
using UnityEngine;

public enum Estado_Combate { Congelado, Envenenado, Desacelerado }

// Guarda quais seres vivos estao sob algum estado (congelado, envenenado...) e ate quando,
// para que habilidades como Opressao Glacial e Mutacao Toxica consigam consultar.
public static class EstadosCombate
{
    private static readonly Dictionary<Estado_Combate, Dictionary<Ser_Vivo, List<float>>> _aplicacoesPorEstado =
        new Dictionary<Estado_Combate, Dictionary<Ser_Vivo, List<float>>>();

    public static void Registrar(Ser_Vivo serVivo, Estado_Combate estado, float duracao)
    {
        if (serVivo == null) return;

        Dictionary<Ser_Vivo, List<float>> aplicacoes = AplicacoesDoEstado(estado);
        if (!aplicacoes.TryGetValue(serVivo, out List<float> temposFinais))
        {
            temposFinais = new List<float>();
            aplicacoes[serVivo] = temposFinais;
        }
        temposFinais.Add(Time.time + duracao);
    }

    public static bool Possui(Ser_Vivo serVivo, Estado_Combate estado)
    {
        return QuantidadeAcumulos(serVivo, estado) > 0;
    }

    // Quantas aplicacoes do estado ainda estao ativas no ser vivo (para efeitos que acumulam)
    public static int QuantidadeAcumulos(Ser_Vivo serVivo, Estado_Combate estado)
    {
        if (serVivo == null) return 0;
        if (!AplicacoesDoEstado(estado).TryGetValue(serVivo, out List<float> temposFinais)) return 0;

        temposFinais.RemoveAll(tempoFinal => tempoFinal <= Time.time);
        return temposFinais.Count;
    }

    public static int QuantidadeSeresVivosCom(Estado_Combate estado)
    {
        Dictionary<Ser_Vivo, List<float>> aplicacoes = AplicacoesDoEstado(estado);
        List<Ser_Vivo> semEstado = new List<Ser_Vivo>();
        int quantidade = 0;

        foreach (KeyValuePair<Ser_Vivo, List<float>> aplicacao in aplicacoes)
        {
            aplicacao.Value.RemoveAll(tempoFinal => tempoFinal <= Time.time);
            if (aplicacao.Key == null || aplicacao.Key.VidaAtual <= 0 || aplicacao.Value.Count == 0)
                semEstado.Add(aplicacao.Key);
            else
                quantidade++;
        }

        foreach (Ser_Vivo serVivo in semEstado)
            aplicacoes.Remove(serVivo);

        return quantidade;
    }

    private static Dictionary<Ser_Vivo, List<float>> AplicacoesDoEstado(Estado_Combate estado)
    {
        if (!_aplicacoesPorEstado.TryGetValue(estado, out Dictionary<Ser_Vivo, List<float>> aplicacoes))
        {
            aplicacoes = new Dictionary<Ser_Vivo, List<float>>();
            _aplicacoesPorEstado[estado] = aplicacoes;
        }
        return aplicacoes;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        _aplicacoesPorEstado.Clear();
    }
}
