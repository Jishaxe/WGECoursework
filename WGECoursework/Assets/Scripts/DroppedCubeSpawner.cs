using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCubeSpawner : MonoBehaviour
{
    public GameObject droppedCubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerScript.OnBlockRemoval += OnBlockRemoval;
    }

    void OnBlockRemoval(int type, Vector3 position)
    {
        GameObject droppedCube = Instantiate(droppedCubePrefab);
        droppedCube.transform.position = position + new Vector3(0.5f, 0.5f, 0.5f);

        // -1 because 0 is air but we dont have a type for that
        droppedCube.GetComponent<DroppedCubeScript>().type = (Block)type - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
