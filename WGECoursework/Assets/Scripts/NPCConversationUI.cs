using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConversationUI : MonoBehaviour
{
    public Animator blackBars;

    public void Activate()
    {
        blackBars.Play("in");
    }

    public void Deactivate()
    {
        blackBars.Play("out");
    }
}
