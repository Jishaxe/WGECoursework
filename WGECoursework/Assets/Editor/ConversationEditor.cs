using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ConversationEditor : EditorWindow
{
    [MenuItem("Window/Conversation Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ConversationEditor));
    }

    void OnGUI()
    {

    }
}
