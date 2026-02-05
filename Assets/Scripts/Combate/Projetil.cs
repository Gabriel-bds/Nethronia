using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projetil : Ataque
{
    [Header("Projetil:")]
    [SerializeField] float _velocidade;
    [SerializeField] bool _progressaoEscala;
    [SerializeField] LayerMask _colisoes;
    [SerializeField] AudioSource _quebrandoSom;
    protected override void Start()
    {
        if (_progressaoEscala)
        {
            switch (_tipoDano)
            {
                case Tipo_Dano.Físico:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderForca._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderForca._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Fogo:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderFogo._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderFogo._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Gelo:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderGelo._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderGelo._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Veneno:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderVeneno._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderVeneno._nivel, 1, 0.033f)));
                    break;

                case Tipo_Dano.Eletricidade:
                    transform.localScale = new Vector2(Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderEletricidade._nivel, 1, 0.033f)), Utilidades.LimitadorNumero(1, 10, Utilidades.Escala(_dono._poderEletricidade._nivel, 1, 0.033f)));
                    break;
            }
        }
        base.Start();
        _tempoDeVidaAtual = _tempoDeVida;
    }
    void Update()
    {
        Movimentar();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _colisoes) != 0)
        {
            Debug.Log("Tocou");
            Ser_Vivo atingido = collision.GetComponent<Ser_Vivo>();

            if (atingido != null)
            {
                PeitoAco peito = atingido.GetComponentInChildren<PeitoAco>();

                if (peito != null && peito.ativo)
                {
                    float danoTotal = _dano / 100f * Utilidades.NivelAtualTipoDano(_tipoDano, _dono);

                    float danoNegado = peito.CalcularNegacao(danoTotal);
                    float danoFinal = danoTotal - danoNegado;

                    // Aplica o dano reduzido manualmente
                    AplicarAtaque(collision);

                    // Deflete o projetil
                    Defletir(
                        atingido,
                        danoNegado
                    );

                    return;
                }
                AplicarAtaque(collision);
            }
            try
            { 
                _quebrandoSom.Play();
            }
            catch 
            {
                //Debug.Log("Projetil sem som de hit anexado");
            }
            //Debug.Log("Chegou na destruição");
            IniciarDestruicao();
        }
        //base.OnTriggerEnter2D(collision);
    }
    void Movimentar()
    {
        if(_tempoDeVidaAtual  > 0)
        { 
            transform.Translate(Vector3.right * _velocidade * Time.deltaTime);
            _tempoDeVidaAtual -= Time.deltaTime;
        }
        else
        {
            IniciarDestruicao();
        }
        
    }
    void IniciarDestruicao()
    {
        _velocidade = 0;
        if(GetComponent<Animator>() != null)
        {
            Animator _animator = GetComponent<Animator>();
            _animator.SetBool("Destruir", true);
        }
        if(GetComponent<ParticleSystem>() != null)
        {
            ParticleSystem _particula = GetComponent<ParticleSystem>();
            _particula.Stop();
        }
        if(GetComponentInChildren<ParticleSystem>() != null)
        {
            ParticleSystem[] _particulas = GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem _particula in _particulas)
            {
                _particula.Stop();
            }
            AutoDestruir();
        }
    }
    public new void AutoDestruir()
    {
        Debug.Log("projetil auto destruindo");
        Destroy(gameObject);
    }
    public void AutoDestruir(float _tempo)
    {
        Destroy(gameObject, _tempo);
    }

    public void Defletir(Ser_Vivo novoDono, float danoRefletido)
    {
        int layerNovoDono = novoDono.gameObject.layer;
        int layerAntigoDono = _dono.gameObject.layer;

        _dono = novoDono;

        // Recalcula o dano percentual
        float danoBase = _dano / 100f * Utilidades.NivelAtualTipoDano(_tipoDano, novoDono);
        _dano = (danoRefletido / danoBase) * 100f;

        //Debug.Log($"rotação atual projetil: {transform.rotation}");
        // Inverte a direção
        transform.rotation = Quaternion.Euler(
            0,
            0,
            transform.eulerAngles.z + 180f
        );
        //Debug.Log($"rotação nova projetil: {transform.rotation}");

        // ❌ remove a layer de quem defletiu
        _alvos &= ~(1 << layerNovoDono);

        // ✅ adiciona a layer do antigo dono
        _alvos |= (1 << layerAntigoDono);

        _colisoes &= ~(1 << layerNovoDono);
        _colisoes |= (1 << layerAntigoDono);
    }



}
