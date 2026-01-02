using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public bool ativo;
    public float intervalo = 0.03f;
    public float tempoVida = 0.15f;
    public float alphaInicial = 0.6f;

    private Ser_Vivo _serVivo;
    private float _timer;

    void Awake()
    {
        _serVivo = GetComponentInParent<Ser_Vivo>();
    }

    void Update()
    {
        if (!ativo || _serVivo == null) return;

        _timer += Time.deltaTime;
        if (_timer >= intervalo)
        {
            _timer = 0f;
            CriarGhost();
        }
    }

    void CriarGhost()
    {
        foreach (var membro in _serVivo.Membros)
        {
            // Aqui você instancia um GhostSprite
            // copiando sprite, posição e rotação
        }
    }
}
