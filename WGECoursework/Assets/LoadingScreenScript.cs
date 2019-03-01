using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenScript : MonoBehaviour
{
    // Consumes events from the chunk loader to render a progress bar

    ChunkLoader chunkLoader;
    public Image progressBar;

    private void Awake()
    {
        chunkLoader = GameObject.Find("ChunkLoader").GetComponent<ChunkLoader>();
        chunkLoader.OnStartLoading += StartLoading;
        chunkLoader.OnLoadProgress += Progress;
        chunkLoader.OnFinishLoading += Finished;
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
