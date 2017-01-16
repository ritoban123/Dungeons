using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Room
{
    public Room(int x, int y, int sizeX, int sizeY)
    {
        X = x;
        Y = y;
        SizeX = sizeX;
        SizeY = sizeY;
        TilesInRoom = new Tile[sizeX, sizeY];
    }

    public int X { get; protected set; }
    public int Y { get; protected set; }
    public int SizeX { get; protected set; }
    public int SizeY { get; protected set; }
    public Tile[,] TilesInRoom { get; set; }
    public List<Tile> Connecectors = new List<Tile>();

    Dungeon dungeon { get { return TilesInRoom[0, 0].dungeon; } }

    public Color c = UnityEngine.Random.ColorHSV(0, 1, 0, 0.5f, 0.5f, 0.8f);

    //public void GenerateConnectors(float chance)
    //{
    //    for (int x = -1; x <= SizeX; x++)
    //    {
    //        for (int y = -1; y <= SizeY; y++)
    //        {
    //            if (x != -1 && x != SizeX && y != -1 && y != SizeY)
    //                continue;
    //            // Are we at a diagnol
    //            // FIXME: Do not like hard-coding long if statements
    //            if ((x == -1 && y == -1) || (x == -1 && y == SizeY) || (y == -1 && x == SizeX) || (x == SizeX && y == SizeY))
    //                continue;
    //            Tile t = dungeon.GetTileAt(X + x, Y + y);
    //            if (dungeon.rand.NextDouble() < chance)
    //            {
    //                t.isConnector = true;
    //                Connecectors.Add(dungeon.GetTileAt(X + x, Y + y));
    //            }
    //        }
    //    }
    //}
}
