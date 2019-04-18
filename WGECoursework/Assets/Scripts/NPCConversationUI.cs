using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCConversationUI : MonoBehaviour
{
    public Animator blackBars;
    public Animator dialog;

    public float secondsBetweenTextAnimations = 0.1f;
    public Text conversationText;
    public Text speakerText;

    public AudioClip[] bloops;

    bool focusedOnNPC = true;

    public GameObject playerOptionButtonPrefab;
    public GameObject playerOptions;
    public GameObject pressSpaceToContinue;
    public Vector2 playerOptionStartingPos;
    public float buttonHeight;

    private AudioSource audioSource;
    private bool _animatingInText = false; // whether we are currently animating in text with the AnimateInText corourtine
    private Action OnPlayerContinue; // action to call when the player presses space after text has finished animating
    private Action<PlayerSpeechOption> OnPlayerChoseOption;
    private PlayerSpeechOption chosenOption; // the chosen playerspeechoption 

    public void Activate()
    {
        blackBars.Play("in");
        dialog.Play("in");
    }

    public void Deactivate()
    {
        blackBars.Play("out");
        dialog.Play("out");
    }

    // make the NPC say somthing
    public void NPCSays(string says, Action OnPlayerContinue)
    {
        pressSpaceToContinue.SetActive(true);
        conversationText.text = "";
        speakerText.text = "Sir Redsworth";
        playerOptions.SetActive(false);

        // if we're not already focused on NPC
        if (!focusedOnNPC)
        {
            dialog.Play("npc");
            focusedOnNPC = true;
        }


        this.OnPlayerContinue = OnPlayerContinue;


        StartCoroutine(AnimateInText(says));
    }

    public void PlayerSays(string says)
    {
        conversationText.text = "";
        speakerText.text = "Robert Blueniro";
        pressSpaceToContinue.SetActive(true);
        playerOptions.SetActive(false);

        // if we're not already focused on NPC
        if (focusedOnNPC)
        {
            dialog.Play("player");
            focusedOnNPC = false;
        }


        StartCoroutine(AnimateInText(says));
    }


    // present the player with buttons corresponding with the PlayerSpeechOptions and call the OnPlayerChoseOption action when done
    public void PlayerOptions(PlayerSpeechOption[] options, Action<PlayerSpeechOption> OnPlayerChoseOption)
    {
        chosenOption = null;
        conversationText.text = "";
        speakerText.text = "Robert Blueniro";
        playerOptions.SetActive(true);
        pressSpaceToContinue.SetActive(false);

        this.OnPlayerChoseOption = OnPlayerChoseOption;

        // if we're not already focused on NPC
        if (focusedOnNPC)
        {
            dialog.Play("player");
            focusedOnNPC = false;
        }

        int i = 0;

        foreach (PlayerSpeechOption option in options)
        {
            GameObject optionButton = Instantiate(playerOptionButtonPrefab, playerOptions.transform);
            optionButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(playerOptionStartingPos.x, playerOptionStartingPos.y - (i * buttonHeight));

            optionButton.transform.GetChild(0).GetComponent<Text>().text = option.playerSays;

            // pass in the option and the callback to this script once the button is pressed (it doesn't call the original OnPlayerChoseOption Action until the player has finished speaking
            optionButton.GetComponent<PlayerOptionButtonScript>().RegisterOption(option, OnPlayerClickedOption);

            i++;
        }
    }

    // when player clicks an option button, have the player actually say that 
    public void OnPlayerClickedOption(PlayerSpeechOption option)
    {
        foreach (Transform child in playerOptions.transform) Destroy(child.gameObject); // destroy the option buttons
        chosenOption = option;
        PlayerSays(option.playerSays);
    }

    IEnumerator AnimateInText(string text)
    {
        _animatingInText = true;
        int i = 0;

        do
        {
            audioSource.Stop();
            conversationText.text += text[i];
 

            if (text[i] != ' ')
            {
                // If this letter isn't a space, play a bloop
                audioSource.clip = bloops[UnityEngine.Random.Range(0, bloops.Length)];
                audioSource.Play();
            }

            i++;

            if (i == text.Length) _animatingInText = false;
            yield return new WaitForSeconds(secondsBetweenTextAnimations);
        } while (_animatingInText);

        conversationText.text = text;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // if we're currently animating text, space skips to completion
            if (_animatingInText)
            {
                _animatingInText = false;
            } else
            {
                if (focusedOnNPC) OnPlayerContinue?.Invoke(); // this was an NPC speech, call the player continue action
                else
                {
                    // if the chosen option has been set and we're not animating text, then the player must be pressing space to continue after picking their option
                    if (chosenOption != null)
                    {
                        OnPlayerChoseOption?.Invoke(chosenOption); // this was a player option choice, call the action and pass the chosen option
                    } 
                }

            }
        }
    }
}
