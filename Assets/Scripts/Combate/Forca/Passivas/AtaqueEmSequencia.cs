using System.Collections.Generic;
using UnityEngine;

// Ataque em Sequencia: permite usar um ataque X vezes seguidas antes de ele entrar em recarga.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Forca/Ataque em Sequencia")]
public class AtaqueEmSequencia : DadosHabilidadePassiva
{
    [Header("Ataque em Sequencia:")]
    [SerializeField] private EscalaValor _quantidadeAtaquesEmSequencia = new EscalaValor(2f, 5f, 100);
    [Tooltip("IDs dos ataques afetados. Vazio = todos os golpes fisicos comuns (soco, palmada...)")]
    [SerializeField] private List<int> _idsAtaquesAfetados = new List<int>();

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<AtaqueEmSequencia>, IControlaRecarga
    {
        private readonly Dictionary<int, int> _usosSeguidosPorAtaque = new Dictionary<int, int>();
        private readonly Dictionary<int, float> _ultimoUsoPorAtaque = new Dictionary<int, float>();

        public Logica(AtaqueEmSequencia dados) : base(dados) { }

        public bool UsarSemRecarregar(Ataque ataque)
        {
            if (!AfetaAtaque(ataque)) return false;

            int idAtaque = ataque._idAtaque;

            // Se passou tempo demais desde o ultimo uso, a sequencia recomeca
            if (_ultimoUsoPorAtaque.TryGetValue(idAtaque, out float ultimoUso) && Time.time - ultimoUso > Mathf.Max(ataque._tempoRecargaTotal, 1f))
                _usosSeguidosPorAtaque[idAtaque] = 0;
            _ultimoUsoPorAtaque[idAtaque] = Time.time;

            _usosSeguidosPorAtaque.TryGetValue(idAtaque, out int usosSeguidos);
            usosSeguidos++;

            int quantidadeMaxima = Mathf.Max(1, Mathf.RoundToInt(_dados._quantidadeAtaquesEmSequencia.Avaliar(Nivel)));
            if (usosSeguidos >= quantidadeMaxima)
            {
                _usosSeguidosPorAtaque[idAtaque] = 0;
                return false;
            }

            _usosSeguidosPorAtaque[idAtaque] = usosSeguidos;
            return true;
        }

        private bool AfetaAtaque(Ataque ataque)
        {
            if (_dados._idsAtaquesAfetados.Count > 0)
                return _dados._idsAtaquesAfetados.Contains(ataque._idAtaque);

            return ataque._tipoDano == Tipo_Dano.Fisico && ataque.GetType() == typeof(Ataque);
        }
    }
}
