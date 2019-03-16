using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCubeScript : MonoBehaviour
{
    public delegate void OnDroppedCubeEvent(Block type);
    public static event OnDroppedCubeEvent OnDroppedCubePickup;

    public Block type;
    Mesh mesh;

    public float floatHeight;
    public float floatForce;
    public float pullForce;
    public float distanceBeforeCollecting;

    BoxCollider collid;
    Rigidbody rb;
    VoxelGenerator voxelGenerator;

    // If this is null the cube isn't flying towards anything
    public GameObject flyTowardsTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collid = GetComponent<BoxCollider>();
        mesh = GetComponent<MeshFilter>().mesh;
        voxelGenerator = GetComponent<VoxelGenerator>();
        voxelGenerator.Initialize();

        Vector2 uvs = VoxelGenerator.Block2UV(type);
        voxelGenerator.CreateNegativeXFace(0, 0, 0, uvs);
        voxelGenerator.CreatePositiveXFace(0, 0, 0, uvs);

        voxelGenerator.CreateNegativeYFace(0, 0, 0, uvs);
        voxelGenerator.CreatePositiveYFace(0, 0, 0, uvs);

        voxelGenerator.CreateNegativeZFace(0, 0, 0, uvs);
        voxelGenerator.CreatePositiveZFace(0, 0, 0, uvs);

        voxelGenerator.UpdateMesh();
    }

    void Update()
    {
        // Spin a little bit
        rb.transform.eulerAngles += new Vector3(0, 1, 0);

        // if we have a target to fly towards, do that
        if (flyTowardsTarget != null)
        {
            rb.AddForce((flyTowardsTarget.transform.position - this.transform.position) * pullForce);
            float distance = (flyTowardsTarget.transform.position - this.transform.position).magnitude;
            if (distance < distanceBeforeCollecting)
            {
                if (OnDroppedCubePickup != null) OnDroppedCubePickup(type);
                Destroy(this.gameObject);
            }
        }

        // otherwise just bob up and down
        Ray ray = new Ray(collid.bounds.center, -this.transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, floatHeight, LayerMask.GetMask("Blocks")))
        {
            Debug.DrawLine(collid.bounds.center, collid.bounds.center - (this.transform.up * floatHeight));

            rb.AddForce(this.transform.up * floatForce, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(-this.transform.up * floatForce, ForceMode.Acceleration);
        }
    }

    public void FlyTowards(GameObject gameObject)
    {
        flyTowardsTarget = gameObject;
        floatHeight *= 2;
    }
}
