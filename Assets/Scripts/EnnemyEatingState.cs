using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnnemyEatingState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }

    private SteeringBehavior steeringBehavior;

    public float fleeDistance = 5;
    public LayerMask OBSTACLES_MASK;

    public const float STATE_SPEED = 11;
    public Color STATE_COLOR = Color.red;

    public float stateDuration = 5;
    private float startTimeState;

    public EnnemyEatingState(StateMachine owner, SteeringBehavior steering)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
    }

    public void Enter()
    {
        owner.GetComponent<EnnemyController>().UpdateEnnemyMaterial(this.GetType());
        OBSTACLES_MASK = owner.GetComponent<EnnemyController>().ObstaclesMask;
        owner.GetComponent<EnnemyController>().MAX_VELOCITY = STATE_SPEED;

        startTimeState = Time.time;
    }

    public Type Execute()
    {
        if (Time.time >= startTimeState + stateDuration)
            return typeof(EnnemyWanderState);

        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= fleeDistance)
            {
                steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fleeDistance), 1);
            }
        }
        steeringBehavior.AddForce(steeringBehavior.Flee(SwanController.Instance.transform.position), 1);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(5, OBSTACLES_MASK, 180 / 2), 5f);
        return GetType();
    }

    public void Exit()
    {

    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, fleeDistance);


        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= fleeDistance)
            {
                Handles.DrawLine(owner.transform.position, agentTab[i].GetPosition());
            }
        }
    }
}
