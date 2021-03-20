using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyStateMachine : StateMachine
{
    Dictionary<Type, IState> babyStates = new Dictionary<Type, IState>();

    public PerimeterController perimeterController;

    [SerializeField] private BabyStateScriptableObject idleStateData, followStateData, fleeStateData, wanderStateData, draggedStateData, safeStateData;

    private void OnDrawGizmosSelected()
    {
        currentState?.OnDrawGizmos();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize(Type defaultState, Transform leader, SteeringBehavior steering)
    {
        babyStates.Add(typeof(BabyIdleState), new BabyIdleState(this, steering, perimeterController, idleStateData));
        babyStates.Add(typeof(BabyFollowState), new BabyFollowState(this, steering, leader, followStateData));
        babyStates.Add(typeof(BabyFleeState), new BabyFleeState(this, steering, fleeStateData));
        babyStates.Add(typeof(BabyWanderState), new BabyWanderState(this, steering, wanderStateData));
        babyStates.Add(typeof(BabyDraggedState), new BabyDraggedState(this, steering, draggedStateData));
        babyStates.Add(typeof(BabySafeState), new BabySafeState(this, steering, safeStateData));
        babyStates.Add(typeof(BabyDeadState), new BabyDeadState(this));

        babyStates.TryGetValue(typeof(BabyIdleState), out currentState);
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
