using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderGraphNode
{
    public string name;
    public int value;
}

public class RenderGraphShower
{
    public List<RenderGraphNode> nodes;
    public List<List<int>> nodesEdge;
    public int rootNode;
    public int selectNode;
} 