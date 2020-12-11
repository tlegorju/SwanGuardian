using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BabyWanderState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }

    SteeringBehavior steeringBehavior;

    public float turnChance = 0.05f;
    public float circleDistance = 1;
    public float circleRadius = 1;

    Vector3 wanderForce;

    public const float AVOID_DISTANCE = 1;
    public const float FIELD_OF_VIEW = 270;
    public LayerMask OBSTACLES_MASK;

    public const float STATE_SPEED = 3;
    public Color STATE_COLOR = Color.yellow;

    public BabyWanderState(StateMachine owner, SteeringBehavior steering)
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
        wanderForce = steeringBehavior.Wander(turnChance, circleDistance, circleRadius, wanderForce);
        steeringBehavior.AddForce(wanderForce, .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(AVOID_DISTANCE, FIELD_OF_VIEW), .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(5, OBSTACLES_MASK, 180 / 2), 5f);

        return this.GetType();
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(owner.transform.position+owner.transform.forward*circleDistance, owner.transform.up, circleRadius);
        Handles.DrawLine(owner.transform.position, owner.transform.position + wanderForce);

        Handles.color = Color.red;
        owner.transform.Rotate(Vector3.up, -FIELD_OF_VIEW / 2);
        Handles.DrawWireArc(owner.transform.position,
                            owner.transform.up,
                            owner.transform.forward,
                            FIELD_OF_VIEW,
                            AVOID_DISTANCE);
        owner.transform.Rotate(Vector3.up, FIELD_OF_VIEW / 2);

        for (float i = 90 / 10; i < 90; i += 90 / 10)
        {
            Vector3 dir = (Quaternion.AngleAxis(i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), 5, OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position+dir*5);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), 5, OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * 5);
        }
    }
}
