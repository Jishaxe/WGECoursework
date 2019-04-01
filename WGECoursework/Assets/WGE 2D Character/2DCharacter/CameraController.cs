using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    float initialZ;
    public float tightness = 0.25f;

    void Start()
    {
        initialZ = this.transform.position.z;
    }

    void Update()
    {
        if (target == null) return;
        Vector3 lerped = Vector3.Lerp(this.transform.position, target.transform.position, tightness);
        lerped.z = initialZ;
        this.transform.position = lerped;
    }
}
