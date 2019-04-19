using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConversationNode
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;
    public GUIStyle currentStyle;

    public ConnectionPoint inPoint;
    public List<ConnectionPoint> outPoints = new List<ConnectionPoint>();

    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;

    public GUIStyle textStyle;

    public NPCSpeech npcSpeech;

    public Action<ConversationNode> OnRemoveNode;
    Action<ConnectionPoint> OnClickOutPoint;
    Action<ConnectionPoint> OnRemoveOutPoint;
    Action OnDirty;
    public bool isStarter;

    public ConversationNode(NPCSpeech npcSpeech, GUIStyle nodeStyle, GUIStyle selectedNodeStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, GUIStyle textStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<ConversationNode> OnRemoveNode, Action<ConnectionPoint> OnRemoveOutPoint, Action OnDirty)
    {
        rect = new Rect(npcSpeech.x, npcSpeech.y, 0, 0);
        currentStyle = defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedNodeStyle;
        this.inPointStyle = inPointStyle;
        this.outPointStyle = outPointStyle;
        this.textStyle = textStyle;
        this.npcSpeech = npcSpeech;

        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        //outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        this.OnRemoveNode = OnRemoveNode;
        this.OnClickOutPoint = OnClickOutPoint;
        this.OnRemoveOutPoint = OnRemoveOutPoint;
        this.OnDirty = OnDirty;

        if (npcSpeech.playerOptions != null)
        {
            foreach (PlayerSpeechOption option in npcSpeech.playerOptions)
            {
                AddOutPointForOption(option);
            }
        }

        CalculateSize();
    }

    public void CalculateSize()
    {
        rect.width = 200;
    }


    public void Drag(Vector2 delta, bool shouldDirty = true)
    {
        rect.position += delta;
        npcSpeech.x = (int)rect.position.x;
        npcSpeech.y = (int)rect.position.y;

        if (shouldDirty) OnDirty();
    }

    public void Draw()
    {
        float starterOffset = 0;
        if (isStarter) starterOffset = 30;

        // calculate height
        if (npcSpeech.playerOptions != null)
        {
            rect.height = 200 + starterOffset + (150 * (npcSpeech.playerOptions.Count));
        }
        else
        {
            rect.height = 200 + starterOffset;
        }

        List<PlayerSpeechOption> optionsToDelete = new List<PlayerSpeechOption>();

        inPoint.Draw(0);

       // outPoint.Draw();
        GUI.Box(rect, title, currentStyle);

        if (isStarter)
        {
            GUIStyle startsHere = new GUIStyle();
            startsHere.normal.textColor = Color.white;
            startsHere.fontStyle = FontStyle.Bold;
            startsHere.fontSize = 16;
            startsHere.alignment = TextAnchor.MiddleCenter;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 10, rect.width, 30f), new GUIContent("STARTS HERE"), startsHere);
        }

        EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + 10 + starterOffset, rect.width, 10f), new GUIContent("NPC says:"), textStyle);
        string newText = EditorGUI.TextArea(new Rect(rect.x + 10, rect.y + 30 + starterOffset, rect.width - 20, 100), npcSpeech.npcSays);
        if (newText != npcSpeech.npcSays)
        {
            npcSpeech.npcSays = newText;
            OnDirty();
        }

        EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + rect.height - 50, rect.width - 20, 2f), "", GUI.skin.horizontalSlider);

        float startingY = rect.y + 150 + starterOffset;


        if (npcSpeech.playerOptions != null)
        {
            int i = 0;

            foreach (PlayerSpeechOption option in npcSpeech.playerOptions)
            {
                float y = startingY + (i * 150);
                EditorGUI.LabelField(new Rect(rect.x + 10, y, rect.width, 20f), new GUIContent("Player says:"), textStyle);

                if (GUI.Button(new Rect(rect.x + rect.width - 30, y, 20, 20), new GUIContent("x")))
                {
                    // remove this option
                    optionsToDelete.Add(option);
                }
                
                // draw the related connectionpoint for this option
                foreach (ConnectionPoint connectionPoint in outPoints)
                {
                    if (connectionPoint.playerOption == option) connectionPoint.Draw(y);
                }

                y += 25;
                string newPlayerSays = EditorGUI.TextArea(new Rect(rect.x + 10, y, rect.width - 20, 100), option.playerSays);
                if (newPlayerSays != option.playerSays)
                {
                    option.playerSays = newPlayerSays;
                    OnDirty();
                }

                i++;
            }
        }

        if (GUI.Button(new Rect(rect.x + 10, rect.y + rect.height - 35, rect.width - 20, 25), new GUIContent("Add new player option"))) {
            AddPlayerOption();
            OnDirty();
        }

        foreach (PlayerSpeechOption optionToDelete in optionsToDelete) RemovePlayerOption(optionToDelete);
    }

    public void RemovePlayerOption(PlayerSpeechOption option)
    {
        ConnectionPoint outPointToRemove = null;

        foreach (ConnectionPoint outPoint in outPoints)
        {
            if (outPoint.playerOption == option) outPointToRemove = outPoint;
        }

        outPoints.Remove(outPointToRemove);

        // to ensure the connection gets removed too
        OnRemoveOutPoint(outPointToRemove);

        OnDirty();
        npcSpeech.RemovePlayerSpeechOption(option);

        GUI.changed = true;
    }

    public void AddPlayerOption()
    {
        PlayerSpeechOption newOption = npcSpeech.CreatePlayerSpeechOption();

        AddOutPointForOption(newOption);

        GUI.changed = true;
    }

    public void AddOutPointForOption(PlayerSpeechOption option)
    {
        ConnectionPoint outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        outPoint.playerOption = option;
        outPoints.Add(outPoint);
    }

    private void ShowContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        menu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        OnRemoveNode?.Invoke(this);
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {

            case EventType.KeyDown:
                if ((e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace) && isSelected) OnClickRemoveNode();
                break;

            case EventType.MouseDown:
                if (e.button == 0)
                {
                    // left mouse
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        currentStyle = selectedNodeStyle;
                    } else
                    {
                        isSelected = false;
                        currentStyle = defaultNodeStyle;
                    }

                    GUI.changed = true;

                }

                if (e.button == 1 && rect.Contains(e.mousePosition))
                {
                    ShowContextMenu();
                    e.Use();
                }

                break;
            case EventType.MouseUp:
                isDragged = false;
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use(); // prevent event from bubbling
                    return true;
                }
                break;

        }
                
        
        return false;
    }
}
