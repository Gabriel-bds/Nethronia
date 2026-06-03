using UnityEngine;

public class GerenciadorCamadasCorpo : MonoBehaviour
{
    private Ser_Vivo _serVivo;
    private MembroCamadas[] _membros;

    private void Start()
    {
        _serVivo = GetComponent<Ser_Vivo>();
        _membros = GetComponentsInChildren<MembroCamadas>();
        _serVivo.OnVidaAlterada += AtualizarMembros;
    }

    private void OnDestroy()
    {
        if (_serVivo != null)
            _serVivo.OnVidaAlterada -= AtualizarMembros;
    }

    private void AtualizarMembros(Ser_Vivo sv, float vidaAtual, float vidaMax)
    {
        float dano = 1f - (vidaAtual / vidaMax);
        foreach (MembroCamadas membro in _membros)
            membro.DefinirDano(dano);
    }
}
