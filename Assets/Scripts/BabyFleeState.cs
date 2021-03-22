using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BabyFleeState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }
    private SteeringBehavior steeringBehavior;
    private FieldOfView fov;

    public float stateDuration = 3;
    private float startTime = 0;

    private BabyStateScriptableObject stateData;

    public BabyFleeState(StateMachine owner, SteeringBehavior steering, BabyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        fov = owner.GetComponent<FieldOfView>();

        this.stateData = stateData;
    }

    public void Enter()
    {
        owner.GetComponent<BabySwanController>().UpdateBabyMaterial(this.GetType());
        owner.GetComponent<BabySwanController>().MAX_VELOCITY = stateData.stateSpeed;

        startTime = Time.time;
    }

    public Type Execute()
    {
        if(Time.time >= startTime+stateDuration)
        {
            return typeof(BabyWanderState);
        }

        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(stateData.fleeDistance),5);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);

        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if(agentTab[i].GetTransform().GetComponent<EnnemyController>())
            {
                steeringBehavior.AddForce(steeringBehavior.Flee(agentTab[i].GetPosition()), 1);
            }
        }
        return this.GetType();
    }

    public void Exit()
    {

    }


    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, stateData.fleeDistance);


        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for(int i=0; i<agentTab.Length; i++)
        {
            if(Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= stateData.fleeDistance)
            {
                Handles.DrawLine(owner.transform.position, agentTab[i].GetPosition());
            }
        }
#endif
    }
}
