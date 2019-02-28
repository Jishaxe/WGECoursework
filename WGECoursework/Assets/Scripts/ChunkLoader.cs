using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public GameObject voxelChunkPrefab;

    public void LoadChunk(string fileName)
    {
        XmlReader reader = XmlReader.Create(fileName);

        GameObject chunk = Instantiate(voxelChunkPrefab);
        VoxelChunk chunkScript = chunk.GetComponent<VoxelChunk>();
        chunkScript.Initialize();

        List<BlockData> blocks = new List<BlockData>();

        BlockData block = new BlockData
        {
            x = 0,
            y = 0,
            z = 0,
            type = 1
        };

        blocks.Add(block);

        chunkScript.InitializeTerrainFromData(blocks.ToArray());
        chunkScript.BuildChunk();
    }

    private void Start()
    {
        LoadChunk("AssessmentChunk1.xml");
    }
}
