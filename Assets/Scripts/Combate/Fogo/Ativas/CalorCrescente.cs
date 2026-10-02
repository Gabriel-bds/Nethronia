using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Calor Crescente (Epica): ao ativar, todo o cenario esquenta e causa dano de fogo nos inimigos
// a cada intervalo, durante um tempo. O dano cresce a cada tique.
public class CalorCrescente : Ataque
{
    [Header("Calor Crescente:")]
    [SerializeField] private EscalaValor _intervaloEntreTiques = new EscalaValor(5f, 2f, 100);
    [SerializeField] private EscalaValor _duracao = new EscalaValor(20f, 40f, 100);
    [Tooltip("Quanto o dano cresce (%) a cada tique em relacao ao primeiro")]
    [SerializeField] private EscalaValor _crescimentoDanoPorTiquePercentual = new EscalaValor(25f, 50f, 100);
    [SerializeField] private Color _corNumeroDano = new Color32(255, 104, 0, 255);
    [Tooltip("Particula criada em cada inimigo atingido por um tique")]
    [SerializeField] private GameObject _particulaTique;

    protected override void Start()
    {
        _efeitoAplicado = GetComponent<Efeito>();
        gameObject.layer = default;
        transform.position = _dono.transform.position;
        ControlarMagnitudadeVisual();
        ControlarRecarga();
        SomIntanciar();
        StartCoroutine(Esquentar());
    }

    private void Update()
    {
        // O "calor" acompanha o dono (as particulas cobrem a area em volta dele)
        if (_dono != null)
            transform.position = _dono.transform.position;
    }

    private IEnumerator Esquentar()
    {
        int nivelFogo = _dono._poderFogo._nivel;
        float intervalo = Mathf.Max(0.1f, _intervaloEntreTiques.Avaliar(nivelFogo));
        float tempoRestante = _duracao.Avaliar(nivelFogo);
        int tique = 0;

        while (tempoRestante > 0f && _dono != null)
        {
            yield return new WaitForSeconds(intervalo);
            tempoRestante -= intervalo;

            float multiplicadorCrescimento = 1f + _crescimentoDanoPorTiquePercentual.Avaliar(nivelFogo) / 100f * tique;
            foreach (Ser_Vivo inimigo in InimigosVivos())
                AplicarTique(inimigo, multiplicadorCrescimento);
            tique++;
        }

        Encerrar();
    }

    private List<Ser_Vivo> InimigosVivos()
    {
        List<Ser_Vivo> inimigos = new List<Ser_Vivo>();
        foreach (Ser_Vivo serVivo in FindObjectsByType<Ser_Vivo>())
            if (serVivo != _dono && serVivo.VidaAtual > 0 && !serVivo._invulneravel && ((1 << serVivo.gameObject.layer) & _alvos) != 0)
                inimigos.Add(serVivo);
        return inimigos;
    }

    private void AplicarTique(Ser_Vivo inimigo, float multiplicadorCrescimento)
    {
        float dano = Utilidades.ArredondarNegativo(_dano / 100 * _dono._poderFogo._dano * multiplicadorCrescimento
            - inimigo._poderResistencia._negacaoDano / 2 - inimigo._poderFogo._negacaoDano);
        dano = EventosCombate.ModificarDano(_dono, inimigo, dano, _tipoDano, this);
        if (dano <= 0) return;

        inimigo.AplicarDano(dano);
        Utilidades.InstanciarNumeroDano((-dano).ToString("0.#"), inimigo.transform, _corNumeroDano);

        if (_particulaTique != null)
        {
            GameObject particula = Instantiate(_particulaTique, inimigo.transform.position, Quaternion.identity, inimigo.transform);
            Animator animador = particula.GetComponent<Animator>();
            if (animador != null)
                animador.SetFloat("Forca", Utilidades.LimitadorNumero(0, 1, (float)_dono._poderFogo._nivel / Mathf.Max(1, nivelMaximoMagnitudeVisual)));
            Destroy(particula, 2f);
        }

        if (_efeitoAplicado != null)
            _efeitoAplicado.Aplicar(_dono, inimigo);

        SomHit(dano / inimigo._vidaMax);
        EventosCombate.NotificarDanoCausado(_dono, inimigo, dano, _tipoDano, this);
    }

    private void Encerrar()
    {
        foreach (ParticleSystem particula in GetComponentsInChildren<ParticleSystem>())
            particula.Stop();

        Destroy(gameObject, 3f);
    }
}
