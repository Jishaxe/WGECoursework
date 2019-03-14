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

    int numQuads = 0;

    // Converts a Block enum to a UV coord
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

    // Prep the vars
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
        numQuads = 0;
    }

    // Brings the vertex data over to the mesh filter and recalculates normals
    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triIndexList.ToArray();
        mesh.uv = UVList.ToArray();
        mesh.RecalculateNormals();
        Debug.Log("vertices: " + vertexList.ToArray().Length + ", indices: " + triIndexList.ToArray().Length + ", uvs: " + UVList.ToArray().Length);
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
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

    //.................................................................................................................................
    //.VVVV....VVVVV..OOOOOOO....OXXXX..XXXXX.EEEEEEEEEEE.LLLL............FFFFFFFFFF...AAAAA.......CCCCCCC....CEEEEEEEEEE..SSSSSSS.....
    //.VVVV....VVVV..OOOOOOOOOO...XXXX..XXXX..EEEEEEEEEEE.LLLL............FFFFFFFFFF...AAAAA......CCCCCCCCC...CEEEEEEEEEE.ESSSSSSSS....
    //.VVVV....VVVV.OOOOOOOOOOOO..XXXXXXXXXX..EEEEEEEEEEE.LLLL............FFFFFFFFFF..AAAAAA.....ACCCCCCCCCC..CEEEEEEEEEE.ESSSSSSSSS...
    //.VVVVV..VVVV..OOOOO..OOOOO...XXXXXXXX...EEEE........LLLL............FFFF........AAAAAAA....ACCC...CCCCC.CEEE.......EESSS..SSSS...
    //..VVVV..VVVV.VOOOO....OOOOO...XXXXXX....EEEE........LLLL............FFFF.......AAAAAAAA...AACC.....CCC..CEEE.......EESSS.........
    //..VVVV..VVVV.VOOO......OOOO...XXXXXX....EEEEEEEEEE..LLLL............FFFFFFFFF..AAAAAAAA...AACC..........CEEEEEEEEE..ESSSSSS......
    //..VVVVVVVVV..VOOO......OOOO...XXXXX.....EEEEEEEEEE..LLLL............FFFFFFFFF..AAAA.AAAA..AACC..........CEEEEEEEEE...SSSSSSSSS...
    //...VVVVVVVV..VOOO......OOOO...XXXXXX....EEEEEEEEEE..LLLL............FFFFFFFFF.FAAAAAAAAA..AACC..........CEEEEEEEEE.....SSSSSSS...
    //...VVVVVVVV..VOOOO....OOOOO..XXXXXXXX...EEEE........LLLL............FFFF......FAAAAAAAAAA.AACC.....CCC..CEEE..............SSSSS..
    //...VVVVVVV....OOOOO..OOOOO...XXXXXXXX...EEEE........LLLL............FFFF......FAAAAAAAAAA..ACCC...CCCCC.CEEE.......EESS....SSSS..
    //....VVVVVV....OOOOOOOOOOOO..XXXX.XXXXX..EEEEEEEEEEE.LLLLLLLLLL......FFFF.....FFAA....AAAA..ACCCCCCCCCC..CEEEEEEEEEEEESSSSSSSSSS..
    //....VVVVVV.....OOOOOOOOOO..OXXXX..XXXXX.EEEEEEEEEEE.LLLLLLLLLL......FFFF.....FFAA.....AAAA..CCCCCCCCCC..CEEEEEEEEEE.ESSSSSSSSS...
    //....VVVVV........OOOOOO....OXXX....XXXX.EEEEEEEEEEE.LLLLLLLLLL......FFFF....FFFAA.....AAAA...CCCCCCC....CEEEEEEEEEE..SSSSSSSS....
    //.................................................................................................................................
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

}
