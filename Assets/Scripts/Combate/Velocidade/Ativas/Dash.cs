using System.Collections;
using UnityEngine;

public class Dash : Ataque
{
    [Header("Dash")]
    [SerializeField] float _forcaDash = 25f;
    [SerializeField] float _tempoDash = 0.25f;

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
        ControlarRecarga();
    }

    void IniciarDash()
    {
        _dono.GetComponent<GhostTrail>().ativo = true;

        _ativo = true;

        _serVivo.TravarCorpoMao(1);
        _serVivo.Invulneravel(1);

        _rigidbody.velocity = Vector2.zero;

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

        _rigidbody.velocity = Vector2.zero;

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
