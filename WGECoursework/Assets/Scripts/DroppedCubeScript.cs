using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCubeScript : MonoBehaviour
{
    public Block type;
    Mesh mesh;

    public float floatHeight;
    public float floatForce;

    BoxCollider collider;
    Rigidbody rb;
    VoxelGenerator voxelGenerator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
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
        Ray ray = new Ray(collider.bounds.center, -this.transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, floatHeight, LayerMask.GetMask("Blocks"))) {
            Debug.DrawLine(collider.bounds.center, collider.bounds.center - (this.transform.up * floatHeight));

            rb.AddForce(this.transform.up * floatForce, ForceMode.Acceleration);
            rb.transform.eulerAngles += new Vector3(0, 1, 0);
        } else
        {
            rb.AddForce(-this.transform.up * floatForce, ForceMode.Acceleration);
        }
    }
}
