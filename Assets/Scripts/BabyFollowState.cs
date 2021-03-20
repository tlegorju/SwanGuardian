using System;
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
    private FieldOfView fov;

    public const float AVOID_DISTANCE= 1;
    public const float FIELD_OF_VIEW = 270;

    private BabyStateScriptableObject stateData;

    public BabyFollowState(StateMachine owner, SteeringBehavior steering, Transform leader, BabyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.leaderToFollow = leader;
        fov = owner.GetComponent<FieldOfView>();

        this.stateData = stateData;
    }

    public void Enter()
    {
        owner.GetComponent<BabySwanController>().UpdateBabyMaterial(this.GetType());
        owner.GetComponent<BabySwanController>().MAX_VELOCITY = stateData.stateSpeed;
    }

    public Type Execute()
    {
        if (Vector3.Distance(leaderToFollow.position, Owner.transform.position) > stateData.maxFollowDistance)
            return typeof(BabyWanderState);

        if(Vector3.Distance(leaderToFollow.position, Owner.transform.position) >= stateData.closeEnoughDistance)
            steeringBehavior.AddForce(steeringBehavior.Follow(leaderToFollow.position), .7f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(AVOID_DISTANCE, FIELD_OF_VIEW), .3f);

        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);

        return this.GetType();
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, stateData.maxFollowDistance);

        //Handles.color = Color.red;
        //owner.transform.Rotate(Vector3.up, -FIELD_OF_VIEW/2);
        //Handles.DrawWireArc(owner.transform.position, 
        //                    owner.transform.up,
        //                    owner.transform.forward, 
        //                    FIELD_OF_VIEW, 
        //                    AVOID_DISTANCE);
        //owner.transform.Rotate(Vector3.up, FIELD_OF_VIEW/2);


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
