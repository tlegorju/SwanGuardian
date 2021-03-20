using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnnemyIdleState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }
    private SteeringBehavior steeringBehavior;
    private PerimeterController perimeterController;
    private FieldOfView fov;

    Vector3 wanderForce;

    EnnemyStateScriptableObject stateData;

    public EnnemyIdleState(StateMachine owner, SteeringBehavior steering, PerimeterController perimeterController, EnnemyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.perimeterController = perimeterController;
        fov = owner.GetComponent<FieldOfView>();
        this.stateData = stateData;
    }

    public void Enter()
    {
        owner.GetComponent<EnnemyController>().UpdateEnnemyMaterial(this.GetType());
        owner.GetComponent<EnnemyController>().MAX_VELOCITY = stateData.stateSpeed;
    }

    public Type Execute()
    {
        wanderForce = steeringBehavior.Wander(stateData.turnChance, stateData.circleDistance, stateData.circleRadius, wanderForce);
        steeringBehavior.AddForce(wanderForce, .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fov.Radius, fov.HalfAngle*2), .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);
        steeringBehavior.AddForce(steeringBehavior.Follow(perimeterController.transform.position), Vector3.Distance(owner.transform.position, perimeterController.transform.position) / (perimeterController.radius*100) );

        return this.GetType();
    }

    public void Exit()
    {
        
    }

    public void OnDrawGizmos()
    {
        //Handles.color = Color.yellow;
        //Handles.DrawWireDisc(owner.transform.position+owner.transform.forward*circleDistance, owner.transform.up, circleRadius);
        //Handles.DrawLine(owner.transform.position, owner.transform.position + wanderForce);

        //Handles.color = Color.red;
        //owner.transform.Rotate(Vector3.up, -FIELD_OF_VIEW / 2);
        //Handles.DrawWireArc(owner.transform.position,
        //                    owner.transform.up,
        //                    owner.transform.forward,
        //                    FIELD_OF_VIEW,
        //                    AVOID_DISTANCE);
        //owner.transform.Rotate(Vector3.up, FIELD_OF_VIEW / 2);

        for (float i = fov.HalfAngle / 10; i < fov.HalfAngle; i += fov.HalfAngle / 10)
        {
            Vector3 dir = (Quaternion.AngleAxis(i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position+dir* fov.Radius);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), fov.Radius, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * fov.Radius);
        }
    }
}
