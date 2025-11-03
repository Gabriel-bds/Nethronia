using UnityEngine;

public class TravarDuranteDano : StateMachineBehaviour
{
    // Chamado quando o estado começa (entrou no blend tree)
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var personagem = animator.GetComponent<Ser_Vivo>();
        if (personagem != null)
            personagem._travar = 1;
            personagem._mao.GetComponent<Mao>()._travar = 1;
    }

    // Chamado quando o estado termina (saiu do blend tree)
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var personagem = animator.GetComponent<Ser_Vivo>();
        if (personagem != null)
            personagem._travar = 0;
            personagem._mao.GetComponent<Mao>()._travar = 0;

    }
}
