using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    StateMachine Owner { get; }

    void Enter();
    Type Execute();
    void Exit();

    void OnDrawGizmos();
}
