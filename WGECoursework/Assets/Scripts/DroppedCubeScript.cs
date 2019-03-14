using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCubeScript : MonoBehaviour
{
    public Block type;
    Mesh mesh;

    VoxelGenerator voxelGenerator;

    // Start is called before the first frame update
    void Start()
    {
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
        
    }
}
