using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PerimeterController : MonoBehaviour
{
    public float radius = 5;
    public Color arcColor = Color.blue;
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = arcColor;
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, 360, radius);
    }
#endif
}
