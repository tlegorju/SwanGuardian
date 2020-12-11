using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyController : MonoBehaviour, IBoid
{
    [SerializeField] float MAX_VELOCITY = 5;
    private Vector3 velocity=new Vector3(0,0,1);
    private float mass = 10;

    private Vector3 steeringForce= Vector3.zero;
    [SerializeField] float maxForce = 30;

    public float followDistance = 3;
    public float fleeDistance = 4;
    private Vector3 wanderForce = Vector3.zero;

    bool eating=false;

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

    //Temp
    public Material ennemyMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ennemyState = UpdateState();

        switch(ennemyState)
        {
            case EnnemyState.Chasing:
                ennemyMaterial.color = Color.red;
                steeringForce = Chase();
                break;
            case EnnemyState.Fleeing:
                ennemyMaterial.color = Color.white;
                steeringForce = Flee();
                break;
            case EnnemyState.Wandering:
                ennemyMaterial.color = Color.yellow;
                steeringForce = Wander();
                break;
        }

        if(steeringForce!=Vector3.zero)
            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce) / mass;
        velocity = Vector3.ClampMagnitude(velocity + steeringForce, MAX_VELOCITY);

        Translate(velocity * Time.deltaTime);
        transform.forward = velocity.normalized;
    }

    private EnnemyState UpdateState()
    {
        if (ennemyState == EnnemyState.Fleeing)
            return EnnemyState.Fleeing;

        for (int i=0;i<targets.Length; i++)
        {
            if (targets[i] == null)
                continue;
            if (Vector3.Distance(transform.position, targets[i].position) < fleeDistance)
            {
                currentTarget = targets[i];
                return EnnemyState.Chasing;
            }
        }

        return EnnemyState.Wandering;
    }

    private Vector3 Chase()
    {
        return (currentTarget.position - GetPosition()).normalized * MAX_VELOCITY;
    }

    private Vector3 Flee()
    {
        return (GetPosition() - Swan.position).normalized * MAX_VELOCITY;
    }

    private Vector3 Wander()
    {
        float turnChance = 0.05f;
        float circleRadius = 1;
        float circleDistance = 1;
        if (Random.value < turnChance)
        {
            Vector3 circleCenter = velocity.normalized * circleDistance;

            Vector3 randomPoint = Random.insideUnitCircle;//was UnitCircle
            randomPoint = new Vector3(randomPoint.x, 0, randomPoint.y);
            Vector3 displacement = randomPoint * circleRadius;
            displacement = Quaternion.LookRotation(velocity) * displacement;


            wanderForce = circleCenter + displacement;
        }
        return wanderForce;
    }

    public void FleeSwan(Transform swan)
    {
        this.Swan = swan;
        ennemyState = EnnemyState.Fleeing;

        Invoke("StartWandering", 5);
    }

    private void StartWandering()
    {
        ennemyState = EnnemyState.Wandering;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(!eating && other.gameObject.GetComponent<BabySwanController>())
        {
            StartCoroutine(StartEating(other.gameObject));
        }
    }

    IEnumerator StartEating(GameObject obj)
    {
        eating = true;
        float startTime = Time.time;
        while(Time.time<startTime+3)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) > 1.5f)
            {
                eating = false;
                break;
            }
            yield return null;
        }
        if(eating)
        {
            obj.GetComponent<BabySwanController>().Dies();
            UpdateState();
            eating = false;
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
}
