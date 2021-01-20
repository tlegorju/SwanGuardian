using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BabySwanController : MonoBehaviour, IBoid
{
    public bool alive = true;
    private float life = 1;
    public float Life { get { return life; } }
    public event Action<float> OnLoseLife = delegate { };

    [SerializeField] public float MAX_VELOCITY = 5;
    [SerializeField] private Vector3 velocity = new Vector3(0, 0, 1);
    [SerializeField] private float mass = 10;


    [SerializeField] SteeringBehavior steeringBehavior;
    [SerializeField] BabyStateMachine stateMachine;
    [SerializeField] FieldOfView Fov;


    public int ObstaclesMask { get; private set; }

    public Transform leader;
    //public Transform[] ennemies;


    //Temp
    [SerializeField] private Material babyMaterial;
    public Material BabyMaterial { get { return babyMaterial; } set { babyMaterial = value; } }
    public Material[] StateMaterials;
    private SkinnedMeshRenderer renderer;
    private Material[] materials;


    //private void OnDrawGizmos()
    //{

    //    switch (babyState)
    //    {
    //        case BabyState.Following:
    //            Handles.color = Color.green;
    //            Handles.DrawWireDisc(transform.position, transform.up, followDistance);
    //            break;
    //        case BabyState.Fleeing:
    //            Handles.color = Color.red;
    //            Handles.DrawWireDisc(transform.position, transform.up, fleeDistance);
    //            for (int i = 0; i < ennemies.Length; i++)
    //            {
    //                if (Vector3.Distance(GetPosition(), ennemies[i].position) > maxFleeDistance)
    //                    continue;
    //                Handles.DrawLine(transform.position, ennemies[i].position);
    //            }
    //            break;
    //        case BabyState.Wandering:
    //            Handles.color = Color.yellow;
    //            Handles.DrawWireDisc(transform.position+velocity.normalized*circleDistance, transform.up, circleRadius);
    //            Handles.DrawLine(transform.position, transform.position + wanderForce);
    //            break;
    //    }
    //}

    void Awake()
    {
        if (stateMachine != null)
            stateMachine = GetComponent<BabyStateMachine>();
        if (steeringBehavior != null)
            steeringBehavior = GetComponent<SteeringBehavior>();
        stateMachine.Initialize(typeof(BabyFollowState), leader, steeringBehavior);

        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        materials = renderer.materials;

        ObstaclesMask = LayerMask.GetMask("Obstacle");

        if (Fov == null)
            Fov = GetComponent<FieldOfView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Fov.OnEnterFOV += OnSeenSmth;
        Fov.OnExitFOV += OnLostSightOfSmth;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.UpdateStateMachine();

        if (GetMaxVelocity() <= 0)
            return;

        Vector3 steering = steeringBehavior.ComputeSteeringAndReset() / GetMass();

        velocity = Vector3.ClampMagnitude(velocity + steering, GetMaxVelocity());

        GetComponent<NavMeshAgent>().Move(velocity * Time.deltaTime);

        //Translate(velocity * Time.deltaTime);
        if(velocity.normalized!=Vector3.zero)
            transform.forward = velocity.normalized;
    }

    public void Dies()
    {
        BabySwanManager.Instance?.OnBabyDies(this);
        Destroy(gameObject, 1);
    }

    public void StartBeingDragged()
    {
        stateMachine.SetState(typeof(BabyDraggedState));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Nest"))
        {
            BabySwanManager.Instance.OnBabySaved(this);
            Destroy(this);
        }
        //else if(other.gameObject.CompareTag("Rat"))
        //{
        //    stateMachine.SetState(typeof(BabyFleeState));
        //}
    }

    public bool LoseLife(float damages) //Damages between 0 & 1
    {
        life -= damages;
        OnLoseLife(life);

        if (life<=0)
        {
            Dies();
            return true;
        }
        return false;
    }

    public void UpdateBabyMaterial(Color newColor)
    {
        babyMaterial.color = newColor;
        Debug.Log("Color : " + babyMaterial.color);
        materials[0] = babyMaterial;
        renderer.materials = materials;
    }

    public void UpdateBabyMaterial(Type state)
    {
        if (state == typeof(BabyFollowState))
            materials[0] = StateMaterials[0];
        else if (state == typeof(BabyWanderState))
            materials[0] = StateMaterials[1];
        else if(state == typeof(BabyFleeState))
            materials[0] = StateMaterials[2];
        else if(state == typeof(BabySafeState))
            materials[0] = StateMaterials[3];
        else if(state == typeof(BabyDeadState))
            materials[0] = StateMaterials[4];

        renderer.materials = materials;
    }


    private void OnSeenSmth(Transform obj)
    {
        if (obj.GetComponent<EnnemyController>())
        {
            stateMachine.SetState(typeof(BabyFleeState));
        }
    }

    private void OnLostSightOfSmth(Transform obj)
    {
        //if (target == null)
        //    return;
        //if (obj.Equals(target))
        //{
        //    Debug.Log("LOST TARGET");
        //    target = FindClosestTarget();
        //    Debug.Log("NEW TARGET : " + target.gameObject.name);
        //}
    }




    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Translate(Vector3 displacement)
    {
        transform.Translate(displacement,Space.World);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float GetMaxVelocity()
    {
        return MAX_VELOCITY;
    }

    public float GetMass()
    {
        return mass;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
