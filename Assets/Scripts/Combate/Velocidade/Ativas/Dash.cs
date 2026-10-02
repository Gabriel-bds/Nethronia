using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ataque
{
    [Header("Dash")]
    [SerializeField] float _forcaDash = 25f;
    [SerializeField] float _tempoDash = 0.25f;
    [Tooltip("Quantos dashes seguidos podem ser feitos antes de entrar em recarga")]
    [SerializeField] int _dashesConsecutivos = 1;

    static readonly Dictionary<Ser_Vivo, int> _dashesSeguidosPorDono = new Dictionary<Ser_Vivo, int>();
    static readonly Dictionary<Ser_Vivo, float> _ultimoDashPorDono = new Dictionary<Ser_Vivo, float>();

    [Header("Colisão")]
    [SerializeField] LayerMask _layerParede;

    Ser_Vivo _serVivo;
    Rigidbody2D _rigidbody;

    Vector2 _alvo;
    Vector2 _direcaoDash;

    float _distanciaAnterior;
    bool _ativo;

    protected override void Start()
    {
        transform.SetParent(_dono.transform);
        transform.localPosition = Vector3.zero;

        _serVivo = GetComponentInParent<Ser_Vivo>();
        if (_serVivo == null)
        {
            Debug.LogError("Dash precisa estar como filho de um Ser_Vivo");
            Destroy(gameObject);
            return;
        }

        _rigidbody = _serVivo._rigidbody;

        _alvo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 origem = _serVivo.transform.position;
        _direcaoDash = (_alvo - origem).normalized;
        _distanciaAnterior = Vector2.Distance(origem, _alvo);

        IniciarDash();
        if (!UsarSemRecarregar())
            ControlarRecarga();
    }

    // Enquanto houver dashes consecutivos sobrando, o dash continua disponivel sem recarga
    bool UsarSemRecarregar()
    {
        if (_dashesConsecutivos <= 1) return false;

        if (_ultimoDashPorDono.TryGetValue(_dono, out float ultimoDash) && Time.time - ultimoDash > Mathf.Max(_tempoRecargaTotal, 1f))
            _dashesSeguidosPorDono[_dono] = 0;
        _ultimoDashPorDono[_dono] = Time.time;

        _dashesSeguidosPorDono.TryGetValue(_dono, out int dashesSeguidos);
        dashesSeguidos++;

        if (dashesSeguidos >= _dashesConsecutivos)
        {
            _dashesSeguidosPorDono[_dono] = 0;
            return false;
        }

        _dashesSeguidosPorDono[_dono] = dashesSeguidos;
        return true;
    }

    void IniciarDash()
    {
        _dono.GetComponent<GhostTrail>().ativo = true;

        _ativo = true;

        _serVivo.TravarCorpoMao(1);
        _serVivo.Invulneravel(1);

        _rigidbody.linearVelocity = Vector2.zero;

        // IMPULSO FÍSICO
        _rigidbody.AddForce(_direcaoDash * _forcaDash, ForceMode2D.Impulse);

        StartCoroutine(TempoDash());
    }

    void FixedUpdate()
    {
        if (!_ativo) return;

        float distanciaAtual =
            Vector2.Distance(_serVivo.transform.position, _alvo);

        // Passou do alvo ou chegou muito perto
        if (distanciaAtual > _distanciaAnterior || distanciaAtual <= 0.05f)
        {
            // Clamp exato no mouse
            _serVivo.transform.position = _alvo;
            EncerrarDash();
        }
        else
        {
            _distanciaAnterior = distanciaAtual;
        }
    }

    IEnumerator TempoDash()
    {
        yield return new WaitForSeconds(_tempoDash);
        EncerrarDash();
    }

    void EncerrarDash()
    {
        _dono.GetComponent<GhostTrail>().ativo = false;

        if (!_ativo) return;

        _ativo = false;

        _rigidbody.linearVelocity = Vector2.zero;

        _serVivo.TravarCorpoMao(0);
        _serVivo.Invulneravel(0);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!_ativo) return;

        if (((1 << col.gameObject.layer) & _layerParede) != 0)
        {
            EncerrarDash();
            return;
        }

        base.OnTriggerEnter2D(col);
    }
}
