using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BabyWanderState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }
    private SteeringBehavior steeringBehavior;
    private FieldOfView fov;

    Vector3 wanderForce;

    public const float AVOID_DISTANCE = 1;
    public const float FIELD_OF_VIEW = 270;

    private BabyStateScriptableObject stateData;

    public BabyWanderState(StateMachine owner, SteeringBehavior steering, BabyStateScriptableObject stateData)
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
    }

    public Type Execute()
    {
        wanderForce = steeringBehavior.Wander(stateData.turnChance, stateData.circleDistance, stateData.circleRadius, wanderForce);
        steeringBehavior.AddForce(wanderForce, .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(AVOID_DISTANCE, FIELD_OF_VIEW), .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);

        return this.GetType();
    }

    public void Exit()
    {
        
    }


    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(owner.transform.position+owner.transform.forward* stateData.circleDistance, owner.transform.up, stateData.circleRadius);
        Handles.DrawLine(owner.transform.position, owner.transform.position + wanderForce);

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
            Gizmos.DrawLine(owner.transform.position, owner.transform.position+dir*fov.Radius);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * fov.Radius);
        }
#endif
    }
}
