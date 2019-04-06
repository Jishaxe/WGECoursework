using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation", order = 1)]
public class Conversation : ScriptableObject
{
    public GUIStyle nodeStyle;
    public GUIStyle selectedNodeStyle;
    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;


    public List<ConversationNode> nodes = new List<ConversationNode>();
    public List<Connection> connections = new List<Connection>();
}
