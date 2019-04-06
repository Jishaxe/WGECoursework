using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// A lot of the work done here is from the tutorial http://gram.gs/gramlog/creating-node-based-editor-unity/ for the node-based stuff
public class ConversationEditor : EditorWindow
{
    ConnectionPoint selectedInPoint;
    ConnectionPoint selectedOutPoint;

    // drag of the entire canvas
    private Vector2 offset;
    private Vector2 drag;

    public Conversation conversation;
    string conversationFilename;

    public static void ShowWindow(Conversation conversation)
    {
        //Show existing window instance. If one doesn't exist, make one.
        ConversationEditor window = (ConversationEditor)GetWindow(typeof(ConversationEditor));
        window.titleContent = new GUIContent("Conversation Editor");
        window.conversation = conversation;
        window.Load();
        window.Show();
    }

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject as Conversation != null)
        {
            ShowWindow((Conversation)Selection.activeObject);
            return true; //catch open file
        }

        return false; // let unity open the file
    }


    private void OnEnable()
    {

    }

    public void DrawNodes()
    {
        foreach (ConversationNode node in conversation.nodes) node.Draw();
    }

    public void DrawConnections()
    {
        foreach (Connection connection in conversation.connections) connection.Draw();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }


    void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawToolStrip();
        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        if (GUI.changed) Repaint();
    }
    
    private void DrawToolStrip()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        if (GUILayout.Button("Add speech", EditorStyles.toolbarButton))
        {
            OnAddNode(new Vector2(100, 100));
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label(conversationFilename);

        if (GUILayout.Button("Save", EditorStyles.toolbarButton)) ForceSave();

        GUILayout.EndHorizontal();
    }

    void ForceSave()
    {
        EditorUtility.SetDirty(conversation); // user has to save project as well
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawConnectionLine(Event e)
    {
        // draw a line from the in point to the mouse
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        // draw a line from the out point to the mouse
        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    void ProcessNodeEvents(Event e)
    {
        // process node events backwards because the last node is the topmost node and should get events first
        for (int i = conversation.nodes.Count - 1; i >= 0; i--)
        {
            bool guiChanged = conversation.nodes[i].ProcessEvents(e);
            if (guiChanged) GUI.changed = true;
        }
    }

    void CreateContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add speech"), false, () => OnAddNode(mousePosition));
        menu.ShowAsContext();
    }

    void OnAddNode(Vector2 mousePosition)
    {
        ConversationNode node = new ConversationNode(mousePosition, 200, 50, conversation.nodeStyle, conversation.selectedNodeStyle, conversation.inPointStyle, conversation.outPointStyle);

        node.Initialize(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        conversation.nodes.Add(node);
        Save();
    }

    void Save()
    {
        EditorUtility.SetDirty(conversation); // user has to save project as well
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public void Load()
    {
        conversation.nodeStyle = new GUIStyle();
        // use animator node 
        conversation.nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        conversation.nodeStyle.border = new RectOffset(12, 12, 12, 12);

        // two distinct styles for each connection type
        conversation.inPointStyle = new GUIStyle();
        conversation.inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        conversation.inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        conversation.inPointStyle.border = new RectOffset(4, 4, 12, 12);

        conversation.selectedNodeStyle = new GUIStyle();
        conversation.selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        conversation.selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);


        conversation.outPointStyle = new GUIStyle();
        conversation.outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        conversation.outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        conversation.outPointStyle.border = new RectOffset(4, 4, 12, 12);

        this.conversationFilename = AssetDatabase.GetAssetPath(conversation.GetInstanceID());
        Debug.Log("Loaded conversation with " + conversation.nodes.Count + " nodes and " + conversation.connections.Count + " connections");
        // reassign events
        foreach (ConversationNode node in conversation.nodes) node.Initialize(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);

        // Find each connection nodes and reconnect
        foreach (Connection connection in conversation.connections)
        {
            connection.OnClickRemoveConnection = OnClickRemoveConnection;
            connection.inPoint = GetNodeByID(connection.connectedIn).inPoint;
            connection.outPoint = GetNodeByID(connection.connectedOut).outPoint;
        }
    }

    public ConversationNode GetNodeByID(string ID)
    {
        foreach (ConversationNode node in conversation.nodes)
        {
            if (node.ID == ID) return node;
        }

        return null;
    }

    void OnClickRemoveNode(ConversationNode node)
    {
        // remove all conversation.connections to this node first
        List<Connection> connectionsToRemove = new List<Connection>();

        foreach (Connection connection in conversation.connections)
        {
            if (connection.inPoint == node.inPoint || connection.outPoint == node.outPoint) connectionsToRemove.Add(connection);
        }

        foreach (Connection connection in connectionsToRemove)
        {
            conversation.connections.Remove(connection);
        }

        conversation.nodes.Remove(node);

        GUI.changed = true;
        Save();
    }

    void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        // if we already have an out point selected, create them
        if (selectedOutPoint != null)
        {
            // if we're not trying to connect a node to itself, connect them together
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            } else
            {
                ClearConnectionSelection();
            }
        };
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
        conversation.connections.Remove(connection);
    }

    // connect the two selected points
    void CreateConnection()
    {
        conversation.connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        Save();
    }

    void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0) ClearConnectionSelection();
                if (e.button == 1) CreateContextMenu(e.mousePosition);
                break;
            case EventType.MouseDrag:
                if (e.button == 0) OnDrag(e.delta);
                break;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;
        foreach (ConversationNode node in conversation.nodes) node.Drag(delta);

        Save();
        GUI.changed = true;
    }
}
    