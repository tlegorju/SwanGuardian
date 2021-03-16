using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    private static List<IBoid> agentList = new List<IBoid>();
    private IBoid boid;

    private Vector3 steeringForce = Vector3.zero;
    [SerializeField] float maxSteeringForce = 30;

    private void Awake()
    {
        boid = GetComponent<IBoid>();
        agentList.Add(boid);
    }

    public void AddForce(Vector3 force, float weight)
    {
        steeringForce += force * weight;
    }

    public Vector3 ComputeSteeringAndReset()
    {
        Vector3 force = steeringForce.normalized * maxSteeringForce;
        steeringForce = Vector3.zero;
        return force;
    }


    private void OnDestroy()
    {
        agentList.Remove(boid);
    }

    public Vector3 Follow(Vector3 leader)
    {
        return (leader - boid.GetPosition()).normalized * boid.GetMaxVelocity();
    }

    public Vector3 Flee(Vector3 fledFrom)
    {
        return (boid.GetPosition() - fledFrom).normalized * boid.GetMaxVelocity();
    }

    public Vector3 Wander(float turnChance = 0.05f, float circleDistance = 1, float circleRadius = 1, Vector3 wanderForce = new Vector3())
    {
        if (Random.value < turnChance)
        {
            Vector3 circleCenter = boid.GetVelocity().normalized * circleDistance;


            Vector3 randomPoint = Random.insideUnitCircle;
            randomPoint = new Vector3(randomPoint.x, 0, randomPoint.y);

            Vector3 displacement = randomPoint * circleRadius;
            displacement = Quaternion.LookRotation(boid.GetVelocity()) * displacement;

            wanderForce = circleCenter + displacement;
            //wanderForce = displacement - boid.GetPosition();
        }
        return wanderForce;
    }

    public Vector3 AvoidAllAgent()
    {
        Vector3 avoidance=new Vector3();
        for(int i=0; i<agentList.Count; i++)
        {
            if(agentList[i]!=null)
            {
                avoidance += boid.GetPosition() - agentList[i].GetPosition();
            }
        }

        return avoidance.normalized * boid.GetMaxVelocity();
    }
    public Vector3 AvoidAllAgent(float maxDistance)
    {
        Vector3 avoidance = new Vector3();
        for (int i = 0; i < agentList.Count; i++)
        {
            if (agentList[i] != null && Vector3.Distance(boid.GetPosition(), agentList[i].GetPosition()) <= maxDistance)
            {
                avoidance += boid.GetPosition() - agentList[i].GetPosition();
            }
        }

        return avoidance.normalized * boid.GetMaxVelocity();
    }
    public Vector3 AvoidAllAgent(float maxDistance, float fieldOfViewAngle)
    {
        float halfAngle = fieldOfViewAngle / 2;
        Vector3 avoidance = new Vector3();
        for (int i = 0; i < agentList.Count; i++)
        {
            if (agentList[i] != null)
            {
                Vector3 towardAgent = agentList[i].GetPosition() - boid.GetPosition();
                if (towardAgent.magnitude <= maxDistance
                    && AngleBetweenVectors(boid.GetVelocity(), towardAgent) <= halfAngle)
                        avoidance -= towardAgent;
            }
        }

        return avoidance.normalized * boid.GetMaxVelocity();
    }

    public Vector3 AvoidObstacles(float avoidDistance, LayerMask obstacleMask, float fieldOfViewHalfAngle, float numberOfFOVSteps=10)
    {
        //Debug.Log(IsHeadingTowardsObstacle(avoidDistance, obstacleMask));
        if(!IsHeadingTowardsObstacle(avoidDistance, obstacleMask))
            return Vector3.zero;

        for(float i= fieldOfViewHalfAngle / numberOfFOVSteps; i<fieldOfViewHalfAngle; i+=fieldOfViewHalfAngle/numberOfFOVSteps)
        {
            Vector3 dir = (Quaternion.AngleAxis(i, Vector3.up) * transform.forward).normalized;
            if (!Physics.Raycast(new Ray(transform.position, dir), avoidDistance, obstacleMask))
                return dir * boid.GetMaxVelocity();

            dir = (Quaternion.AngleAxis(-i, Vector3.up) * transform.forward).normalized;
            if (!Physics.Raycast(new Ray(transform.position, dir), avoidDistance, obstacleMask))
                return dir * boid.GetMaxVelocity();
        }

        return Vector3.zero;
    }

    private bool IsHeadingTowardsObstacle(float avoidDistance, LayerMask obstacleMask)
    {
        return Physics.Raycast(new Ray(transform.position, transform.forward), avoidDistance, obstacleMask);
    }

    public float AngleBetweenVectors(Vector3 a, Vector3 b)
    {
        if (a == Vector3.zero || b == Vector3.zero)
            return 0;
        return Mathf.Acos(Vector3.Dot(a,b)/(a.magnitude*b.magnitude))*Mathf.Rad2Deg;
    }
    
    public static IBoid[] GetAllAgent()
    {
        return agentList.ToArray();
    }
}
