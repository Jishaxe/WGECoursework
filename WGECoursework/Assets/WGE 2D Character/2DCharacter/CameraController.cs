using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public PlayerMovement2D player;
    float initialZ;
    public float xtightness = 0.25f;
    public float ytightness = 0.25f;
    public float shakeAmount = 0.25f;
    public float zoomLevel = 4f;
    Camera cam;

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

        // keep shaking until the shake amount has been reduced to a negligible value
        while (shake > 0.05f) {
            // the calculated offset here is applied to the camera position in FixedUpdate
            offset = new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), 0);
            shake /= 2;
            yield return null;
        }

        offset = Vector3.zero;
    }

    void Start()
    {
        initialZ = this.transform.position.z;
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // work out change in position between the last fixedupdate
        Vector3 delta = target.position - this.transform.position;
        delta.z = 0; // don't change the z

        // smooth out the movement on both sides
        delta.x *= xtightness;
        delta.y *= ytightness;

        float zoomDelta = zoomLevel - cam.orthographicSize;
        zoomDelta *= xtightness;
        cam.orthographicSize += zoomDelta;

        this.transform.position += delta + offset;
    }
}
