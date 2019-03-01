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

        if (Physics.Raycast(ray, out raycastHit, 10f, LayerMask.GetMask("Blocks")))
        {
            Vector3 cornerOfBlock = new Vector3(Mathf.Floor(raycastHit.point.x),
                                              Mathf.Floor(raycastHit.point.y),
                                              Mathf.Floor(raycastHit.point.z));

            // Check if there is a block already occupying this position
            // probably would be quicker to not get the chunkscript every update but we don't know if we gonna support multiple chunks
            VoxelChunk chunkScript = raycastHit.collider.gameObject.GetComponent<VoxelChunk>();
            BlockData blockAtPoint = chunkScript.GetBlockAt(cornerOfBlock);

            // If there is already a block at this point, add on the raycast normal so it pushes the selection box to the next empty space
            // as every block is one unit, this means we can just add on the normal with no changes and it works perfectly :)
            if (blockAtPoint.type != 0)
            {
                cornerOfBlock += raycastHit.normal;
            }

            blockShadow.SetActive(true);
            blockShadow.transform.position = cornerOfBlock + blockShadowOffset;
        } else
        {
            blockShadow.SetActive(false);
        }
    }
}

