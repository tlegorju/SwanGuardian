﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnnemyChaseState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }
    private SteeringBehavior steeringBehavior;
    private FieldOfView fov;

    private Transform targetToFollow;

    EnnemyStateScriptableObject stateData;

    public EnnemyChaseState(StateMachine owner, SteeringBehavior steering, EnnemyStateScriptableObject stateData)
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
        targetToFollow = owner.GetComponent<EnnemyController>().target;
    }

    public Type Execute()
    {
        if (targetToFollow == null)
            return typeof(EnnemyWanderState);

        if (Vector3.Distance(targetToFollow.position, Owner.transform.position) <= stateData.closeEnoughDistance)
            return typeof(EnnemyEatingState);

        steeringBehavior.AddForce(steeringBehavior.Follow(targetToFollow.position), .7f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fov.Radius, fov.HalfAngle*2), .3f);

        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);

        return this.GetType();
    }

    public void Exit()
    {

    }

    public void SetTarget(Transform target)
    {
        targetToFollow = target;
    }

    public void OnDrawGizmos()
    {
        //Handles.color = Color.green;
        //Handles.DrawWireDisc(owner.transform.position, owner.transform.up, MAX_FOLLOW_DISTANCE);

        //Handles.color = Color.red;
        //owner.transform.Rotate(Vector3.up, -FIELD_OF_VIEW / 2);
        //Handles.DrawWireArc(owner.transform.position,
        //                    owner.transform.up,
        //                    owner.transform.forward,
        //                    FIELD_OF_VIEW,
        //                    AVOID_DISTANCE);
        //owner.transform.Rotate(Vector3.up, FIELD_OF_VIEW / 2);


        for (float i = 90 / 10; i < 90; i += 90 / 10)
        {
            Vector3 dir = (Quaternion.AngleAxis(i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * fov.Radius);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * fov.Radius);
        }
    }
}
