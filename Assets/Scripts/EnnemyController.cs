using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemyController : MonoBehaviour, IBoid
{
    [SerializeField] public float MAX_VELOCITY = 5;
    private Vector3 velocity=new Vector3(0,0,1);
    [SerializeField] private float mass = 10;

    private Vector3 steeringForce= Vector3.zero;
    [SerializeField] float maxForce = 30;

    public float followDistance = 3;
    public float fleeDistance = 4;
    private Vector3 wanderForce = Vector3.zero;

    bool eating=false;


    [SerializeField] SteeringBehavior steeringBehavior;
    [SerializeField] EnnemyStateMachine stateMachine;
    [SerializeField] FieldOfView Fov;
    public Transform MouthTransform;

    public int ObstaclesMask { get; private set; }

    [SerializeField] private Material ennemyMaterial;
    public Material EnnemyMaterial { get { return ennemyMaterial; } set { ennemyMaterial = value; } }
    public Material[] StateMaterials;
    private SkinnedMeshRenderer renderer;
    private Material[] materials;

    public Transform target { get; private set; }

    public enum EnnemyState
    {
        Chasing,
        Fleeing,
        Wandering
    };

    public EnnemyState ennemyState;

    public Transform Swan;
    public Transform[] targets;
    private Transform currentTarget;
    private EnnemySoundController soundController;

    public float footstepRate = .5f;
    public float nextFootstep = 0;

    private bool onLand = false;
    public bool OnLand { get { return onLand; } }


    void Awake()
    {
        //ObstaclesMask = LayerMask.GetMask("Obstacle");

        if (stateMachine == null)
            stateMachine = GetComponent<EnnemyStateMachine>();
        if (steeringBehavior == null)
            steeringBehavior = GetComponent<SteeringBehavior>();
        if (Fov == null)
            Fov = GetComponent<FieldOfView>();

        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        materials = renderer?.materials;

        soundController = GetComponent<EnnemySoundController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.Initialize(typeof(EnnemyIdleState), steeringBehavior);

        Fov.OnEnterFOV += OnSeenSmth;
        Fov.OnExitFOV += OnLostSightOfSmth;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.UpdateStateMachine();

        Vector3 steering = steeringBehavior.ComputeSteeringAndReset() / GetMass();

        velocity = Vector3.ClampMagnitude(velocity + steering, GetMaxVelocity());

        GetComponent<NavMeshAgent>().Move(velocity * Time.deltaTime);


        UpdateOnLand();
        if (velocity != Vector3.zero && Time.time > nextFootstep)
        {
            if (onLand)
                soundController.PlayFootstep();
            else
                soundController.PlaySwim();
            nextFootstep = Time.time + footstepRate;
        }

        //Translate(velocity * Time.deltaTime);
        if (velocity.normalized != Vector3.zero)
        {
            if (stateMachine.CurrentState.GetType() != typeof(EnnemyEatingState))
                transform.forward = velocity.normalized;
            else
                transform.forward = -velocity.normalized;
        }
    }


    private void OnSeenSmth(Transform obj)
    {
        if (!target && obj.GetComponent<BabySwanController>())
        {
            target = obj;
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

    private Transform FindClosestTarget()
    {
        float sqrDistanceToTarget = -1;
        Transform currentTarget = null;

        for(int i=0; i<Fov.transformInSight.Count; i++)
        {
            BabySwanController bbController = Fov.transformInSight[i].GetComponent<BabySwanController>();
            if (bbController!=null && bbController.alive)
            {
                if(sqrDistanceToTarget==-1)
                {
                    currentTarget = Fov.transformInSight[i];
                    sqrDistanceToTarget = Vector3.SqrMagnitude(currentTarget.position - transform.position);
                }
                else if(Vector3.SqrMagnitude(Fov.transformInSight[i].position - transform.position)<sqrDistanceToTarget)
                {
                    sqrDistanceToTarget = Vector3.SqrMagnitude(Fov.transformInSight[i].position - transform.position);
                    currentTarget = Fov.transformInSight[i];
                }
            }
        }

        return currentTarget;
    }


    public void Dies()
    {
        //BabySwanManager.Instance.OnBabyDies(this);
        Destroy(gameObject);
    }

    public void UpdateEnnemyMaterial(Color newColor)
    {
        ennemyMaterial.color = newColor;
        Debug.Log("Color : " + ennemyMaterial.color);
        materials[0] = ennemyMaterial;
        renderer.materials = materials;
    }

    public void UpdateEnnemyMaterial(Type state)
    {
        if (StateMaterials.Length == 0)
            return;

        if (state == typeof(EnnemyChaseState))
            materials[0] = StateMaterials[0];
        else if (state == typeof(EnnemyWanderState))
            materials[0] = StateMaterials[1];
        else if (state == typeof(EnnemyFleeState))
            materials[0] = StateMaterials[2];
        else if (state == typeof(EnnemyEatingState))
            materials[0] = StateMaterials[3];
        //else if (state == typeof(EnnemyDeadState))
        //    materials[0] = StateMaterials[4];

        renderer.materials = materials;
    }

    public void FleeSwan()
    {
        if(target)
        {
            target.position = transform.position;
        }
        target = null;
        stateMachine.SetState(typeof(EnnemyFleeState));
    }

    private void UpdateOnLand()
    {
        if (Terrain.activeTerrain.SampleHeight(transform.position) >= 3)
        {
            if (!onLand)
            {
                soundController.ComeOutOfWater();
                onLand = true;
            }
        }
        else
        {
            if (onLand)
            {
                soundController.GetIntoWater();
                onLand = false;
            }
        }
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
