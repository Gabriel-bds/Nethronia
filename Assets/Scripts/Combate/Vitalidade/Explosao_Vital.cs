using System.Collections;
using UnityEngine;

public class ExplosaoVital : Ataque
{
    [Header("Explosão Vital")]
    [SerializeField] private EscalaValor _escalaConsumoVida;
    [SerializeField] private EscalaValor _escalaRaio;

    protected override void Start()
    {
        //base.Start();
        ControlarRecarga();
        ControlarEscalaVisualVitalidade();
        gameObject.layer = default;
        Explodir();
    }

    private void ControlarEscalaVisualVitalidade()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float forca = Utilidades.LimitadorNumero(0, 1,
                (float)_dono._poderVitalidade._nivel / nivelMaximoMagnitudeVisual);
            anim.SetFloat("Forca", forca);
        }
    }

    private void Explodir()
    {
        int nivel = _dono._poderVitalidade._nivel;
        float porcentagemConsumo = _escalaConsumoVida.Avaliar(nivel) / 100f;
        float raio = _escalaRaio.Avaliar(nivel);

        float vidaConsumida = _dono.VidaAtual * porcentagemConsumo;
        vidaConsumida = Mathf.Min(vidaConsumida, _dono.VidaAtual - 1f);

        if (vidaConsumida <= 0) return;

        _dono.VidaAtual -= vidaConsumida;
        Utilidades.InstanciarNumeroDano((-vidaConsumida).ToString(), _dono.transform, new Color(1f, 0.4f, 0f));

        Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, raio, _alvos);

        int inimigosAtingidos = 0;
        foreach (Collider2D col in atingidos)
        {
            Ser_Vivo serVivo = col.GetComponent<Ser_Vivo>();
            if (serVivo == null || serVivo._invulneravel) continue;

            serVivo.AplicarDano(vidaConsumida);
            Utilidades.InstanciarNumeroDano((-vidaConsumida).ToString(), serVivo.transform);

            if (serVivo._sangue != null)
            {
                ParticleSystem sangue = Instantiate(serVivo._sangue, serVivo.transform.position, Quaternion.identity)
                    .GetComponent<ParticleSystem>();
                var emissao = sangue.emission;
                emissao.rateOverTime = vidaConsumida / serVivo._vidaMax * emissao.rateOverTime.constant;
            }

            inimigosAtingidos++;
        }

        if (inimigosAtingidos > 0)
        {
            FindObjectOfType<Camera_Controller>()?.Tremer(vidaConsumida / _dono._vidaMax * inimigosAtingidos);
            FindObjectOfType<Hitstop>()?.Aplicar(vidaConsumida / _dono._vidaMax);
            SomHit(vidaConsumida / _dono._vidaMax);
        }

        StartCoroutine(EncerrarExplosao());
    }

    private IEnumerator EncerrarExplosao()
    {
        yield return new WaitForSeconds(1f);
        foreach (ParticleSystem particula in GetComponentsInChildren<ParticleSystem>())
            particula.Stop();
        Destroy(gameObject, 2f);
    }
}
