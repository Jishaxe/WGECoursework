using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConversationScript : MonoBehaviour
{
    // the conversation to use for this interaction
    public TextAsset conversation;

    public Animator attentionBubbleAnimator;

    // how to zoom the camera when in conversation
    public float zoomLevel;

    private bool _isInConversation = false;
    private PlayerMovement2D _targetPlayer;
    private CameraController _targetCamera;
    private float _prevZoomLevel; // zoom level of cam before we entered conversation

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // if the player entered the trigger
        if (collision.GetComponent<PlayerMovement2D>() != null)
        {
            StartConversationWith(collision.GetComponent<PlayerMovement2D>());
        }
    }

    public void StartConversationWith(PlayerMovement2D player)
    {
        Debug.Log("Starting conversation with player");

        player.StartConversationWith(this);
        attentionBubbleAnimator.Play("hide");

        _isInConversation = false;
        _targetPlayer = player;
        _targetCamera = Camera.main.GetComponent<CameraController>();
        _prevZoomLevel = _targetCamera.zoomLevel;
        _targetCamera.zoomLevel = zoomLevel;

        SwitchFocusToNPC();
    }

    public void SwitchFocusToPlayer()
    {
        _targetCamera.target = _targetPlayer._cameraTarget.transform;
    }

    public void SwitchFocusToNPC()
    {
        _targetCamera.target = this.transform;
    }

    public void EndConversation()
    {
        _isInConversation = false;

        // restore camera to player
        _targetCamera.zoomLevel = _prevZoomLevel;
        SwitchFocusToPlayer();

        _targetPlayer.EndConversation();
        _targetPlayer = null;
        _targetCamera = null;



        attentionBubbleAnimator.Play("show");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
