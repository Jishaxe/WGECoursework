using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenScript : MonoBehaviour
{
    public delegate void StartLoadEvent(string filename);
    public static event StartLoadEvent OnStartLoad;

    public GameObject errorText;
    public InputField fileText;
    public GameObject fileInputScreen;
    public GameObject loadinBar;

    public void OnLoadButtonPressed ()
    {
        string filename = fileText.text;
        if (filename == "") filename = "AssessmentChunk2.xml";

        // check if the file exists and exit if it doesn't
        if (!File.Exists(filename))
        {
            errorText.SetActive(true);
            return;
        }

        // show the loading bar, hide the input screen and pass the event on to chunkloader
        fileInputScreen.SetActive(false);
        loadinBar.SetActive(true);

        OnStartLoad(filename);

        // and that's my job done
    }

    private void Update()
    {
        if (Input.GetKeyDown("enter")) OnLoadButtonPressed();
    }
}
