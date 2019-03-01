using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public GameObject voxelChunkPrefab;

    public void LoadChunk(string fileName)
    {
        // We'll put our parsed blocks in here
        List<BlockData> blocks = new List<BlockData>();

 
        XmlReader reader = XmlReader.Create(fileName);

        // While we can still read elements
        while (reader.Read())
        {
            if (!reader.IsStartElement() || reader.Name != "Voxel") continue; // Ignore if it's not a <Voxel> start tag

            int x, y, z;
            int type;

            // Read in the xyz
            x = int.Parse(reader["x"]);
            y = int.Parse(reader["y"]);
            z = int.Parse(reader["z"]);

            // Now step to the text content and parse that
            reader.Read();
            type = int.Parse(reader.Value.Trim());

            // Add to our block list
            BlockData block = new BlockData
            {
                x = x,
                y = y,
                z = z,
                type = type
            };

            blocks.Add(block);
        }

        Debug.Log("Loaded " + blocks.Count + " blocks from file " + fileName);

        // Instansiate the chunk gameobject and initialize the chunkscript
        GameObject chunk = Instantiate(voxelChunkPrefab);
        VoxelChunk chunkScript = chunk.GetComponent<VoxelChunk>();
        chunkScript.Initialize();

        // toss the blocks into it and build
        chunkScript.InitializeTerrainFromData(blocks.ToArray());
        chunkScript.BuildChunk();
    }

    private void Start()
    {
        LoadChunk("AssessmentChunk2.xml");
    }
}
