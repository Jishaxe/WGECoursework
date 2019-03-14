using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    // The positioning in this array is used to match sounds to blocks - the index of a given AudioClip is the same as a block type int
    public AudioClip[] blockRemovalSounds;
    public AudioClip[] blockPlacementSounds;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Attach to the player's events
        PlayerScript.OnBlockPlacement += OnBlockPlacement;
        PlayerScript.OnBlockRemoval += OnBlockRemoval;
    }


    void OnBlockRemoval(int type)
    {
        // Play the relevant removal sound
        audioSource.PlayOneShot(blockRemovalSounds[type]);
    }

    void OnBlockPlacement(int type)
    {
        // Play the relevant placement sounds
        audioSource.PlayOneShot(blockPlacementSounds[type]);
    }
}
