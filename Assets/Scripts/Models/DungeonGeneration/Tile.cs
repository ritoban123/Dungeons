using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Tile : IPath_Node
{
    public Tile(int x, int y, Dungeon dungeon)
    {
        X = x;
        Y = y;
        this.dungeon = dungeon;
    }

    public int X { get; set; } // FIXME: Ideally, I would like to keep protected set, but interfaces don't allow accessor protection
    public int Y { get; set; }

    public Vector2 Position
    {
        get
        {
            return new Vector2(X, Y);
        }
        set
        {
            X = Mathf.RoundToInt(value.x);
            Y = Mathf.RoundToInt(value.y);
        }
    }

    public Dungeon dungeon { get; protected set; }

    public Room room;
    public bool isCorridor = false;
    public bool isConnector = false;
    public bool debug = false;

    public Region region;

    public Tile NorthNeighbor_2
    {
        get
        {
            return dungeon.GetTileAt(X, Y - 2);
        }
    }
    public Tile EastNeighbor_2
    {
        get
        {
            return dungeon.GetTileAt(X + 2, Y);
        }
    }
    public Tile SouthNeighbor_2
    {
        get
        {
            return dungeon.GetTileAt(X, Y + 2);
        }
    }
    public Tile WestNeighbor_2
    {
        get
        {
            return dungeon.GetTileAt(X - 2, Y);
        }
    }

    public Tile NorthNeighbor
    {
        get
        {
            return dungeon.GetTileAt(X, Y - 1);
        }
    }
    public Tile EastNeighbor
    {
        get
        {
            return dungeon.GetTileAt(X + 1, Y);
        }
    }
    public Tile SouthNeighbor
    {
        get
        {
            return dungeon.GetTileAt(X, Y + 1);
        }
    }
    public Tile WestNeighbor
    {
        get
        {
            return dungeon.GetTileAt(X - 1, Y);
        }
    }

    //public Region GetOtherRegion(Region mainRegion)
    //{
    //    //if (isConnector == false)
    //    //    Debug.LogError("Trying to get other region of non connector tile: " + X + ", " + Y);
    //    Region[] neighborTileRegions = this.GetNeighbors().Select<Tile, Region>((neighbor) => { return neighbor.region; }).ToArray<Region>();
    //    Region otherRegion = null;
    //    foreach (Region r in neighborTileRegions)
    //    {
    //        if (r == null)
    //        {
    //            // one of the tiles that connectos to this is a wall, we don't need to worry about telling it that it is conneced
    //            continue;
    //        }
    //        else if (r != mainRegion)
    //        {
    //            // This is the region that it connects to!
    //            otherRegion = r;
    //            break;
    //        }
    //    }
    //    if (otherRegion == null)
    //    {
    //        Debug.Log("We could not find any regions other than Region " + mainRegion.index + " that connector to Tile " + this.X + ", " + this.Y);
    //        return null;
    //    }
    //    return otherRegion;
    //}

    public Tile[] GetNeighbors_2()
    {
        List<Tile> neighbors = new List<Tile>(); ;
        if (NorthNeighbor_2 != null)
            neighbors.Add(NorthNeighbor_2);
        if (EastNeighbor_2 != null)
            neighbors.Add(EastNeighbor_2);
        if (SouthNeighbor_2 != null)
            neighbors.Add(SouthNeighbor_2);
        if (WestNeighbor_2 != null)
            neighbors.Add(WestNeighbor_2);
        return neighbors.ToArray();
    }

    public Tile[] GetNeighbors()
    {
        List<Tile> neighbors = new List<Tile>(); ;
        if (NorthNeighbor != null)
            neighbors.Add(NorthNeighbor);
        if (EastNeighbor != null)
            neighbors.Add(EastNeighbor);
        if (SouthNeighbor != null)
            neighbors.Add(SouthNeighbor);
        if (WestNeighbor != null)
            neighbors.Add(WestNeighbor);
        return neighbors.ToArray();
    }

    public Tile[] GetWallNeighbors()
    {
        Tile[] neighbors = GetNeighbors_2();
        List<Tile> result = new List<Tile>();
        foreach (Tile t in neighbors)
        {
            if (t.room == null && t.isCorridor == false)
                result.Add(t);
        }
        return result.ToArray();
    }

    public int MovementCost
    {
        get
        {
            // TODO: Take props (like chests/weapons/armor) into account
            if (room != null)
                return 50;
            else if (isCorridor == true)
                return 100;
            else if (isConnector == true)
                return 150;
            else
                return int.MaxValue;
        }
    }

    /// <summary>
    /// This should be non-wall neighbors!
    /// </summary>
    /// <returns></returns>
    IPath_Node[] IPath_Node.GetNeighbors()
    {
        Tile[] ns =  GetNeighbors();
        List<Tile> nwns = new List<Tile>(); // Non-wall Neighbors
        foreach(Tile n in ns)
        {
            if (n.IsWall)
                continue;
            nwns.Add(n);
        }
        return nwns.ToArray();
    }

    public bool IsWall { get { return ((this.room == null && this.isCorridor == false && this.isConnector == false)); } }
    public bool EvaluateDeadEnd
    {
        get
        {
            if (NorthNeighbor == null || EastNeighbor == null || SouthNeighbor == null || WestNeighbor == null)
                return false;
            if (isCorridor == false)
                return false;
            return
                (NorthNeighbor.IsWall && EastNeighbor.IsWall && SouthNeighbor.IsWall) ||
                (NorthNeighbor.IsWall && EastNeighbor.IsWall && WestNeighbor.IsWall) ||
                (NorthNeighbor.IsWall && SouthNeighbor.IsWall && WestNeighbor.IsWall) ||
                (EastNeighbor.IsWall && SouthNeighbor.IsWall && WestNeighbor.IsWall);
        }
    }
}
