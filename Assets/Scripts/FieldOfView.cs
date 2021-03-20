using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Transform fovOrigin;
    public Transform FovOrigin { get { return fovOrigin; } }
    [SerializeField] private float angle = 180;
    [SerializeField] private float radius = 5;
    public float Radius { get { return radius; } }

    int layerMask = int.MaxValue;

    public event Action<Transform> OnEnterFOV = delegate { };
    public event Action<Transform> OnExitFOV = delegate { };


    private float halfAngle=0;
    public float HalfAngle { get { return halfAngle; } }

    public List<Transform> transformInSight { get; private set; }

    private void Awake()
    {
        halfAngle = angle / 2;
        transformInSight = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider[] inShightCollider = Physics.OverlapSphere(fovOrigin.position, radius, layerMask);
        if (inShightCollider.Length == 0)
        {
            transformInSight.Clear();
            return;
        }

        List<Transform> frameObjectInSight = new List<Transform>();


        for(int i=0; i<inShightCollider.Length; i++)
        {
            Vector3 objectDirection = inShightCollider[i].transform.position - fovOrigin.position;
            float dotResult = Vector3.Dot(fovOrigin.forward, objectDirection);

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

    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireArc(fovOrigin.position,
                            fovOrigin.up,
                            Quaternion.Euler(0, 0, 1)* fovOrigin.forward,
                            angle,
                            radius);
    }

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
