﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyStateMachine : StateMachine
{
    Dictionary<Type, IState> ennemyStates = new Dictionary<Type, IState>();

    public PerimeterController perimeterController;

    [SerializeField] private EnnemyStateScriptableObject idleStateData, chaseStateData, fleeStateData, wanderStateData, eatingStateData;

    private void OnDrawGizmos()
    {
        currentState?.OnDrawGizmos();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize(Type defaultState, SteeringBehavior steering)
    {
        ennemyStates.Add(typeof(EnnemyIdleState), new EnnemyIdleState(this, steering, perimeterController, idleStateData));
        ennemyStates.Add(typeof(EnnemyChaseState), new EnnemyChaseState(this, steering, chaseStateData));
        ennemyStates.Add(typeof(EnnemyFleeState), new EnnemyFleeState(this, steering, fleeStateData));
        ennemyStates.Add(typeof(EnnemyWanderState), new EnnemyWanderState(this, steering, wanderStateData));
        ennemyStates.Add(typeof(EnnemyEatingState), new EnnemyEatingState(this, steering, GetComponent<EnnemyController>().MouthTransform, eatingStateData));
        //ennemyStates.Add(typeof(EnnemyDeadState), new EnnemyDeadState());

        //ennemyStates.TryGetValue(typeof(EnnemyWanderState), out currentState);
        SetState(defaultState);
    }

    public void UpdateStateMachine()
    {
        if (currentState != null)
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

        ennemyStates.TryGetValue(state, out currentState);

        if (currentState != null)
            currentState.Enter();
    }

    private void OnGUI()
    {
        string display = currentState.GetType().ToString();
        GUI.Label(new Rect(10, 10, 100, 400), display);
    }
}
