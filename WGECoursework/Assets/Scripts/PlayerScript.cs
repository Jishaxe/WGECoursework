using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerScript : MonoBehaviour
{
    // The block "shadow" outline
    public GameObject blockShadowPrefab;

    // Player inventory
    public InventoryScript inventory;

    // If this is zero then we're not holding any blocks
    int blockToPlace = 0;

    GameObject blockShadow;
    // The offset that needs to be added to the bottom left corner to bring it to the center
    Vector3 blockShadowOffset = new Vector3(0.5f, 0.261f, 0.4f);

    // The corner of the empty space that the player is looking at right now, if any
    Vector3 blockPlacementPoint;

    // The block that the player is looking at right now
    BlockData currentSelectedBlock;

    VoxelChunk currentChunk;

    FirstPersonController fps;
    Rigidbody rb;

    public delegate void BlockPlacementEvent(int blockType, Vector3 location);
    public static event BlockPlacementEvent OnBlockPlacement;
    public static event BlockPlacementEvent OnBlockRemoval;

    // Start is called before the first frame update
    void Start()
    {

        fps = GetComponent<FirstPersonController>();
        rb = GetComponent<Rigidbody>();

        // unlock cursor at the start
        fps.m_MouseLook.m_cursorIsLocked = false;
        blockShadow = Instantiate(blockShadowPrefab);
        blockShadow.SetActive(false);


        // freeze game at start
        Time.timeScale = 0;

        ChunkLoader.OnFinishLoading += OnFinishedLoading;

        HotbarScript.OnHotbarSelectionChanged += OnHotbarSelectionChange;
    }

    public void OnFinishedLoading(float progress)
    {
        // unfreeze game once loaded
        fps.m_MouseLook.m_cursorIsLocked = true;
        Time.timeScale = 1;
    }

    public void OnHotbarSelectionChange(int selected)
    {
        blockToPlace = selected;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit raycastHit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        if (Physics.Raycast(ray, out raycastHit, 4f, LayerMask.GetMask("Blocks")))
        {
            // Get the chunk for this block
            currentChunk = raycastHit.collider.gameObject.GetComponent<VoxelChunk>();

            // Find the position of the block local to its chunk
            raycastHit.point = currentChunk.transform.InverseTransformPoint(raycastHit.point);

            // Subtracting half the normal from the point will always yield a point inside the block, which we can then floor to get the corner of the block
            Vector3 cornerOfBlock = raycastHit.point - (raycastHit.normal / 2);
            cornerOfBlock = new Vector3(Mathf.Floor(cornerOfBlock.x),
                                              Mathf.Floor(cornerOfBlock.y),
                                              Mathf.Floor(cornerOfBlock.z));

            // Check if there is a block already occupying this position
            // probably would be quicker to not get the chunkscript every update but we don't know if we gonna support multiple chunks

            currentSelectedBlock = currentChunk.GetBlockAt(cornerOfBlock);
            currentSelectedBlock.DrawDebugLines();

            blockPlacementPoint = cornerOfBlock;


            
            // If there is already a block at this point, add on the raycast normal so it pushes the selection box to the next empty space
            // as every block is one unit, this means we can just add on the normal with no changes and it works perfectly :)
            if (currentSelectedBlock.type != 0)
            {
                blockPlacementPoint = raycastHit.point + (raycastHit.normal / 2);
                blockPlacementPoint = new Vector3(Mathf.Floor(blockPlacementPoint.x), Mathf.Floor(blockPlacementPoint.y), Mathf.Floor(blockPlacementPoint.z));
                Debug.DrawLine(cornerOfBlock, blockPlacementPoint, Color.yellow);
            }

            // place the block shadow
            blockShadow.SetActive(true);
            blockShadow.transform.position = currentChunk.transform.TransformPoint(blockPlacementPoint);

            if (fps.m_MouseLook.m_cursorIsLocked)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    if (blockToPlace != 0) PlaceBlock(blockPlacementPoint, currentChunk, blockToPlace);
                }
                if (Input.GetMouseButtonUp(0)) DigBlock(currentSelectedBlock, currentChunk);
            }

        } else
        {
            blockShadow.SetActive(false);
        }

    }

    private void OnDrawGizmos()
    {
        if (currentChunk == null) return;
        Gizmos.DrawSphere(currentChunk.transform.TransformPoint(blockPlacementPoint), 0.1f);
    }

    void PlaceBlock(Vector3 position, VoxelChunk chunk, int blockType)
    {
        Debug.Log("Adding block at " + position + ", currently: " + chunk.GetBlockAt(position).type);

        if (inventory.GetBlockCount((Block)blockType - 1) == 0) return; // Check we actually have blocks of this type to place

        // Don't place if there is already a block at this position
        if (chunk.GetBlockAt(position).type != 0) return;

        // Don't place if the new block would collide with the player

        if (Physics.OverlapBox(currentChunk.transform.TransformPoint(position) + new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("Player")).Length > 0) return;
        chunk.AddBlock(new BlockData
        {
            x = (int)position.x,
            y = (int)position.y,
            z = (int)position.z,
            type = blockType
        });

        // Pass in the placed block type (AudioManager uses this to determine which placing sound to place)
        OnBlockPlacement(blockType, chunk.transform.TransformPoint(position));
        chunk.BuildChunk();
    }

    void DigBlock(BlockData block, VoxelChunk chunk)
    {
        Debug.Log("Removing block " + block.ToString());
        if (block.type == 0) return;
        chunk.RemoveBlockAt(new Vector3(block.x, block.y, block.z));

        // Pass in the destroyed block type (AudioManager uses this to determine which destroying sound to place)
        OnBlockRemoval(block.type, chunk.transform.TransformPoint(new Vector3(block.x, block.y, block.z)));

        chunk.BuildChunk();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DroppedCube")
        {
            DroppedCubeScript droppedCube = other.gameObject.GetComponent<DroppedCubeScript>();
            droppedCube.FlyTowards(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DroppedCube")
        {
            Destroy(collision.gameObject);
        }
    }
}

