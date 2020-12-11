using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoid
{
    Vector3 GetPosition();
    void Translate(Vector3 displacement);

    Vector3 GetVelocity();
    float GetMaxVelocity();
    float GetMass();
}
