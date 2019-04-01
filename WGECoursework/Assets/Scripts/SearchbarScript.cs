using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchbarScript : MonoBehaviour
{
    public delegate void SearchBarEvent(string text);
    public SearchBarEvent OnSearch;
    public bool showing = false;
    public Animator animator;
    public InputField input;

    public void Toggle()
    {
        if (!showing) Show();
        else Hide();
    }

    public void Show()
    {
        showing = true;
        animator.Play("show");
        input.gameObject.SetActive(true);
    }

    public void Hide()
    {
        showing = false;
        animator.Play("hide");
        input.gameObject.SetActive(false);
    }

    public void OnTextChanged()
    {
        OnSearch(input.text);
    }
}
