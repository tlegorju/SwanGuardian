using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyDeadState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }

    public void Enter()
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {
        throw new NotImplementedException();
    }

    Type IState.Execute()
    {
        throw new NotImplementedException();
    }
}
