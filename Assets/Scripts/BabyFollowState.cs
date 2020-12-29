﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BabyFollowState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }


    private SteeringBehavior steeringBehavior;
    private Transform leaderToFollow;
    public const float MAX_FOLLOW_DISTANCE = 10;
    public const float CLOSE_ENOUGH_DISTANCE=2;

    public const float AVOID_DISTANCE= 1;
    public const float FIELD_OF_VIEW = 270;
    public LayerMask OBSTACLES_MASK;

    public const float STATE_SPEED = 6;
    public Color STATE_COLOR = Color.green;


    public BabyFollowState(StateMachine owner, SteeringBehavior steering, Transform leader)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.leaderToFollow = leader;
    }

    public void Enter()
    {
        owner.GetComponent<BabySwanController>().UpdateBabyMaterial(this.GetType());
        owner.GetComponent<BabySwanController>().MAX_VELOCITY = STATE_SPEED;
        OBSTACLES_MASK = owner.GetComponent<BabySwanController>().ObstaclesMask;
    }

    public Type Execute()
    {
        if (Vector3.Distance(leaderToFollow.position, Owner.transform.position) > MAX_FOLLOW_DISTANCE)
            return typeof(BabyWanderState);

        if(Vector3.Distance(leaderToFollow.position, Owner.transform.position) >= CLOSE_ENOUGH_DISTANCE)
            steeringBehavior.AddForce(steeringBehavior.Follow(leaderToFollow.position), .7f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(AVOID_DISTANCE, FIELD_OF_VIEW), .3f);

        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(5, OBSTACLES_MASK, 180 / 2), 5f);

        return this.GetType();
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, MAX_FOLLOW_DISTANCE);

        Handles.color = Color.red;
        owner.transform.Rotate(Vector3.up, -FIELD_OF_VIEW/2);
        Handles.DrawWireArc(owner.transform.position, 
                            owner.transform.up,
                            owner.transform.forward, 
                            FIELD_OF_VIEW, 
                            AVOID_DISTANCE);
        owner.transform.Rotate(Vector3.up, FIELD_OF_VIEW/2);


        for (float i = 90 / 10; i < 90; i += 90 / 10)
        {
            Vector3 dir = (Quaternion.AngleAxis(i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), 5, OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * 5);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), 5, OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * 5);
        }
    }
}
