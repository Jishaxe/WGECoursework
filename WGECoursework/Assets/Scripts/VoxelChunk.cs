using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public struct BlockData
{
    public int x, y, z;
    public int type;
    public override string ToString()
    {
        return "(" + x + ", " + y + ", " + z + ") - " + type;
    }
    public bool IsInBounds()
    {
        return (x < VoxelChunk.chunkSize && y < VoxelChunk.chunkSize && z < VoxelChunk.chunkSize && x >= 0 && y >= 0 && z >= 0);
    }
    public void DrawDebugLines()
    {
        Debug.DrawLine(new Vector3(x, y, z), new Vector3(x, y + 1, z), Color.green);
        Debug.DrawLine(new Vector3(x, y, z), new Vector3(x + 1, y, z), Color.red);
        Debug.DrawLine(new Vector3(x, y, z), new Vector3(x, y, z + 1), Color.blue);
    }
}

public class VoxelChunk : MonoBehaviour {
    VoxelGenerator voxelGenerator;
    int[,,] terrainArray;
    public static int chunkSize = 16;

    // Mutable block data - call BuildChunk() after modifying
    List<BlockData> blocks = new List<BlockData>();

    // Use this for initialization
    public void Initialize() {
        voxelGenerator = GetComponent<VoxelGenerator>();
        terrainArray = new int[chunkSize, chunkSize, chunkSize];

        voxelGenerator.Initialize();
    }

    // Update the block list and discard any not within the blocksize
    public void SetBlocks(List<BlockData> newBlocks)
    {
        List<BlockData> filteredBlocks = new List<BlockData>();

        foreach (BlockData block in newBlocks)
        {
            if (!block.IsInBounds())
            {
                Debug.Log("Block " + block.x + " " + block.y + " " + block.z + " is outside block dimensions, discarding");
                continue;
            }

            filteredBlocks.Add(block);
        }

        this.blocks = filteredBlocks;
    }

    // Create the individual vertices for the terrainArray from the blocks 
    public void BuildChunk()
    {
        terrainArray = new int[chunkSize, chunkSize, chunkSize];
        voxelGenerator.Clear();

        foreach (BlockData block in blocks)
        {
            if (terrainArray[block.x, block.y, block.z] != 0) Debug.Log(block.ToString() + " ALREADY EXISTS");
            terrainArray[block.x, block.y, block.z] = block.type;
        }

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

                        // get the UVs for this given block type
                        Vector2 uvs = VoxelGenerator.Block2UV(tex);

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

        // Now update the mesh
        voxelGenerator.UpdateMesh();
    }

    // Returns the block data at this position, or a BlockData with type of 0 if none exists
    public BlockData GetBlockAt(Vector3 blockPosition)
    {
        // Type 0 means no block
        int type = 0;
        try
        {
            type = terrainArray[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z];
        }
        catch (IndexOutOfRangeException) { } // Catch the exception we'll hit if it's out of range

        return new BlockData
        {
            x = (int)blockPosition.x,
            y = (int)blockPosition.y,
            z = (int)blockPosition.z,
            type = type
        };
    }

    // Add block to the blocks list, don't forget to call BuildChunk() to update
    public void AddBlock(BlockData block)
    {
        if (!block.IsInBounds())
        {
            Debug.Log("Block " + block.x + " " + block.y + " " + block.z + " is outside block dimensions, not adding");
            return; // Don't add if outside the chunk
        }

        Debug.Log("adding " + block.ToString());
        blocks.Add(block);
    }

    // Remove the block at the given position, don't forget to call BuildChunk() to update
    public void RemoveBlockAt(Vector3 position)
    {
        List<BlockData> filteredBlocks = new List<BlockData>();

        foreach (BlockData block in blocks) if (block.x != (int)position.x || block.y != (int)position.y || block.z != (int)position.z) filteredBlocks.Add(block);
        this.blocks = filteredBlocks;
    }
}

