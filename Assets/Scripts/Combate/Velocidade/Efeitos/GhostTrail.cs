using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [Header("Ativação")]
    public bool ativo;

    [Header("Configurações")]
    public float intervalo = 0.03f;
    public float tempoVida = 0.15f;
    public float alphaInicial = 0.6f;

    private Ser_Vivo _serVivo;
    private float _timer;

    void Awake()
    {
        _serVivo = GetComponent<Ser_Vivo>();

        if (_serVivo == null)
        {
            Debug.LogError("GhostTrailController precisa estar no mesmo GameObject do Ser_Vivo.");
        }
    }

    void Update()
    {
        if (!ativo || _serVivo == null) return;

        _timer += Time.deltaTime;
        if (_timer >= intervalo)
        {
            _timer = 0f;
            CriarGhostPose();
        }
    }

    void CriarGhostPose()
    {
        foreach (Rigidbody2D membro in _serVivo._membros)
        {
            if (membro == null) continue;

            SpriteRenderer sr = membro.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) continue;

            CriarGhostSprite(membro, sr);
        }
    }

    void CriarGhostSprite(Rigidbody2D membro, SpriteRenderer originalSR)
    {
        GameObject ghost = new GameObject("GhostSprite");

        ghost.transform.position = membro.transform.position;
        ghost.transform.rotation = membro.transform.rotation;
        ghost.transform.localScale = membro.transform.lossyScale;

        SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();
        ghostSR.sprite = originalSR.sprite;
        ghostSR.sortingLayerID = originalSR.sortingLayerID;
        ghostSR.sortingOrder = originalSR.sortingOrder - 1;

        Color c = originalSR.color;
        c.a = alphaInicial;
        ghostSR.color = c;

        GhostSprite gs = ghost.AddComponent<GhostSprite>();
        gs.Inicializar(ghostSR, tempoVida);
    }
}
