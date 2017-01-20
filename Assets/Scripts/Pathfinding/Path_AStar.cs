using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;
using System.Linq;

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



    public Queue<IPath_Node> path = new Queue<IPath_Node>();

    public void CalculatePath()
    {
        // The set of nodes already evaluated
        HashSet<IPath_Node> closedSet = new HashSet<IPath_Node>();
        // The set of currenty discovered nodes that have not been evaluated
        SimplePriorityQueue<IPath_Node> openSet = new SimplePriorityQueue<IPath_Node>();
        // Initially, only the start node is known.
        openSet.Enqueue(startNode, 0);

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        Dictionary<IPath_Node, IPath_Node> cameFrom = new Dictionary<IPath_Node, IPath_Node>();

        // For each node, the cost of getting from the start node to that node.
        Dictionary<IPath_Node, int> gScore = new Dictionary<IPath_Node, int>();
        for (int i = 0; i < Graph.nodes.Count; i++)
        {
            gScore.Add(Graph.nodes[i], int.MaxValue); // QUESTION: Consider using floats
        }
        // The cost of going from start to start is 0
        if (gScore.ContainsKey(startNode))
            gScore[startNode] = 0;
        else
        {
            Debug.LogError("Start node is not in Graph.nodes!");
            return;
        }

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        Dictionary<IPath_Node, int> fScore = new Dictionary<IPath_Node, int>();
        for (int i = 0; i < Graph.nodes.Count; i++)
        {
            fScore.Add(Graph.nodes[i], int.MaxValue); // QUESTION: Consider using floats
        }
        // For the first node, that value is completely heuristic
        if (fScore.ContainsKey(startNode))
            fScore[startNode] = ManhattanDist(startNode, endNode);
        else
        {
            Debug.LogError("Start node is not in Graph.nodes!");
            return;
        }

        while (openSet.Count > 0)
        {
            IPath_Node current = openSet.Dequeue();
            if(current == endNode)
            {
                // WE DID IT!
                ReconstructPath(cameFrom, current);
                return;
            }
            closedSet.Add(current);
            foreach(IPath_Node n in current.GetNeighbors())
            {
                if (closedSet.Contains(n))
                    continue;
                int gScoreTemp = gScore[current] + n.MovementCost; // TODO: Diagnol neighbors will be 14, and regular neighbors will be 10
                int fScoreTemp = gScoreTemp + ManhattanDist(n, endNode);
                if (openSet.Contains(n) == false)
                {
                    openSet.Enqueue(n, fScoreTemp);
                }
                else if (gScoreTemp >= gScore[n])
                    continue; // This is not a better path

                cameFrom[n] = current;
                gScore[n] = gScoreTemp;
                fScore[n] = fScoreTemp;
            }
        }
        return;

    }

    private void ReconstructPath(Dictionary<IPath_Node, IPath_Node> camerFrom, IPath_Node current)
    {
        path = new Queue<IPath_Node>();
        path.Enqueue(current);
        while(camerFrom.ContainsKey(current))
        {
            current = camerFrom[current];
            path.Enqueue(current);
        }
        path = new Queue<IPath_Node>(path.Reverse());
    }

    private int ManhattanDist(IPath_Node startNode, IPath_Node endNode)
    {
        return Mathf.Abs(startNode.X - endNode.X) + Mathf.Abs(startNode.Y - endNode.Y);
    }
}
