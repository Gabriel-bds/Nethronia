using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirObjeto : StateMachineBehaviour
{
    // Chamado quando o estado termina (saiu do blend tree)
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}
