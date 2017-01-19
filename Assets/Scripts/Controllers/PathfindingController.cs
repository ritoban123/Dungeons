using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; // NOTE: This is just for testing purposes. If we actually used randomness, we would create a SYstem.Random

public class PathfindingController : MonoBehaviour
{
    public static PathfindingController instance;
    public Path_TileGraph tileGraph { get; protected set; }
    //Path_AStar<Path_TileGraph> aStar;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        tileGraph = new Path_TileGraph();
        StartCoroutine(WaitUntilDungeonGenerationComplete());

    }

    IEnumerator WaitUntilDungeonGenerationComplete()
    {
        while (DungeonController.instance == null || DungeonController.instance.dungeon == null)
            yield return null;
        tileGraph.CreatePathfindingGraph(dungeon);
        /*Tile startNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];
        while(startNode.IsWall)
            startNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];

        Tile endNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];
        while (endNode.IsWall)
            endNode = (Tile)tileGraph.nodes[Random.Range(0, tileGraph.nodes.Count)];

        aStar = new Path_AStar<Path_TileGraph>(tileGraph, startNode, endNode); // From the bottom left to the top right
        aStar.CalculatePath();*/
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

        //if (aStar == null)
        //{
        //    return;
        //}
        //Queue<IPath_Node> path = new Queue<IPath_Node>(aStar.path);
        //while (path.Count > 0)
        //{
        //    IPath_Node current = path.Dequeue();
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawCube(new Vector3(current.X, current.Y), Vector3.one * 0.3f);
        //}
    }

}
