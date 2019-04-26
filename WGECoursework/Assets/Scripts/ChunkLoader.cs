using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public delegate void LoadEvent(float percentage);
    public static event LoadEvent OnStartLoading;
    public static event LoadEvent OnLoadProgress;
    public static event LoadEvent OnFinishLoading;

    List<BlockData> blocks;
    GameObject chunk;

    public GameObject voxelChunkPrefab;

    IEnumerator LoadChunk(string fileName)
    {
        OnStartLoading?.Invoke(0f);

        // We'll put our parsed blocks in here
        blocks = new List<BlockData>();

        FileStream fs = File.OpenRead(fileName);
        XmlReader reader = XmlReader.Create(fs);
        int progressMade = 0;
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

            progressMade++;

            // call the onloadprogress every 100 blocks to avoid slowing it down too much
            if (progressMade > 100)
            {
                progressMade = 0;
                OnLoadProgress?.Invoke((float)fs.Position / (float)fs.Length);
                yield return null;
            }
        }

        reader.Close();
        
        Debug.Log("Loaded " + blocks.Count + " blocks from file " + fileName);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // Instansiate the chunk gameobject and initialize the chunkscript
                chunk = Instantiate(voxelChunkPrefab);
                chunk.transform.position = new Vector3(x * VoxelChunk.chunkSize, 0, y * VoxelChunk.chunkSize);
                VoxelChunk chunkScript = chunk.GetComponent<VoxelChunk>();
                chunkScript.Initialize();


                // toss the blocks into it and build
                chunkScript.SetBlocks(blocks);
                chunkScript.BuildChunk();


            }
        }

        OnFinishLoading?.Invoke(1f);

    }

    public void OnStartLoad(string filename)
    {
        StartCoroutine(LoadChunk(filename));
    }

    private void Start()
    {
        StartScreenScript.OnStartLoad += OnStartLoad;
    }
}


