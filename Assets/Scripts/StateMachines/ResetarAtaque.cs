using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetarAtaque : StateMachineBehaviour
{

    // Chamado quando o estado termina (saiu do blend tree)
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            animator.SetInteger("Ataque", 0);
        //Debug.Log("Reset");
    }
}
