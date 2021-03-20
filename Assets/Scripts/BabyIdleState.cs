using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BabyIdleState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }
    private SteeringBehavior steeringBehavior;
    private PerimeterController perimeterController;
    private FieldOfView fov;

    Vector3 wanderForce;

    public const float AVOID_DISTANCE = 1;
    public const float FIELD_OF_VIEW = 270;

    private BabyStateScriptableObject stateData;

    public BabyIdleState(StateMachine owner, SteeringBehavior steering, PerimeterController perimeterController, BabyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.perimeterController = perimeterController;
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
        steeringBehavior.AddForce(steeringBehavior.Follow(perimeterController.transform.position), Vector3.Distance(owner.transform.position, perimeterController.transform.position) / (perimeterController.radius*100) );

        return this.GetType();
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(fov.FovOrigin.position + fov.FovOrigin.forward * stateData.circleDistance, fov.FovOrigin.up, stateData.circleRadius);
        Handles.DrawLine(fov.FovOrigin.position, fov.FovOrigin.position + wanderForce);

        for (float i = 90 / 10; i < 90; i += 90 / 10)
        {
            Vector3 dir = (Quaternion.AngleAxis(i, Vector3.up) * fov.FovOrigin.forward).normalized;
            if (!Physics.Raycast(new Ray(fov.FovOrigin.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(fov.FovOrigin.position, fov.FovOrigin.position +dir*fov.Radius);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * fov.FovOrigin.forward).normalized;
            if (!Physics.Raycast(new Ray(fov.FovOrigin.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(fov.FovOrigin.position, fov.FovOrigin.position + dir * fov.Radius);
        }
    }
}
