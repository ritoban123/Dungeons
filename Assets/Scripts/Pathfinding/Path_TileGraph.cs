    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph : Path_Graph
{
    public override List<IPath_Node> nodes { get; set; }

    public Path_TileGraph()
    {
        nodes = new List<IPath_Node>();
    }


    public void CreatePathfindingGraph(Dungeon dungeon)
    {
        for (int y = 0; y < dungeon.Height; y++)
        {
            for (int x = 0; x < dungeon.Width; x++)
            {
                IPath_Node node = dungeon.GetTileAt(x, y);
                node.X = x;
                node.Y = y;
                this.nodes.Add(node);
            }
        }
    }
}
