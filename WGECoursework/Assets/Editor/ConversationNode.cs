using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationNode
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public GUIStyle style;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public ConversationNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;

        // make the left in point and right out point with the given styles and callbacks
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    // left mouse
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                    }

                    GUI.changed = true;

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
