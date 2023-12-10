using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public partial class GraphDrawerWindow : EditorWindow
{
    private bool showControlPanel = true;

    public RenderGraphShower Graph
    {
        set
        {
            m_Graph = value;
            BuildGraph();
        }
    }

    private RenderGraphShower m_Graph;
    private List<Vector2> nodePos;
    private Vector2Int sizePerNode = new Vector2Int(208,100);
    private Vector2Int offsetBetweenNode = new Vector2Int(30, 30);
    private int curClickNode;

    public static GraphDrawerWindow Init(RenderGraphShower graph)
    {
        if (graph!=null)
        {
            GraphDrawerWindow w = CreateInstance<GraphDrawerWindow>();
            w.titleContent = new GUIContent("红点树");
            w.wantsMouseMove = true;
            w.panOffset = Vector2.zero;
            w.Graph = graph;
            w.BuildGraph();
            w.Show();

            return w;
        }
        return null;
    }

    public void BuildGraph()
    {
        if (m_Graph != null)
        {
            int rootIdx = m_Graph.rootNode;
            curClickNode = rootIdx;
            Span<bool> visFlag = stackalloc bool[m_Graph.nodes.Count];
            Span<int> depthFlag = stackalloc int[m_Graph.nodes.Count];
            Span<int> depthIdx = stackalloc int[m_Graph.nodes.Count];
            //每层深度上节点的个数
            Dictionary<int, int> depthTempIdx = new Dictionary<int, int>();
            int maxDepth = 0;
            for (int i = 0; i < m_Graph.nodes.Count; ++i) visFlag[i] = false;
            for (int i = 0; i < m_Graph.nodes.Count; ++i) depthFlag[i] = -1;
            for (int i = 0; i < m_Graph.nodes.Count; ++i) depthIdx[i] = -1;
            Queue<KeyValuePair<int,int>> que = new Queue<KeyValuePair<int, int>>();
            //下推过程
            if(rootIdx < m_Graph.nodesEdge.Count && m_Graph.nodesEdge[rootIdx] != null)
            {
                que.Enqueue(new KeyValuePair<int, int>(m_Graph.rootNode,0));
                depthFlag[m_Graph.rootNode] = 0;
                depthIdx[m_Graph.rootNode] = 0;
                depthTempIdx[0] = 0;
                while (que.Count > 0)
                {
                    KeyValuePair<int, int> idDepth = que.Peek();
                    que.Dequeue();
                    if (!visFlag[idDepth.Key])
                    {
                        visFlag[idDepth.Key] = true;
                        int nextDepth = idDepth.Value + 1;
                        List<int> pointList = m_Graph.nodesEdge[idDepth.Key];
                        if (pointList != null)
                        {
                            for (int i = 0; i < pointList.Count; ++i)
                            {
                                int toIdx = pointList[i];
                                depthFlag[toIdx] = nextDepth;
                                que.Enqueue(new KeyValuePair<int, int>(toIdx, nextDepth));
                                maxDepth = Math.Max(maxDepth, nextDepth);
                                if(depthIdx[toIdx] == -1)
                                {
                                    if (depthTempIdx.TryGetValue(nextDepth, out int allocIdx))
                                    {
                                        depthIdx[toIdx] = allocIdx + 1;
                                        depthTempIdx[nextDepth] = allocIdx + 1;
                                    }
                                    else
                                    {
                                        depthIdx[toIdx] = 0;
                                        depthTempIdx[nextDepth] = 0;
                                    }
                                }
                            }
                        }
                    }
                }

                nodePos = new List<Vector2>();
                //1. 计算每层有多少节点,分配x的下标,节点个数等于to的总数
                for (int i = 0; i < m_Graph.nodes.Count; ++i)
                {
                    Vector2 realPos = Vector2.zero;
                    
                    int depth = depthFlag[i];
                    int inDepthIdx = depthIdx[i];
                    int depthCountNum = depthTempIdx[depth];
                    float depthWidth = (depthCountNum + 1) * sizePerNode.x + depthCountNum * offsetBetweenNode.x;
                    float halfDepthWidth = depthWidth / 2;
                    //0.0是根节点坐标
                    //1. 计算Y轴坐标
                    realPos.y = depth * (offsetBetweenNode.y + sizePerNode.y);
                    //2. 计算X轴坐标,按照X=0对称
                    realPos.x = inDepthIdx * (offsetBetweenNode.x + sizePerNode.x) - halfDepthWidth;

                    nodePos.Add(realPos);
                }
            }
        }
    }

    protected virtual void OnGUI()
    {
        Control();

        for(int i = 0; i < m_Graph.nodes.Count; ++i)
        {
            List<int> edges = m_Graph.nodesEdge[i];
            if (edges != null)
            {
                for(int j = 0; j < edges.Count; ++j)
                {
                    int toIdx = edges[j];
                    if (m_Graph.nodes[toIdx].value > 0)
                    {
                        DrawConnection(nodePos[i], nodePos[toIdx], Color.green);
                    }
                    else
                    {
                        DrawConnection(nodePos[i], nodePos[toIdx], Color.white);
                    }
                }
            }
        }
        if (clickDirty)
        {
            for (int i = 0; i < m_Graph.nodes.Count; ++i)
            {
                if (CheckTouched(nodePos[i]))
                {
                    curClickNode = i;
                }
            }
            clickDirty = false;
        }

        for(int i = 0; i < m_Graph.nodes.Count; ++i)
        {
            DrawNode(nodePos[i], m_Graph.nodes[i],i);
        }

        DrawPanel();
    }

    public void DrawConnection(Vector2 fromPos,Vector2 toPos,Color color)
    {
        Vector2 fromLneOffset = new Vector2(104, 60);
        Vector2 toLineOffset = new Vector2(104, 10);
        fromPos = fromPos + panOffset + fromLneOffset;
        toPos = toPos + panOffset + toLineOffset;
        Handles.color = color;
        DrawAAPolyLineNonAlloc(5, fromPos, toPos);
    }

    public void DrawNode(Vector2 pos, RenderGraphNode node,int nodeId)
    {
        bool isSelected = node.value > 0;
        bool isClicked = nodeId == curClickNode;
        GUILayout.BeginArea(new Rect(pos + panOffset, new Vector2(208, 4000)));

        GUIStyle style = new GUIStyle(NodeEditorResources.styles.nodeBody);
        GUIStyle highlightStyle = new GUIStyle(NodeEditorResources.styles.nodeHighlight);
        highlightStyle.padding = style.padding;
        GUI.color = new Color32(90, 97, 105, 255);
        
        if(isSelected)
        {
            style.padding = new RectOffset();
            GUILayout.BeginVertical(style);
            GUI.color = Color.green;
            GUILayout.BeginVertical(new GUIStyle(highlightStyle));
        }
        else
        {
            if (isClicked)
            {
                style.padding = new RectOffset();
                GUILayout.BeginVertical(style);
                GUI.color = Color.white;
                GUILayout.BeginVertical(new GUIStyle(highlightStyle));
            }
            else
            {
                GUILayout.BeginVertical(style);
            }
        }

        GUI.color = Color.white;
        if (node.value == 1)
        {
            GUILayout.Label(node.name, NodeEditorResources.styles.nodeActiveHeader, GUILayout.Height(30));
        }
        else
        {
            GUILayout.Label(node.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        GUILayout.Label("红点的值: " + node.value);

        if (isSelected) GUILayout.EndVertical();
        else if (isClicked) GUILayout.EndVertical();
        GUILayout.EndVertical();
       
        GUILayout.EndArea();
    }

    public void DrawPanel()
    {
        if (showControlPanel)
        {
            if (GUILayout.Button("隐藏", GUILayout.Height(28), GUILayout.Width(58)))
            {
                showControlPanel = false;
                Repaint();
            }
            GUIStyle style = new GUIStyle(NodeEditorResources.styles.sidePanel);
            GUI.color = new Color32(90, 97, 105, 125);
            GUILayout.BeginArea(new Rect(new Vector2(0, 30), new Vector2(208, 240)), style);
            GUI.color = Color.white;
            GUILayout.Label("显示面板", NodeEditorResources.styles.panelHeader, GUILayout.Height(30));
            GUI.color = new Color32(150, 150, 150, 255);
            if (GUILayout.Button("恢复视口"))
            {
                panOffset = Vector2.zero;
            }
            if (GUILayout.Button("刷新红点信息"))
            {
                Repaint();
            }
            GUI.color = Color.white;
            GUILayout.Label("鼠标点击位置: \n" + lastMousePos.ToString());
            GUILayout.Label("Offset: \n" + panOffset.ToString());
            GUILayout.Label("红点Key: \n");
            GUILayout.TextField(m_Graph.nodes[curClickNode].name);
            GUILayout.EndArea();
        }
        else
        {
            if (GUILayout.Button("显示控制面板", GUILayout.Height(28), GUILayout.Width(100)))
            {
                showControlPanel = true;
                Repaint();
            }
        }
    }

    private static readonly Vector3[] polyLineTempArray = new Vector3[2];
    /// <summary> Draws a line segment without allocating temporary arrays </summary>
    static void DrawAAPolyLineNonAlloc(float thickness, Vector2 p0, Vector2 p1)
    {
        polyLineTempArray[0].x = p0.x;
        polyLineTempArray[0].y = p0.y;
        polyLineTempArray[1].x = p1.x;
        polyLineTempArray[1].y = p1.y;
        Handles.DrawAAPolyLine(thickness, polyLineTempArray);
    }
}
