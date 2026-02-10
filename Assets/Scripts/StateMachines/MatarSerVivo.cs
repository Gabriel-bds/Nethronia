using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatarSerVivo : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var personagem = animator.GetComponent<Ser_Vivo>();
        if (personagem != null)
            personagem._travar = 1;
        personagem._mao.GetComponent<Mao>()._travar = 1;
        personagem._mao.GetComponent<Mao>().ResetarMao();
    }
}
