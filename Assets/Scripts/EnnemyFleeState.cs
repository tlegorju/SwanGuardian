using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnnemyFleeState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }
    private SteeringBehavior steeringBehavior;
    private FieldOfView fov;

    public float stateDuration = 5;
    private float startTimeState;

    EnnemyStateScriptableObject stateData;

    public EnnemyFleeState(StateMachine owner, SteeringBehavior steering, EnnemyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        fov = owner.GetComponent<FieldOfView>();
        this.stateData = stateData;
    }

    public void Enter()
    {
        owner.GetComponent<EnnemyController>().UpdateEnnemyMaterial(this.GetType());
        owner.GetComponent<EnnemyController>().MAX_VELOCITY = stateData.stateSpeed;

        startTimeState = Time.time;
    }

    public Type Execute()
    {
        if (Time.time >= startTimeState + stateDuration)
            return typeof(EnnemyWanderState);

        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= stateData.fleeDistance)
            {
                steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(stateData.fleeDistance), .3f);
            }
        }
        steeringBehavior.AddForce(steeringBehavior.Flee(SwanController.Instance.transform.position), .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);
        return GetType();
    }

    public void Exit()
    {

    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, stateData.fleeDistance);


        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= stateData.fleeDistance)
            {
                Handles.DrawLine(owner.transform.position, agentTab[i].GetPosition());
            }
        }
    }
}
