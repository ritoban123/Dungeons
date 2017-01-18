using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingController : MonoBehaviour
{
    Path_TileGraph tileGraph;
    private void Awake()
    {
        tileGraph = new Path_TileGraph();
    }

    IEnumerator WaitUntilDungeonGenerationComplete()
    {
        while (DungeonController.instance.dungeon == null)
            yield return null;
        tileGraph.CreatePathfindingGraph(dungeon);
    }

    Dungeon dungeon
    {
        get
        {
            return DungeonController.instance.dungeon;
        }
    }

}
