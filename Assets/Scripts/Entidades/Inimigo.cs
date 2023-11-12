using NavMeshPlus.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Inimigo : Ser_Vivo
{
    private NavMeshAgent _agent;
    protected override void Awake()
    {
        base.Awake();
        _alvo = FindAnyObjectByType<Player>().gameObject;
    }
    protected override void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Andar();
        AtacarPlayer();
        
    }
    public void TravarAgent(bool _travar)
    {
        if(_travar)
        {
            _agent.enabled = false;
        }
        else
        {
            _agent.enabled = true;
            _agent.SetDestination(_alvo.transform.position);
        }
    }
    void Andar()
    {
        _agent.SetDestination(_alvo.transform.position);
    }
    void AtacarPlayer()
    {
        _mao.GetComponent<Animator>().SetInteger("Ataque", 0);
        if (_agent.velocity.x == 0 & _agent.velocity.y == 0)
        {
            if (_mao.GetComponent<Mao>()._mirandoAlvo)
            {
                if (_mao.GetComponent<Mao>()._ataquesDisponiveis.Count != 0)
                {
                    int _numeroAtq = Random.Range(0, _mao.GetComponent<Mao>()._ataquesDisponiveis.Count);
                    _mao.GetComponent<Animator>().SetInteger("Ataque", _mao.GetComponent<Mao>()._ataques[_numeroAtq].GetComponent<Ataque>()._idAtaque);
                }
            }
        }
    }


}
