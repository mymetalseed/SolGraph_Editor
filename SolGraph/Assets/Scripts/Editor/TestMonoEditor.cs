using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestMono))]
public class TestMonoEditor : Editor
{
    private TestMono m_Target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button(new GUIContent("打开红点数树")))
        {
            RenderGraphShower graph = new RenderGraphShower();
            graph.nodes = new List<RenderGraphNode>();
            graph.nodesEdge = new List<List<int>>();
            graph.rootNode = 0;
     
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_ROOT",value=1});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_1",value=1});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_2",value=0});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_1_1",value=0});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_1_2",value=1});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_2_1",value=0});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_2_1_1",value=0});
            graph.nodes.Add(new RenderGraphNode() { name = "REDDOT_SUB_ROOT_1_1_1",value=1});

            for (int i = 0; i < graph.nodes.Count; ++i)
            {
                graph.nodesEdge.Add(new List<int>());
            }
            graph.nodesEdge[0].Add(1);
            graph.nodesEdge[0].Add(2);
            graph.nodesEdge[1].Add(3);
            graph.nodesEdge[1].Add(4);
            graph.nodesEdge[1].Add(5);
            graph.nodesEdge[2].Add(5);
            graph.nodesEdge[2].Add(6);
            graph.nodesEdge[4].Add(7);

            graph.selectNode = 4;

            GraphDrawerWindow.Init(graph);
        }
    }
}
