using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyDeadState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }


    public BabyDeadState(StateMachine owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        Owner.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        Owner.GetComponent<Collider>().enabled = false;
    }

    public Type Execute()
    {
        return this.GetType();
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {

    }
}
