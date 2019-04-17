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

    // the NPC conversation UI
    private NPCConversationUI _ui;
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

        _isInConversation = true;
        _targetPlayer = player;
        _targetCamera = Camera.main.GetComponent<CameraController>();
        
        // save the previous zoom level so we can restore it after
        _prevZoomLevel = _targetCamera.zoomLevel;

        // zoom in on interaction
        _targetCamera.zoomLevel = zoomLevel;

        SwitchFocusToNPC();

        // show the UI
        _ui.Activate();

        _ui.NPCSays("HELLO THERE HOW ARE YOU DOING MY SON", OnPlayerContinue);
    }

    public void OnPlayerContinue()
    {
        PlayerSpeechOption option1 = new PlayerSpeechOption
        {
            playerSays = "Option 1"
        };

        PlayerSpeechOption option2 = new PlayerSpeechOption
        {
            playerSays = "Option 2"
        };

        PlayerSpeechOption option3 = new PlayerSpeechOption
        {
            playerSays = "Option 3"
        };


        _ui.PlayerOptions(new PlayerSpeechOption[] { option1, option2, option3 }, null);
        SwitchFocusToPlayer();
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

        _ui.Deactivate();

        attentionBubbleAnimator.Play("show");
    }

    // Start is called before the first frame update
    void Start()
    {
        _ui = GameObject.FindGameObjectWithTag("NPCConversationUI").GetComponent<NPCConversationUI>();
    }

    // Update is called once per frame
    void Update()
    {

        // debug
        if (Input.GetKeyDown(KeyCode.Escape) && _isInConversation) EndConversation();
    }
}
