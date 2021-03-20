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


    private BabyStateScriptableObject stateData;

    public BabyDraggedState(StateMachine owner, SteeringBehavior steering, BabyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;

        this.stateData = stateData;
    }

    public void Enter()
    {
        owner.GetComponent<BabySwanController>().UpdateBabyMaterial(this.GetType());
        owner.GetComponent<BabySwanController>().MAX_VELOCITY = stateData.stateSpeed;

        Owner.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        Owner.GetComponent<Collider>().enabled = false;
    }

    public Type Execute()
    {
        return this.GetType();
    }

    public void Exit()
    {
        Owner.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        Owner.GetComponent<Collider>().enabled = true;
    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, stateData.fleeDistance);


        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= stateData.fleeDistance)
            {
                Handles.DrawLine(owner.transform.position, agentTab[i].GetPosition());
            }
        }
    }
}
