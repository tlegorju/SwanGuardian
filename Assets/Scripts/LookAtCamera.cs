using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Camera cameraToLookAt;

    private void Awake()
    {
        if (cameraToLookAt == null)
            cameraToLookAt = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(-cameraToLookAt.transform.forward, cameraToLookAt.transform.up);
        //transform.forward = -cameraToLookAt.transform.forward;
        //transform.rotation = cameraToLookAt.transform.rotation;
    }
}
