using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowScript : MonoBehaviour
{
    public Transform toFollow;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if(toFollow)
        {
            offset = toFollow.position - transform.position;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (toFollow)
            transform.position = toFollow.position - offset;
    }
}
