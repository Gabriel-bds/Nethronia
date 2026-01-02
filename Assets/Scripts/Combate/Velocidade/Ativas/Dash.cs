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
    Vector2 _direcaoDash;
    bool _ativo;

    protected override void Start()
    {
        //base.Start();
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

        // Posiciona o dash exatamente no player
        transform.localPosition = Vector3.zero;

        // Direção até o mouse
        Vector2 alvo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _direcaoDash = (alvo - (Vector2)_serVivo.transform.position).normalized;

        IniciarDash();
    }

    void IniciarDash()
    {
        _ativo = true;

        // Trava ações normais
        _serVivo.TravarCorpoMao(1);
        _serVivo.Invulneravel(1);

        // Zera movimento anterior
        _rigidbody.velocity = Vector2.zero;

        // Impulso (mesma lógica do knockback)
        _rigidbody.AddForce(_direcaoDash * _forcaDash, ForceMode2D.Impulse);

        StartCoroutine(TempoDash());
    }

    IEnumerator TempoDash()
    {
        yield return new WaitForSeconds(_tempoDash);
        EncerrarDash();
    }

    void EncerrarDash()
    {
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

        // Colisão com parede → encerra dash
        if (((1 << col.gameObject.layer) & _layerParede) != 0)
        {
            EncerrarDash();
            return;
        }
        base.OnTriggerEnter2D (col);
    }
}
