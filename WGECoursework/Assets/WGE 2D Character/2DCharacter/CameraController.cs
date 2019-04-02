using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public PlayerMovement2D player;
    float initialZ;
    public float tightness = 0.25f;
    public float shakeAmount = 0.25f;

    private void Awake()
    {
        player.OnPlayerLand += OnPlayerLand;
    }
     
    public void OnPlayerLand()
    {
        StartCoroutine(CameraShake());
    }

    IEnumerator CameraShake()
    {
        float shake = shakeAmount;

        while (shake > 0.05f) {
            offset = new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), 0);
            shake /= 2;
            yield return null;
        }

        offset = Vector3.zero;
    }

    void Start()
    {
        initialZ = this.transform.position.z;
    }

    void Update()
    {
        if (target == null) return;
        Vector3 lerped = Vector3.Lerp(this.transform.position, target.transform.position, tightness);
        lerped.z = initialZ;
        this.transform.position = lerped + offset;
    }
}
