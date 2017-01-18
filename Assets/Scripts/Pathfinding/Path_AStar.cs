using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for calculating a path, given a start and end point
/// </summary>
/// <typeparam name="T">The Graph Type </typeparam>
public class Path_AStar<TGraph> where TGraph : Path_Graph
{
    public TGraph Graph { get; protected set; }
    public IPath_Node startNode { get; protected set; }
    public IPath_Node endNode { get; protected set; }

    public Path_AStar(TGraph graph, IPath_Node startNode, IPath_Node endNode)
    {
        Graph = graph;
        this.startNode = startNode;
        this.endNode = endNode;
    }
}
