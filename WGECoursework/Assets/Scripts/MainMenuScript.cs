using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void YeetCraftHovered()
    {
        animator.Play("yeetcraft");
    }

    public void DialogHovered()
    {
        animator.Play("dialog");
    }

    public void YeetCraftClicked()
    {
        SceneManager.LoadScene("Voxel");
    }

    public void DialogClicked()
    {
        SceneManager.LoadScene("2D");
    }
}
