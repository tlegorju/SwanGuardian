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


    BabySoundController soundController;

    public float footstepRate = .5f;
    public float nextFootstep = 0;

    private bool onLand = false;
    public bool OnLand { get { return onLand; } }

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

        soundController = GetComponent<BabySoundController>();
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
        velocity = (new Vector3(velocity.x, 0, velocity.z)).normalized * velocity.magnitude;

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
        if (velocity.normalized!=Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
    }

    public void Dies()
    {
        alive = false;
        stateMachine.SetState(typeof(BabyDeadState));
        BabySwanManager.Instance?.OnBabyDies(this);
        Destroy(gameObject, 1f);
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
            stateMachine.perimeterController = other.GetComponentInChildren<PerimeterController>();
            stateMachine.SetState(typeof(BabySafeState));
            velocity = Vector3.zero;
            //Destroy(this);
            soundController.ArriveAtNest();
        }
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
        if (obj.GetComponent<EnnemyController>() && stateMachine.CurrentState.GetType()!=typeof(BabyDraggedState) && stateMachine.CurrentState.GetType() != typeof(BabyDeadState))
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
