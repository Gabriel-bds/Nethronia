using UnityEngine;

// Ricochete Perfeito (Comum): projeteis rebatidos pelo dono (Peito de Aco) atravessam os inimigos.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Resistencia/Ricochete Perfeito")]
public class RicochetePerfeito : DadosHabilidadePassiva
{
    [Header("Ricochete Perfeito:")]
    [SerializeField] private GameObject _particulaRicochete;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<RicochetePerfeito>, IReageProjetilRefletido
    {
        public Logica(RicochetePerfeito dados) : base(dados) { }

        public void AoRefletirProjetil(Projetil projetil)
        {
            projetil.PermitirAtravessarInimigos();
            InstanciarParticula(_dados._particulaRicochete, projetil.transform.position, 1f, projetil.transform);
        }
    }
}
