using UnityEngine;

public class Habilidade_RegeneracaoPassiva : MonoBehaviour
{
    private Ser_Vivo _dono;
    private Regeneracao _regeneracao;

    [SerializeField] private float _valorPorTick = 1f;
    [SerializeField] private float _intervalo = 1f;

    private void Awake()
    {
        _dono = GetComponent<Ser_Vivo>();

        _regeneracao = new Regeneracao(_dono, _valorPorTick, _intervalo, this);

        _dono.OnVidaAlterada += VerificarRegeneracao;
    }

    private void OnDestroy()
    {
        if (_dono != null)
            _dono.OnVidaAlterada -= VerificarRegeneracao;
    }

    private void VerificarRegeneracao(Ser_Vivo sv, float vidaAtual, float vidaMax)
    {
        if (vidaAtual < vidaMax)
        {
            //Debug.Log("Iniciou regeneração");
            _regeneracao.Iniciar();
        }
        else
            _regeneracao.Parar();
    }
}