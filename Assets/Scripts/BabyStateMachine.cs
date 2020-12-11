using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyStateMachine : StateMachine
{
    Dictionary<Type, IState> babyStates = new Dictionary<Type, IState>();


    private void OnDrawGizmos()
    {
        currentState?.OnDrawGizmos();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize(Type defaultState, Transform leader, SteeringBehavior steering)
    {
        babyStates.Add(typeof(BabyFollowState), new BabyFollowState(this, steering, leader));
        babyStates.Add(typeof(BabyFleeState), new BabyFleeState(this, steering));
        babyStates.Add(typeof(BabyWanderState), new BabyWanderState(this, steering));
        babyStates.Add(typeof(BabySafeState), new BabySafeState());
        babyStates.Add(typeof(BabyDeadState), new BabyDeadState());

        babyStates.TryGetValue(typeof(BabyFollowState), out currentState);
        //SetState(defaultState);
    }

    public void UpdateStateMachine()
    {
        if(currentState!=null)
        {
            Type newType = currentState.Execute();
            if (newType != null && newType != currentState.GetType())
                SetState(newType);
        }
    }

    public void SetState(Type state)
    {
        if (currentState != null)
            currentState.Exit();

        babyStates.TryGetValue(state, out currentState);

        if (currentState != null)
            currentState.Enter();
    }
}
