using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// A lot of the work done here is from the tutorial http://gram.gs/gramlog/creating-node-based-editor-unity/ for the node-based stuff
public class ConversationEditor : EditorWindow
{
    public GUIStyle nodeStyle;
    public GUIStyle selectedNodeStyle;
    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;
    public GUIStyle textStyle;

    public List<ConversationNode> nodes = new List<ConversationNode>();
    public List<Connection> connections = new List<Connection>();

    ConnectionPoint selectedInPoint;
    ConnectionPoint selectedOutPoint;

    // drag of the entire canvas
    private Vector2 offset;
    private Vector2 drag;

    // loaded conversation
    public Conversation conversation;

    string conversationFilename;

    bool dirty = false;

    List<Connection> connectionsToRemove = new List<Connection>();

    [MenuItem("Conversation Editor/New conversation")]
    public static void OpenNewConversation()
    {
        Conversation cons = new Conversation();
        cons.fileName = "Untitled";
        ShowWindow(cons);
    }

    public static void ShowWindow(Conversation conversation)
    {
        //Show existing window instance. If one doesn't exist, make one.
        ConversationEditor window = (ConversationEditor)GetWindow(typeof(ConversationEditor));
        window.titleContent = new GUIContent("Conversation Editor");
        window.conversation = conversation;
        window.LoadNodesFromConversation();
        window.Show();
    }

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject as TextAsset != null)
        {
            ShowWindow(Conversation.LoadFromXML((TextAsset)Selection.activeObject));
            return true; //catch open file
        }

        return false; // let unity open the file
    }

    public void RemovePendingConnections()
    {
        foreach (Connection connection in connectionsToRemove)
        {
            connection.outPoint.playerOption.result = null;
            connection.outPoint.playerOption.resultID = null;
            connections.Remove(connection);
        }

        if (connectionsToRemove.Count > 0)
        {
            GUI.changed = true;
            dirty = true;
            connectionsToRemove.Clear();
        }
    }

    private void OnEnable()
    {

    }

    public void DrawNodes()
    {
        foreach (ConversationNode node in nodes) node.Draw();
    }

    public void DrawConnections()
    {
        foreach (Connection connection in connections) connection.Draw();
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

    void OnDirty()
    {
        dirty = true;
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

        RemovePendingConnections();
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

        // draw the filename and a star if it's dirty
        GUILayout.Label(conversation.fileName + ((dirty) ? "*":""));

        if (GUILayout.Button("Save", EditorStyles.toolbarButton)) Save();

        GUILayout.EndHorizontal();
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
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            bool guiChanged = nodes[i].ProcessEvents(e);
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
        NPCSpeech npcSpeech = conversation.CreateNPCSpeech(mousePosition);

        ConversationNode node = new ConversationNode(npcSpeech, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, textStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnRemoveConnectionPoint, OnDirty);
        nodes.Add(node);

        UpdateStartingSpeechNode();

        dirty = true;
    }

    void Save()
    {
        // pick a filename
        if (conversation.fileName == "Untitled")
        {
            string path = EditorUtility.SaveFilePanel("Save Conversation as XML", "", "New conversation", ".xml");
            if (path.Length == 0) return;

            conversation.fileName = path;
        }

        conversation.SaveToXML(conversation.fileName);
        dirty = false;
    }

    // disconnect any connections related to this outpoint
    public void OnRemoveConnectionPoint(ConnectionPoint outPoint)
    {
        foreach (Connection connection in connections)
        {
            if (connection.outPoint == outPoint) connectionsToRemove.Add(connection);
        }

        dirty = true;
    }

    // load nodes from the Conversation object
    public void LoadNodesFromConversation()
    {
        if (conversation == null)
        {
            conversation = new Conversation();
            conversation.fileName = "Untitled";
        }

        nodeStyle = new GUIStyle();
        // use animator node 
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        // two distinct styles for each connection type
        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);


        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;

        // Create nodes from NPCSpeeches
        foreach (NPCSpeech speech in conversation.npcSpeeches)
        {
            ConversationNode node = new ConversationNode(speech, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, textStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnRemoveConnectionPoint, OnDirty);
            nodes.Add(node);
        }


        // connect option results to nodes
        foreach (ConversationNode node in nodes)
        {
            if (node.outPoints != null)
            {
                foreach (ConnectionPoint outPoint in node.outPoints)
                {
                    if (outPoint.playerOption.result != null)
                    {
                        connections.Add(new Connection(GetNodeFromNPCSpeech(outPoint.playerOption.result).inPoint, outPoint, OnClickRemoveConnection));
                    }
                }
            }
        }

        UpdateStartingSpeechNode();
    }

    public ConversationNode GetNodeFromNPCSpeech(NPCSpeech speech)
    {
        foreach (ConversationNode node in nodes) if (node.npcSpeech == speech) return node;

        return null;
    }

    void OnClickRemoveNode(ConversationNode node)
    {
        foreach (Connection connection in connections)
        {
            if (connection.inPoint == node.inPoint) connectionsToRemove.Add(connection);
            else
            {
                foreach (ConnectionPoint outPoint in node.outPoints)
                {
                    if (connection.outPoint == outPoint) connectionsToRemove.Add(connection);
                }
            }
        }

        conversation.RemoveNPCSpeech(node.npcSpeech);
        UpdateStartingSpeechNode();
        nodes.Remove(node);

        dirty = true;
        GUI.changed = true;
    }

    // called when Set as conversation starter on context menu is clicked
    public void OnChangeStartingSpeech(ConversationNode newStarter)
    {
        conversation.startingID = newStarter.npcSpeech.ID;
        UpdateStartingSpeechNode();
    }

    // goes through all the nodes and sets node.isStarter according to whether it matches conversation.startingID
    public void UpdateStartingSpeechNode()
    {
        foreach (ConversationNode node in nodes)
        {
            if (node.npcSpeech.ID == conversation.startingID) node.isStarter = true;
            else node.isStarter = false;
        }
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
        connectionsToRemove.Add(connection);
    }

    // connect the two selected points
    void CreateConnection()
    {
        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        selectedOutPoint.playerOption.result = selectedInPoint.node.npcSpeech;
        selectedOutPoint.playerOption.resultID = selectedInPoint.node.npcSpeech.ID;
        dirty = true;
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
        foreach (ConversationNode node in nodes) node.Drag(delta, false);

        GUI.changed = true;
    }
}
    