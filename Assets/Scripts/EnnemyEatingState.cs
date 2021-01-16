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

    public float fleeDistance = 5;
    public LayerMask OBSTACLES_MASK;

    public const float STATE_SPEED = 5f;
    public Color STATE_COLOR = Color.red;

    public const float DAMAGES_PER_SECONDS = .2f;

    public EnnemyEatingState(StateMachine owner, SteeringBehavior steering, Transform MouthTransform)
    {
        this.owner = owner;
        this.steeringBehavior = steering;
        this.mouthTransform = MouthTransform;
    }

    public void Enter()
    {
        controller = owner.GetComponent<EnnemyController>();
        controller.UpdateEnnemyMaterial(this.GetType());
        OBSTACLES_MASK = owner.GetComponent<EnnemyController>().ObstaclesMask;
        controller.MAX_VELOCITY = STATE_SPEED;

        eatenTarget = controller.target;
        targetController = eatenTarget.GetComponent<BabySwanController>();
        targetController.StartBeingDragged();
    }

    public Type Execute()
    {
        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (agentTab[i].GetTransform() == eatenTarget)
                continue;
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= fleeDistance)
            {
                steeringBehavior.AddForce(steeringBehavior.AvoidAllAgent(fleeDistance), .5f);
            }
        }
        steeringBehavior.AddForce(steeringBehavior.Flee(SwanController.Instance.transform.position), 1);
        steeringBehavior.AddForce(steeringBehavior.AvoidObstacles(5, OBSTACLES_MASK, 180 / 2), 5f);

        if (targetController.LoseLife(DAMAGES_PER_SECONDS * Time.deltaTime))
            return typeof(EnnemyWanderState);
        eatenTarget.position = Vector3.MoveTowards(eatenTarget.position, mouthTransform.position, .5f);

        return GetType();
    }

    public void Exit()
    {

    }

    public void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(owner.transform.position, owner.transform.up, fleeDistance);


        IBoid[] agentTab = SteeringBehavior.GetAllAgent();
        for (int i = 0; i < agentTab.Length; i++)
        {
            if (Vector3.Distance(owner.transform.position, agentTab[i].GetPosition()) <= fleeDistance)
            {
                Handles.DrawLine(owner.transform.position, agentTab[i].GetPosition());
            }
        }
    }
}
