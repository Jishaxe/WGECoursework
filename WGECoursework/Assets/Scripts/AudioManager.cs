using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    // The positioning in this array is used to match sounds to blocks - the index of a given AudioClip is the same as a block type int
    public AudioClip[] blockRemovalSounds;
    public AudioClip[] blockPlacementSounds;
    public AudioClip droppedBlockPickupSound;
    public AudioClip hotbarSelectionChangedSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Attach to the player's events
        PlayerScript.OnBlockPlacement += OnBlockPlacement;
        PlayerScript.OnBlockRemoval += OnBlockRemoval;
        DroppedCubeScript.OnDroppedCubePickup += OnDroppedBlockPickup;
        HotbarScript.OnHotbarSelectionChanged += OnHotbarSelectionChanged;
        HotbarScript.OnSortInventory += OnSortInventory;
    }


    void OnSortInventory(SortInventoryBy by, SortInventoryOrder order)
    {
        audioSource.PlayOneShot(hotbarSelectionChangedSound);
    }

    void OnBlockRemoval(int type, Vector3 position)
    {
        // Play the relevant removal sound
        audioSource.PlayOneShot(blockRemovalSounds[type]);
    }

    void OnBlockPlacement(int type, Vector3 position)
    {
        // Play the relevant placement sounds
        audioSource.PlayOneShot(blockPlacementSounds[type]);
    }

    void OnDroppedBlockPickup(Block type)
    {
        audioSource.PlayOneShot(droppedBlockPickupSound);
    }

    void OnHotbarSelectionChanged(int newSelection)
    {
        audioSource.PlayOneShot(hotbarSelectionChangedSound);
    }
}
