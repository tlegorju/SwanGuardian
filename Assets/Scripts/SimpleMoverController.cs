﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class SimpleMoverController : MonoBehaviour, IBoid
{
    public float speed = 10;
    public float rotationSpeed = 180;

    public float screamRange = 10;

    public Transform screamOrigin;
    public Transform screamWave;
    public float screamMaxSize = 4;

    NavMeshAgent navMeshAgent;


    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, screamRange);
    }


    // Start is called before the first frame update
    void Start()
    {
        if(screamWave)
            screamWave.localScale = Vector3.zero;

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(transform.up, rotationSpeed * Time.deltaTime * horizontal);

        //transform.position += (transform.forward * vertical).normalized * speed * Time.deltaTime;

        navMeshAgent.Move((transform.forward * vertical).normalized * speed * Time.deltaTime);

        if (Input.GetButtonDown("Fire1"))
        {
            Scream();
        }
    }

    private void Scream()
    {
        StartCoroutine(AnimateScream());

        Collider[] colliders = Physics.OverlapSphere(transform.position, screamRange);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.GetComponent<EnnemyController>()?.FleeSwan(transform);
            colliders[i].gameObject.GetComponent<BabyStateMachine>()?.SetState(typeof(BabyFollowState));
        }
    }

    private IEnumerator AnimateScream()
    {
        float startTime = Time.time;
        float screamDuration = .3f;


        if (!screamWave)
            screamDuration = 0;
        else
            screamWave.position = screamOrigin.position;
        while(Time.time<startTime+ screamDuration)
        {
            screamWave.position = screamOrigin.position;
            screamWave.localScale = Vector3.Lerp(Vector3.zero, Vector3.one* screamMaxSize, (Time.time - startTime) / screamDuration);
            yield return null;
        }
        if (screamWave)
        {
            screamWave.position = screamOrigin.position;
            screamWave.localScale = Vector3.one * screamMaxSize;
            yield return new WaitForSeconds(.1f);
            screamWave.localScale = Vector3.zero;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Translate(Vector3 displacement)
    {
        transform.position += displacement;
    }

    public Vector3 GetVelocity()
    {
        return transform.forward * speed;
    }

    public float GetMaxVelocity()
    {
        return speed;
    }

    public float GetMass()
    {
        return 10;
    }
}