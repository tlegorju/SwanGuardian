using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwanController : MonoBehaviour
{
    private static SwanController instance;
    public static SwanController Instance { get { return instance; } }

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;
    }
}
