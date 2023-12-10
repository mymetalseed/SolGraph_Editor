using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeEditorResources
{
    public static Texture2D nodeBody { get { return _nodeBody != null ? _nodeBody : _nodeBody = Resources.Load<Texture2D>("xnode_node"); } }
    private static Texture2D _nodeBody;
    public static Texture2D nodeHighlight { get { return _nodeHighlight != null ? _nodeHighlight : _nodeHighlight = Resources.Load<Texture2D>("xnode_node_highlight"); } }
    private static Texture2D _nodeHighlight;
    public static GUIStyleTemplate _styles = null;
    public static GUIStyleTemplate styles { get { return _styles != null ? _styles : _styles = new GUIStyleTemplate(); } }
}

public class GUIStyleTemplate
{
    public GUIStyle nodeBody, nodeHighlight,nodeHeader,nodeActiveHeader,sidePanel,panelHeader;
    public GUIStyleTemplate()
    {
        nodeBody = new GUIStyle();
        nodeBody.normal.background = NodeEditorResources.nodeBody;
        nodeBody.border = new RectOffset(32, 32, 32, 32);
        nodeBody.padding = new RectOffset(16, 16, 4, 16);

        nodeHighlight = new GUIStyle();
        nodeHighlight.normal.background = NodeEditorResources.nodeHighlight;
        nodeHighlight.border = new RectOffset(32, 32, 32, 32);
        nodeHighlight.padding = new RectOffset(16, 16, 4, 16);

        nodeHeader = new GUIStyle();
        nodeHeader.alignment = TextAnchor.MiddleCenter;
        nodeHeader.fontStyle = FontStyle.Bold;
        nodeHeader.normal.textColor = Color.black;

        nodeActiveHeader = new GUIStyle();
        nodeActiveHeader.alignment = TextAnchor.MiddleCenter;
        nodeActiveHeader.fontStyle = FontStyle.BoldAndItalic;
        nodeActiveHeader.normal.textColor = Color.red;

        sidePanel = new GUIStyle();
        sidePanel.normal.background = NodeEditorResources.nodeBody;
        sidePanel.border = new RectOffset(32, 32, 32, 32);
        sidePanel.padding = new RectOffset(16, 16, 4, 16);

        panelHeader = new GUIStyle();
        panelHeader.alignment = TextAnchor.MiddleCenter;
        panelHeader.fontStyle = FontStyle.Bold;
        panelHeader.normal.textColor = Color.white;
    }
}
