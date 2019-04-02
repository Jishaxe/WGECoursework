using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


public class ConversationEditor : EditorWindow
{

    [MenuItem("Window/Conversation Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow window = GetWindow(typeof(ConversationEditor));
        window.titleContent = new GUIContent("Conversation Editor");
        window.Show();
    }

    private void OnEnable()
    {
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("NPC says:");
        EditorGUILayout.TextField("");
        GUILayout.Button("x");
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUI.indentLevel * 10);
        GUILayout.Button("Add answer");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Answer:");
        EditorGUILayout.IntField(0);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        GUILayout.Button("Add NPC msg");

    }
}
    