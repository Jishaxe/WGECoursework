using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlayerSpeechOption
{
    [XmlAttribute("ID")]
    public string ID;

    [XmlIgnore]
    public NPCSpeech result;

    [XmlElement("Result")]
    public string resultID;

    [XmlElement("PlayerSays")]
    public string playerSays;
}

[Serializable]
public class NPCSpeech
{
    [XmlIgnore]
    public Conversation converation;

    [XmlAttribute("ID")]
    public string ID;

    [XmlAttribute("x")]
    public int x;

    [XmlAttribute("y")]
    public int y;

    [XmlElement("NPCSays")]
    public string npcSays;

    [XmlArray("Options")]
    [XmlArrayItem("OptionID")]
    public List<string> playerOptionIDs;

    [XmlIgnore]
    public List<PlayerSpeechOption> playerOptions;

    public override string ToString()
    {
        if (playerOptions != null) return "NPCSpeech (" + x + "," + y + ") says: " + npcSays + " with " + playerOptions.Count + " options";
        else return "NPCSpeech (" + x + "," + y + ") says: " + npcSays + " with no options";
    }

    public PlayerSpeechOption CreatePlayerSpeechOption()
    {
        return converation.CreatePlayerSpeechOption(this);
    }

    public void RemovePlayerSpeechOption(PlayerSpeechOption option)
    {
        converation.RemovePlayerSpeechOption(option);
    }
}

[Serializable]
public class Conversation: ScriptableObject
{
    [XmlIgnore]
    public string fileName;

    [XmlIgnore]
    TextAsset textAsset;

    // the ID for the speech that starts
    // this does mean that conversations can only be started by the NPC
    [XmlElement("StartingID")]
    public string startingID;

    [XmlArray("NPCSpeeches")]
    public List<NPCSpeech> npcSpeeches;

    [XmlArray("PlayerOptions")]
    public List<PlayerSpeechOption> playerSpeechOptions;


    public Conversation()
    {
        npcSpeeches = new List<NPCSpeech>();
        playerSpeechOptions = new List<PlayerSpeechOption>();
    }

    public NPCSpeech CreateNPCSpeech(Vector2 position)
    {
        NPCSpeech speech = new NPCSpeech
        {
            playerOptions = new List<PlayerSpeechOption>(),
            playerOptionIDs = new List<string>(),
            x = (int)position.x,
            y = (int)position.y,
            ID = GUID.Generate().ToString(),
            converation = this
        };
        npcSpeeches.Add(speech);

        Debug.Log("Creating a new NPC speech, there are currently: " + npcSpeeches.Count);

        // if this is the first speech, assign it as the first one
        if (npcSpeeches.Count == 1)
        {
            startingID = speech.ID;
            Debug.Log("Setting as starter as this is the first speech");
        }

        return speech;
    }

    public void RemoveNPCSpeech(NPCSpeech speech)
    {
        foreach (PlayerSpeechOption option in speech.playerOptions) playerSpeechOptions.Remove(option);
        npcSpeeches.Remove(speech);

        // if this speech was the starting speech, reassign it to the first speech
        if (startingID == speech.ID)
        {
            if (npcSpeeches.Count > 0) startingID = npcSpeeches[0].ID;
            else startingID = null;
        }
    }

    public void RemovePlayerSpeechOption(PlayerSpeechOption option)
    {
        foreach (NPCSpeech speech in npcSpeeches)
        {
            speech.playerOptions.Remove(option);
            speech.playerOptionIDs.Remove(option.ID);
        }

        playerSpeechOptions.Remove(option);
    }

    public PlayerSpeechOption CreatePlayerSpeechOption(NPCSpeech speech)
    {
        Debug.Log("making new speech option");
        PlayerSpeechOption option = new PlayerSpeechOption();
        option.ID = GUID.Generate().ToString();
        speech.playerOptions.Add(option);
        speech.playerOptionIDs.Add(option.ID);

        playerSpeechOptions.Add(option);
        return option;
    }

    public NPCSpeech GetNPCSpeechByID(string id)
    {
        foreach (NPCSpeech npcSpeech in npcSpeeches)
        {
            if (npcSpeech.ID == id) return npcSpeech;
        }

        Debug.Log("Couldn't find NPCSpeech by ID: " + id);
        return null;
    }

    public PlayerSpeechOption GetPlayerSpeechOptionByID(string id)
    {
        foreach (PlayerSpeechOption option in playerSpeechOptions)
        {
            if (option.ID == id) return option;
        }

        Debug.Log("Couldn't find player speech option by ID: " + id);
        return null;
    }

    public static Conversation LoadFromXML(TextAsset textAsset)
    {
        string fileName = AssetDatabase.GetAssetPath(textAsset.GetInstanceID());

        XmlSerializer xml = new XmlSerializer(typeof(Conversation));
        StringReader reader = new StringReader(textAsset.text);

        Conversation conversation = (Conversation)xml.Deserialize(reader);
        reader.Close();

        Debug.Log("Loaded conversation from XML with " + conversation.npcSpeeches.Count + " NPC speeches and " + conversation.playerSpeechOptions.Count + " player speech options");

        // make the connections by ID between NPCSpeeches and options
        foreach (NPCSpeech npcSpeech in conversation.npcSpeeches)
        {
            npcSpeech.converation = conversation;
            npcSpeech.playerOptions = new List<PlayerSpeechOption>();
            
            foreach (string optionID in npcSpeech.playerOptionIDs)
            {
                npcSpeech.playerOptions.Add(conversation.GetPlayerSpeechOptionByID(optionID));
            }
        }

        foreach (PlayerSpeechOption option in conversation.playerSpeechOptions)
        {
            if (option.resultID != null) option.result = conversation.GetNPCSpeechByID(option.resultID);
        }

        Debug.Log("Starting ID: " + conversation.startingID);
        if (conversation.startingID == null)
        {
            Debug.Log("Empty starting ID, assigning to first NPCSpeech");
            conversation.startingID = conversation.npcSpeeches[0].ID;
        }

        conversation.fileName = fileName;
        return conversation;
    }

    public void SaveToXML(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Conversation));
        StreamWriter writer = new StreamWriter(fileName, false);
        serializer.Serialize(writer.BaseStream, this);
        writer.Close();

        AssetDatabase.ImportAsset(fileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
