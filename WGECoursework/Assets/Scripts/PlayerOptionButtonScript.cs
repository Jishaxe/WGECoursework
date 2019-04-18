using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptionButtonScript : MonoBehaviour
{
    PlayerSpeechOption option;
    Action<PlayerSpeechOption> OnPlayerChoseOption;


    // store the PlayerSpeechOption for this button and the action to call when clicked
    public void RegisterOption(PlayerSpeechOption option, Action<PlayerSpeechOption> OnPlayerChoseOption)
    {
        this.option = option;
        this.OnPlayerChoseOption = OnPlayerChoseOption;
    }

    public void OnClick()
    {
        OnPlayerChoseOption(this.option);
    }
}
