using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnnemyEatingState : IState
{
    private StateMachine owner;
    public StateMachine Owner { get { return owner; } }

    private EnnemyController controller;

    private SteeringBehavior steeringBehavior;
    private Transform mouthTransform;
    private Transform eatenTarget;
    private BabySwanController targetController;

    EnnemyStateScriptableObject stateData;

    public const float DAMAGES_PER_SECONDS = .2f;

    public EnnemyEatingState(StateMachine owner, SteeringBehavior steering, Transform MouthTransform, EnnemyStateScriptableObject stateData)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.mouthTransform = MouthTransform;
        this.stateData = stateData;
    }

    public void Enter()
    {
        controller = owner.GetComponent<EnnemyController>();
        controller.UpdateEnnemyMaterial(this.GetType());
        controller.MAX_VELOCITY = stateData.stateSpeed;

        eatenTarget = controller.target;
        targetController = eatenTarget.GetComponent<BabySwanController>();
        targetController.StartBeingDragged();

        owner.GetComponent<EnnemySoundController>().StartEating();
    }

    public Type Execute()
    {
        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        //for (int i = 0; i < agentTab.Length; i++)
        //{
        //    if (agentTab[i].GetTransform() == eatenTarget)
        //        continue;
        //    if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= fleeDistance)
        //    {
        //        steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fleeDistance), .5f);
        //    }
        //}
        steeringBehavior.AddForce(steeringBehavior.Flee(SwanController.Instance.transform.position), 1);
        //steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(5, OBSTACLES_MASK, 180 / 2), 5f);

        if (targetController.LoseLife(DAMAGES_PER_SECONDS * Time.deltaTime))
            return typeof(EnnemyIdleState);
        eatenTarget.position = Vector3.MoveTowards(eatenTarget.position, mouthTransform.position, .5f);

        return GetType();
    }

    public void Exit()
    {
        owner.GetComponent<EnnemySoundController>().StopEating();
    }


    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
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
#endif
    }
}
