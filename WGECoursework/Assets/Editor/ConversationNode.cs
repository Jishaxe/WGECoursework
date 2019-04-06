using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ConversationNode
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;
    public GUIStyle currentStyle;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<ConversationNode> OnRemoveNode;

    public ConversationNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedNodeStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<ConversationNode> OnRemoveNode)
    {
        rect = new Rect(position.x, position.y, width, height);
        currentStyle = defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedNodeStyle;

        // make the left in point and right out point with the given styles and callbacks
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        this.OnRemoveNode = OnRemoveNode;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, currentStyle);
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
