using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Block
{
    DIRT, SAND, GRASS, STONE
}


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer),
typeof(MeshCollider))]
public class VoxelGenerator : MonoBehaviour {
    Mesh mesh;
    MeshCollider meshCollider;
    List<Vector3> vertexList;
    List<int> triIndexList;
    List<Vector2> UVList;
    public float scale;
    public float sandLimit = 0.2f;
    public float stoneLimit = 0.5f;
    public float grassLimit = 0.8f;

    int numQuads = 0;

    public Vector2 Block2UV(Block type)
    {
        switch (type)
        {
            case Block.GRASS: return new Vector2(0, 0);
            case Block.DIRT: return new Vector2(0, 0.5f);
            case Block.STONE: return new Vector2(0.5f, 0);
            case Block.SAND: return new Vector2(0.5f, 0.5f);
            default: return new Vector2();
        }
    }

    public void Initialize()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        vertexList = new List<Vector3>();
        triIndexList = new List<int>();
        UVList = new List<Vector2>();
    }

    // Clears all vertex data
    public void Clear()
    {
        vertexList.Clear();
        triIndexList.Clear();
        UVList.Clear();
    }

    public void UpdateMesh()
    {
        /*
        for (int x = 0; x < 50; x++)
        {
            for (int z = 0; z < 50; z++)
            {
                float type = Mathf.PerlinNoise((x * scale) + 0.01f, (z * scale) + 0.01f);

                Block blockType = Block.DIRT;

                if (type > sandLimit) blockType = Block.SAND;
                if (type > stoneLimit) blockType = Block.STONE;
                if (type > grassLimit) blockType = Block.GRASS;

                CreateVoxel(x, 0, z, blockType);
            }
        }*/

        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triIndexList.ToArray();
        mesh.uv = UVList.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    // Use this for initialization
    void Start () {
       // Initialize();
        //UpdateMesh();
    }
	
    void CreateVoxel(int x, int y, int z, Vector2 uvCoords)
    {
        CreateNegativeZFace(x, y, z, uvCoords);
        CreatePositiveZFace(x, y, z, uvCoords);
        CreateNegativeXFace(x, y, z, uvCoords);
        CreatePositiveXFace(x, y, z, uvCoords);
        CreatePositiveYFace(x, y, z, uvCoords);
        CreateNegativeYFace(x, y, z, uvCoords);
    }

    public void CreateVoxel(int x, int y, int z, Block block)
    {
        CreateVoxel(x, y, z, Block2UV(block));
    }

    public void CreateNegativeZFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x, y, z));

        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreatePositiveZFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x + 1, y, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreateNegativeXFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x, y, z));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreatePositiveXFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y, z + 1));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreateNegativeYFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x, y, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y, z + 1));

        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }


    public void CreatePositiveYFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x, y + 1, z));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    void AddUVCoords(Vector2 uvCoords)
    {
        UVList.Add(new Vector2(uvCoords.x, uvCoords.y +
       0.5f));
        UVList.Add(new Vector2(uvCoords.x + 0.5f, uvCoords.y +
       0.5f));
        UVList.Add(new Vector2(uvCoords.x + 0.5f,
       uvCoords.y));
        UVList.Add(new Vector2(uvCoords.x, uvCoords.y));
    }


    void AddTriangleIndices()
    {
        triIndexList.Add(numQuads * 4);
        triIndexList.Add((numQuads * 4) + 1);
        triIndexList.Add((numQuads * 4) + 3);
        triIndexList.Add((numQuads * 4) + 1);
        triIndexList.Add((numQuads * 4) + 2);
        triIndexList.Add((numQuads * 4) + 3);
        numQuads++;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown("space"))
        {
            vertexList = new List<Vector3>();
            triIndexList = new List<int>();
            UVList = new List<Vector2>();
            numQuads = 0;

            UpdateMesh();
        }
	}
}
