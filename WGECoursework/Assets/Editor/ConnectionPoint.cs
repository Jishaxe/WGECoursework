using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionPointType { In, Out }

// represents the connection point button
public class ConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    public ConversationNode node;
    public GUIStyle style;

    // callback to call when connectionpoint is clicked
    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(ConversationNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 10f, 20f);
    }

    public void Draw()
    {
        // center of the height of the node
        rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type)
        {
            // draw on left side
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;
            
            // draw on right side
            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
        }

        // call the callback if the button is clicked
        if (GUI.Button(rect, "", style)) OnClickConnectionPoint?.Invoke(this);
    }
}
