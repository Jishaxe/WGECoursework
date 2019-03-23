using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenScript : MonoBehaviour
{
    // Consumes events from the chunk loader to render a progress bar
    public Image progressBar;

    private void Awake()
    {
        ChunkLoader.OnStartLoading += StartLoading;
        ChunkLoader.OnLoadProgress += Progress;
        ChunkLoader.OnFinishLoading += Finished;
    }

    void StartLoading(float progress)
    {
        gameObject.SetActive(true);
    }

    void Progress(float progress)
    {
        progressBar.fillAmount = progress;
    }

    void Finished(float progress)
    {
        gameObject.SetActive(false);
    }
}
