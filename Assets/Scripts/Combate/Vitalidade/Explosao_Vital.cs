using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosaoVital : Ataque
{
    [Header("Explosão Vital")]
    [SerializeField] private EscalaValor _escalaConsumoVida;
    [SerializeField] private int _nivelMaximoVitalidade;

    private float _danoExplosao;
    private HashSet<Ser_Vivo> _jaAtingidos = new HashSet<Ser_Vivo>();

    protected override void Start()
    {
        //base.Start();
        ControlarRecarga();
        gameObject.layer = default;
        ConsumirVida();
        ControlarEscalaVisualVitalidade();
        StartCoroutine(EncerrarExplosao());
    }

    private void ControlarEscalaVisualVitalidade()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float forca = Utilidades.LimitadorNumero(0, 1,
                (float)_dono._poderVitalidade._nivel / _nivelMaximoVitalidade);
            anim.SetFloat("Forca", forca);
        }
    }

    private void ConsumirVida()
    {
        int nivel = _dono._poderVitalidade._nivel;
        float porcentagemConsumo = _escalaConsumoVida.Avaliar(nivel) / 100f;

        float vidaConsumida = _dono.VidaAtual * porcentagemConsumo;
        vidaConsumida = Mathf.Min(vidaConsumida, _dono.VidaAtual - 1f);

        if (vidaConsumida <= 0) return;

        _danoExplosao = vidaConsumida;
        _dono.VidaAtual -= vidaConsumida;
        Utilidades.InstanciarNumeroDano((-vidaConsumida).ToString(), _dono.transform, new Color(1f, 0.4f, 0f));
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (_danoExplosao <= 0) return;
        if (((1 << other.gameObject.layer) & _alvos) == 0) return;

        Ser_Vivo serVivo = other.GetComponent<Ser_Vivo>();
        if (serVivo == null || serVivo._invulneravel || _jaAtingidos.Contains(serVivo)) return;

        _jaAtingidos.Add(serVivo);
        serVivo.AplicarDano(_danoExplosao);
        Utilidades.InstanciarNumeroDano((-_danoExplosao).ToString(), serVivo.transform);

        if (serVivo._sangue != null)
        {
            ParticleSystem sangue = Instantiate(serVivo._sangue, serVivo.transform.position, Quaternion.identity)
                .GetComponent<ParticleSystem>();
            var emissao = sangue.emission;
            emissao.rateOverTime = _danoExplosao / serVivo._vidaMax * emissao.rateOverTime.constant;
        }

        FindObjectOfType<Camera_Controller>()?.Tremer(_danoExplosao / _dono._vidaMax);
        FindObjectOfType<Hitstop>()?.Aplicar(_danoExplosao / _dono._vidaMax);
        SomHit(_danoExplosao / _dono._vidaMax);
    }

    private IEnumerator EncerrarExplosao()
    {
        yield return new WaitForSeconds(1f);
        foreach (ParticleSystem particula in GetComponentsInChildren<ParticleSystem>())
            particula.Stop();
        Destroy(gameObject, 2f);
    }
}
