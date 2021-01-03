using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BabyDraggedState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }

    private SteeringBehavior steeringBehavior;

    public float fleeDistance = 5;
    public LayerMask OBSTACLES_MASK;

    public const float STATE_SPEED = 0;
    public Color STATE_COLOR = Color.black;

    public BabyDraggedState(StateMachine owner, SteeringBehavior steering)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
    }

    public void Enter()
    {
        owner.GetComponent<BabySwanController>().UpdateBabyMaterial(this.GetType());
        OBSTACLES_MASK = owner.GetComponent<BabySwanController>().ObstaclesMask;
        owner.GetComponent<BabySwanController>().MAX_VELOCITY = STATE_SPEED;

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
