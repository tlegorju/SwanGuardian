using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    float angle = 180;
    float radius = 5;
    int layerMask = int.MaxValue;

    public event Action<Transform> OnEnterFOV = delegate { };
    public event Action<Transform> OnExitFOV = delegate { };


    private float halfAngle=0;

    public List<Transform> transformInSight { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        halfAngle = angle/2;
        transformInSight = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] inShightCollider = Physics.OverlapSphere(transform.position, radius, layerMask);
        if (inShightCollider.Length == 0)
        {
            transformInSight.Clear();
            return;
        }

        List<Transform> frameObjectInSight = new List<Transform>();


        for(int i=0; i<inShightCollider.Length; i++)
        {
            Vector3 objectDirection = inShightCollider[i].transform.position - transform.position;
            float dotResult = Vector3.Dot(transform.forward, objectDirection);

            if ((dotResult>= 0 && halfAngle >= (1 - dotResult) * 90) || (dotResult<0 && halfAngle >= (-dotResult) * 90 + 90))
            {
                frameObjectInSight.Add(inShightCollider[i].transform);
            }
        }
        for(int i=0;i<transformInSight.Count; i++)
        {
            if(!frameObjectInSight.Contains(transformInSight[i]))
            {
                OnExitFOV(transformInSight[i]);
                transformInSight.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < frameObjectInSight.Count; i++)
        {
            if (!transformInSight.Contains(frameObjectInSight[i]))
            {
                OnEnterFOV(frameObjectInSight[i]);
                transformInSight.Add(frameObjectInSight[i]);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    transform.Rotate(Vector3.up, -halfAngle);
    //    Handles.DrawWireArc(transform.position,
    //                        transform.up,
    //                        transform.forward,
    //                        angle,
    //                        radius);
    //    transform.Rotate(Vector3.up, halfAngle);
    //}

    //private void OnGUI()
    //{
    //    string display = "In sight : \n";
    //    for(int i = 0; i< transformInSight.Count; i++)
    //    {
    //        display += transformInSight[i].gameObject.name + "\n";
    //    }
    //    GUI.Label(new Rect(10, 10, 100, 400), display);
    //}
}
