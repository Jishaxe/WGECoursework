using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct BlockData
{
    int x, y, z;
    int type;
}

public class VoxelChunk : MonoBehaviour {
    VoxelGenerator voxelGenerator;
    int[,,] terrainArray;
    int chunkSize = 16;

    public List<BlockData> blockData = new List<BlockData>();

	// Use this for initialization
	void Start () {
        voxelGenerator = GetComponent<VoxelGenerator>();
        terrainArray = new int[chunkSize, chunkSize, chunkSize];

        voxelGenerator.Initialize();
        //InitializeTerrainFromData();
        //CreateTerrain();
        //voxelGenerator.UpdateMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitializeTerrainFromData()
    {
        for (int x = 0; x < terrainArray.GetLength(0); x++)
        {
            for (int y = 0; y < terrainArray.GetLength(1); y++)
            {
                for (int z = 0; z < terrainArray.GetLength(2); z++)
                {
                    terrainArray[x, y, z] = 1; // Assign the texture
                }
            }
        }

    }

    // Create the individual vertices for the terrainArray
    void BuildTerrain()
    {
        voxelGenerator.Clear();

        // iterate horizontally on width
        for (int x = 0; x < terrainArray.GetLength(0); x++)
        {
            // iterate vertically
            for (int y = 0; y < terrainArray.GetLength(1); y++)
            {
                // iterate per voxel horizontally on depth
                for (int z = 0; z < terrainArray.GetLength(2); z++)
                {
                    // if this voxel is not empty
                    if (terrainArray[x, y, z] != 0)
                    {
                        Block tex;
                        // set texture name by value
                        switch (terrainArray[x, y, z])
                        {
                            case 1:
                                tex = Block.GRASS;
                                break;
                            case 2:
                                tex = Block.DIRT;
                                break;
                            case 3:
                                tex = Block.SAND;
                                break;
                            case 4:
                                tex = Block.STONE;
                                break;
                            default:
                                tex = Block.GRASS;
                                break;
                        }

                        Vector2 uvs = voxelGenerator.Block2UV(tex);
                        //voxelGenerator.CreateVoxel(x, y, z, tex);

                        // check if we need to draw the positive x face
                        if (x == 0 || terrainArray[x - 1, y, z] == 0)
                        {
                            voxelGenerator.CreateNegativeXFace(x, y, z, uvs);
                        }
                        // check if we need to draw the positive x face
                        if (x == terrainArray.GetLength(0) - 1 || terrainArray[x + 1, y, z] == 0)
                        {
                            voxelGenerator.CreatePositiveXFace(x, y, z, uvs);
                        }

                        // check if we need to draw the negative y face
                        if (y == 0 || terrainArray[x, y - 1, z] == 0)
                        {
                            voxelGenerator.CreateNegativeYFace(x, y, z, uvs);
                        }
                        // check if we need to draw the positive y face
                        if (y == terrainArray.GetLength(1) - 1 || terrainArray[x, y + 1, z] == 0)
                        {
                            voxelGenerator.CreatePositiveYFace(x, y, z, uvs);
                        }

                        // check if we need to draw the negative z face
                        if (z == 0 || terrainArray[x, y, z - 1] == 0)
                        {
                            voxelGenerator.CreateNegativeZFace(x, y, z, uvs);
                        }
                        // check if we need to draw the positive z face
                        if (z == terrainArray.GetLength(2) - 1 || terrainArray[x, y, z + 1] == 0)
                        {
                            voxelGenerator.CreatePositiveZFace(x, y, z, uvs);
                        }
                    }
                }
            }
        }
    }
}

