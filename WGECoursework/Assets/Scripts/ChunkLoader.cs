using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    // 
    public void LoadChunk(string fileName)
    {
        XmlReader reader = XmlReader.Create(fileName);

    }

    private void Start()
    {
        LoadChunk("AssessmentChunk1.xml");
    }
}
