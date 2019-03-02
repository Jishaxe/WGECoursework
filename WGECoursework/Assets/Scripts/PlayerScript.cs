using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // The block "shadow" outline
    public GameObject blockShadowPrefab;
    GameObject blockShadow;
    // The offset that needs to be added to the bottom left corner to bring it to the center
    Vector3 blockShadowOffset = new Vector3(0.5f, 0.261f, 0.4f);

    // The corner of the empty space that the player is looking at right now, if any
    Vector3 blockPlacementPoint;

    // The block that the player is looking at right now
    BlockData currentSelectedBlock;

    VoxelChunk currentChunk;

    // Start is called before the first frame update
    void Start()
    {
        blockShadow = Instantiate(blockShadowPrefab);
        blockShadow.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit raycastHit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        if (Physics.Raycast(ray, out raycastHit, 4f, LayerMask.GetMask("Blocks")))
        {
            Vector3 cornerOfBlock = new Vector3(Mathf.Floor(raycastHit.point.x),
                                              Mathf.Floor(raycastHit.point.y),
                                              Mathf.Floor(raycastHit.point.z));

            // Check if there is a block already occupying this position
            // probably would be quicker to not get the chunkscript every update but we don't know if we gonna support multiple chunks
            if (currentChunk == null) currentChunk = raycastHit.collider.gameObject.GetComponent<VoxelChunk>();
            currentSelectedBlock = currentChunk.GetBlockAt(cornerOfBlock);

            // If there is already a block at this point, add on the raycast normal so it pushes the selection box to the next empty space
            // as every block is one unit, this means we can just add on the normal with no changes and it works perfectly :)
            if (currentSelectedBlock.type != 0)
            {
                cornerOfBlock += raycastHit.normal;
            }

            blockPlacementPoint = cornerOfBlock;

            blockShadow.SetActive(true);
            blockShadow.transform.position = cornerOfBlock + blockShadowOffset;


            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("raycast at " + raycastHit.point);
                PlaceBlock(blockPlacementPoint, currentChunk, 1);
            }
            if (Input.GetMouseButtonDown(0)) DigBlock(currentSelectedBlock, currentChunk);

        } else
        {
            blockShadow.SetActive(false);
        }

    }

    void PlaceBlock(Vector3 position, VoxelChunk chunk, int blockType)
    {
        Debug.Log("Adding block at " + position + ", currently: " + chunk.GetBlockAt(position).type);
        // Don't place if there is already a block at this position
        if (chunk.GetBlockAt(position).type != 0) return;

        // Don't place if the new block would collide with the player

        if (Physics.OverlapBox(position + new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("Player")).Length > 0) return;
        chunk.AddBlock(new BlockData
        {
            x = (int)position.x,
            y = (int)position.y,
            z = (int)position.z,
            type = blockType
        });

        chunk.BuildChunk();
    }

    void DigBlock(BlockData block, VoxelChunk chunk)
    {
        Debug.Log("Removing block " + block.ToString());
        if (block.type == 0) return;
        chunk.RemoveBlockAt(new Vector3(block.x, block.y, block.z));

        chunk.BuildChunk();
    }
}

