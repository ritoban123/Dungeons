using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; // NOTE: This is just for testing purposes. If we actually used randomness, we would create a SYstem.Random

public class PathfindingController : MonoBehaviour
{
    Path_TileGraph tileGraph;
    Path_AStar<Path_TileGraph> aStar;
    private void Awake()
    {
        tileGraph = new Path_TileGraph();
        StartCoroutine(WaitUntilDungeonGenerationComplete());

    }

    IEnumerator WaitUntilDungeonGenerationComplete()
    {
        while (DungeonController.instance == null || DungeonController.instance.dungeon == null)
            yield return null;
        tileGraph.CreatePathfindingGraph(dungeon);
        Tile startNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];
        while(startNode.IsWall)
            startNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];

        Tile endNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];
        while (endNode.IsWall)
            endNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];

        aStar = new Path_AStar<Path_TileGraph>(tileGraph, startNode, endNode); // From the bottom left to the top right
        aStar.CalculatePath();
    }

    Dungeon dungeon
    {
        get
        {
            return DungeonController.instance.dungeon;
        }
    }

    private void OnDrawGizmos()
    {
        if (aStar == null)
        {
            return;
        }
        List<IPath_Node> path = new List<IPath_Node>(aStar.path);
        foreach(IPath_Node current in path)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(current.X, current.Y), Vector3.one * 0.3f);
        }
    }

}
