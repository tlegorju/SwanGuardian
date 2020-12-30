using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BabySwanController : MonoBehaviour, IBoid
{
    public bool alive = true;

    [SerializeField] public float MAX_VELOCITY = 5;
    [SerializeField] private Vector3 velocity = new Vector3(0, 0, 1);
    [SerializeField] private float mass = 10;

    //private Vector3 steeringForce= Vector3.zero;
    //[SerializeField] float maxForce = 30;

    //[Header("FOLLOW STATE")]
    //public float followDistance = 5;

    //[Header("FLEE STATE")]
    //public float fleeDistance = 4;
    //public float maxFleeDistance = 10;

    //[Header("WANDER STATE")]
    //private Vector3 wanderForce = Vector3.zero;
    //public float turnChance = 0.05f;
    //public float circleRadius = 1;
    //public float circleDistance = 1;

    [SerializeField] SteeringBehavior steeringBehavior;
    [SerializeField] BabyStateMachine stateMachine;


     public int ObstaclesMask { get; private set; }

    //public enum BabyState
    //{
    //    Following,
    //    Fleeing,
    //    Wandering
    //};

    //public BabyState babyState;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.UpdateStateMachine();

        Vector3 steering = steeringBehavior.ComputeSteeringAndReset() / GetMass();

        velocity = Vector3.ClampMagnitude(velocity + steering, GetMaxVelocity());

        GetComponent<NavMeshAgent>().Move(velocity * Time.deltaTime);

        //Translate(velocity * Time.deltaTime);
        if(velocity.normalized!=Vector3.zero)
            transform.forward = velocity.normalized;
    }

    //private BabyState UpdateState()
    //{
    //    for(int i=0;i<ennemies.Length; i++)
    //    {
    //        if (Vector3.Distance(transform.position, ennemies[i].position) < fleeDistance)
    //            return BabyState.Fleeing;
    //    }

    //    if (leader!=null && Vector3.Distance(transform.position, leader.position) < followDistance)
    //        return BabyState.Following;

    //    return BabyState.Wandering;
    //}

    //private Vector3 Follow()
    //{
    //    return (leader.position - GetPosition()).normalized * MAX_VELOCITY;
    //}

    //private Vector3 Flee()
    //{
    //    Vector3 steering=Vector3.zero;
    //    for(int i=0; i<ennemies.Length; i++)
    //    {
    //        float distance = Vector3.Distance(GetPosition(), ennemies[i].position);
    //        if (distance > maxFleeDistance)
    //            continue;
    //        steering += (GetPosition() - ennemies[i].position).normalized / distance;
    //    }
    //    return (steering).normalized * MAX_VELOCITY;
    //}

    //private Vector3 Wander()
    //{
    //    if (Random.value < turnChance)
    //    {
    //        Vector3 circleCenter = velocity.normalized * circleDistance;

    //        Vector3 randomPoint = Random.insideUnitCircle;//was UnitCircle
    //        randomPoint = new Vector3(randomPoint.x, 0, randomPoint.y);
    //        Vector3 displacement = randomPoint * circleRadius;
    //        displacement = Quaternion.LookRotation(velocity) * displacement;

    //        wanderForce = circleCenter + displacement;
    //    }
    //    return wanderForce;
    //}

    public void Dies()
    {
        BabySwanManager.Instance.OnBabyDies(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Nest"))
        {
            BabySwanManager.Instance.OnBabySaved(this);
            Destroy(this);
        }
        else if(other.gameObject.CompareTag("Rat"))
        {
            stateMachine.SetState(typeof(BabyFleeState));
        }
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
}
