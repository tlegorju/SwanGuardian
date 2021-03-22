using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnnemyWanderState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }

    private SteeringBehavior steeringBehavior;
    private EnnemyController controller;
    private FieldOfView fov;

    private EnnemyStateScriptableObject stateData;

    Vector3 wanderForce;

    public EnnemyWanderState(StateMachine owner, SteeringBehavior steering, EnnemyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.controller = owner.GetComponent<EnnemyController>();
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
        if (controller.target)
            return typeof(EnnemyChaseState);

        wanderForce = steeringBehavior.Wander(stateData.turnChance, stateData.circleDistance, stateData.circleRadius, wanderForce); ;
        steeringBehavior.AddForce(wanderForce, .3f);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(fov.FovOrigin, fov.Radius, stateData.OBSTACLES_MASK, fov.HalfAngle), 5f);
        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fov.Radius, fov.HalfAngle*2), .3f);

        return this.GetType();
    }

    public void Exit()
    {

    }

    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
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
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), 5, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * 5);

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * owner.transform.forward).normalized;
            if (!Physics.Raycast(new Ray(owner.transform.position, dir), 5, stateData.OBSTACLES_MASK))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(owner.transform.position, owner.transform.position + dir * 5);
        }
#endif
    }
}
