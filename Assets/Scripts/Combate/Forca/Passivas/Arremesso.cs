using System;
using System.Collections.Generic;
using UnityEngine;

// Arremesso (Epica): inimigo repelido que bate em outro inimigo repassa parte do dano que recebeu. Acumula em cadeia.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Forca/Arremesso")]
public class Arremesso : DadosHabilidadePassiva
{
    [Header("Arremesso:")]
    [Tooltip("Percentual do dano do golpe que o inimigo arremessado repassa a quem ele atingir")]
    [SerializeField] private EscalaValor _percentualDanoRepassado = new EscalaValor(30f, 80f, 100);
    [Tooltip("Por quanto tempo, apos ser repelido, o inimigo ainda causa dano ao bater em outro")]
    [SerializeField] private float _duracaoArremesso = 0.6f;
    [Tooltip("Velocidade minima do corpo arremessado para o impacto contar")]
    [SerializeField] private float _velocidadeMinimaImpacto = 1.5f;
    [SerializeField] private Color _corNumeroDano = new Color(1f, 0.85f, 0.3f);
    [SerializeField] private GameObject _particulaImpacto;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<Arremesso>, IReageDanoCausado
    {
        public Logica(Arremesso dados) : base(dados) { }

        public void AoCausarDano(InformacaoDano informacao)
        {
            if (informacao.Origem == null || informacao.Origem.Repulsao <= 0 || !EhAlvo(informacao.Vitima)) return;
            Arremessar(informacao.Vitima, informacao.Dano, null);
        }

        private void Arremessar(Ser_Vivo arremessado, float danoRecebido, Ser_Vivo quemArremessou)
        {
            if (arremessado == null || arremessado.VidaAtual <= 0) return;

            float danoRepassado = danoRecebido * _dados._percentualDanoRepassado.Avaliar(Nivel) / 100f;
            if (danoRepassado <= 0) return;

            CorpoArremessado corpo = arremessado.GetComponent<CorpoArremessado>();
            if (corpo == null)
                corpo = arremessado.gameObject.AddComponent<CorpoArremessado>();
            corpo.Configurar(AplicarImpacto, danoRepassado, _dados._duracaoArremesso, _dados._velocidadeMinimaImpacto, quemArremessou);
        }

        private void AplicarImpacto(Ser_Vivo arremessado, Ser_Vivo atingido, float danoRepassado)
        {
            InstanciarParticula(_dados._particulaImpacto, (CentroDe(arremessado) + CentroDe(atingido)) / 2f, 1.5f);
            AplicarDanoHabilidade(atingido, danoRepassado, Tipo_Dano.Fisico, _dados._corNumeroDano);

            // Acumula: quem foi atingido tambem vira um corpo arremessado, com parte do dano que levou
            Arremessar(atingido, danoRepassado, arremessado);
        }
    }
}

// Colocado temporariamente no inimigo repelido; avisa quando ele bate em outro do mesmo grupo
public class CorpoArremessado : MonoBehaviour
{
    private Action<Ser_Vivo, Ser_Vivo, float> _aoImpactar;
    private Ser_Vivo _serVivo;
    private Rigidbody2D _rigidbody;
    private float _danoRepassado;
    private float _tempoFinal;
    private float _velocidadeMinimaImpacto;
    private readonly HashSet<Ser_Vivo> _jaAtingidos = new HashSet<Ser_Vivo>();

    public void Configurar(Action<Ser_Vivo, Ser_Vivo, float> aoImpactar, float danoRepassado, float duracao, float velocidadeMinimaImpacto, Ser_Vivo ignorar)
    {
        bool arremessoEmAndamento = _aoImpactar != null && Time.time <= _tempoFinal;
        _aoImpactar = aoImpactar;
        _serVivo = GetComponent<Ser_Vivo>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _danoRepassado = danoRepassado;
        _tempoFinal = Time.time + duracao;
        _velocidadeMinimaImpacto = velocidadeMinimaImpacto;
        if (!arremessoEmAndamento) _jaAtingidos.Clear();
        // Quem arremessou este corpo nao leva o dano de volta
        if (ignorar != null) _jaAtingidos.Add(ignorar);
    }

    private void Update()
    {
        if (Time.time > _tempoFinal || _aoImpactar == null)
            Destroy(this);
    }

    private void OnCollisionEnter2D(Collision2D colisao)
    {
        VerificarImpacto(colisao.collider);
    }

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        VerificarImpacto(colisor);
    }

    private void VerificarImpacto(Collider2D colisor)
    {
        if (_aoImpactar == null || _serVivo == null) return;
        if (_rigidbody != null && _rigidbody.linearVelocity.magnitude < _velocidadeMinimaImpacto) return;

        Ser_Vivo atingido = colisor.GetComponent<Ser_Vivo>();
        if (atingido == null || atingido == _serVivo || atingido.VidaAtual <= 0) return;
        if (atingido.gameObject.layer != _serVivo.gameObject.layer) return; // so inimigos do mesmo grupo
        if (!_jaAtingidos.Add(atingido)) return;

        _aoImpactar(_serVivo, atingido, _danoRepassado);
    }
}
