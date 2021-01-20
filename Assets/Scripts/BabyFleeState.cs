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

    public float fleeDistance = 5;
    public LayerMask OBSTACLES_MASK;

    public const float STATE_SPEED = 11;
    public Color STATE_COLOR = Color.red;

    public float stateDuration = 3;
    private float startTime = 0;

    public BabyFleeState(StateMachine owner, SteeringBehavior steering)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
    }

    public void Enter()
    {
        owner.GetComponent<BabySwanController>().UpdateBabyMaterial(this.GetType());
        OBSTACLES_MASK = owner.GetComponent<BabySwanController>().ObstaclesMask;
        owner.GetComponent<BabySwanController>().MAX_VELOCITY = STATE_SPEED;

        startTime = Time.time;
    }

    public Type Execute()
    {
        if(Time.time >= startTime+stateDuration)
        {
            return typeof(BabyWanderState);
        }

        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fleeDistance),5);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fleeDistance, OBSTACLES_MASK, 180 / 2), 5f);

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
        Handles.color = Color.red;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, fleeDistance);


        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for(int i=0; i<agentTab.Length; i++)
        {
            if(Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= fleeDistance)
            {
                Handles.DrawLine(owner.transform.position, agentTab[i].GetPosition());
            }
        }
    }
}
