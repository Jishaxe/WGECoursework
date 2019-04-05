using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    // callback when the connection itself is clicked
    public Action<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        // draw a bezier line
        Handles.DrawBezier(
          inPoint.rect.center,
          outPoint.rect.center,
          inPoint.rect.center + Vector2.left * 50f,
          outPoint.rect.center - Vector2.left * 50f,
          Color.white,
          null,
          2f
        );
    
        // draw a button in the middle of the line to delete the connection
        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            OnClickRemoveConnection?.Invoke(this);
        }
    }
}
