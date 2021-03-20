using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BabyState", menuName = "ScriptableObjects/BabyState", order = 1)]
public class BabyStateScriptableObject : ScriptableObject
{
    [Header("Wander")]
    public float turnChance = 0.05f;
    public float circleDistance = 1;
    public float circleRadius = 1;

    [Header("Flee")]
    public float fleeDistance = 5;

    [Header("Follow")]
    public float maxFollowDistance = 20;
    public float closeEnoughDistance = 3;

    [Header("Layer")]
    public LayerMask OBSTACLES_MASK;

    [Header("State")]
    public float stateSpeed = 10;
    public Color stateColor = Color.yellow;
}
