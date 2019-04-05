using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// A lot of the work done here is from the tutorial http://gram.gs/gramlog/creating-node-based-editor-unity/ for the node-based stuff
public class ConversationEditor : EditorWindow
{
    private List<ConversationNode> nodes = new List<ConversationNode>();
    private List<Connection> connections = new List<Connection>();

    GUIStyle nodeStyle;
    GUIStyle inPointStyle;
    GUIStyle outPointStyle;

    ConnectionPoint selectedInPoint;
    ConnectionPoint selectedOutPoint;

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
        nodeStyle = new GUIStyle();
        // use animator node 
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        // two distinct styles for each connection type
        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

    }

    public void DrawNodes()
    {
        foreach (ConversationNode node in nodes) node.Draw();
    }

    public void DrawConnections()
    {
        foreach (Connection connection in connections) connection.Draw();
    }

    void OnGUI()
    {
        DrawNodes();
        DrawConnections();
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        if (GUI.changed) Repaint();
    }

    void ProcessNodeEvents(Event e)
    {
        // process node events backwards because the last node is the topmost node and should get events first
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            bool guiChanged = nodes[i].ProcessEvents(e);
            if (guiChanged) GUI.changed = true;
        }
    }

    void CreateContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Player talks"), false, () => OnAddNode(mousePosition));
        menu.AddItem(new GUIContent("NPC talks"), false, () => OnAddNode(mousePosition));
        menu.ShowAsContext();
    }

    void OnAddNode(Vector2 mousePosition)
    {
        nodes.Add(new ConversationNode(mousePosition, 200, 50, nodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint));
    }

    void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        // if we already have an out point selected, create them
        if (selectedOutPoint != null)
        {
            // if we're not trying to connect a node to itself, connect them togehter
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            } else
            {
                ClearConnectionSelection();
            }
        }
    }

    void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        // if we already have an out point selected, create them
        if (selectedInPoint != null)
        {
            // if we're not trying to connect a node to itself, connect them togehter
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    // connect the two selected points
    void CreateConnection()
    {
        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1) CreateContextMenu(e.mousePosition);
                break;
        }
    }
}
    