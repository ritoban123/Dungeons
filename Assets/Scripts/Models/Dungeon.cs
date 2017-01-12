using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Dungeon
{
    public Tile[,] Tiles { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public Random rand;
    int MaxRooms;
    int MaxRoomAttempts;
    int MaxRoomSize;
    int MinRoomSize;
    float ExtraConnectorChance;
    float DeadEndRemovalChance;

    public List<Room> Rooms { get; protected set; }

    public Dungeon(int width, int height, int maxRooms, int maxRoomAttempts, int minRoomSize, int maxRoomSize, float roomConnectorChance, float deadEndRemovalChance, int seed)
    {
        if (width % 2 != 1)
        {
            Debug.LogError("Width must be odd!");
            return;
        }
        if (height % 2 != 1)
        {
            Debug.LogError("Height must be odd!");
            return;
        }
        Width = width;
        Height = height;
        MaxRooms = maxRooms;
        MaxRoomAttempts = maxRoomAttempts;
        MaxRoomSize = maxRoomSize;
        MinRoomSize = minRoomSize;
        ExtraConnectorChance = roomConnectorChance;
        DeadEndRemovalChance = deadEndRemovalChance;


        Rooms = new List<Room>();
        Tiles = new Tile[width, height];
        rand = new Random(seed);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tiles[x, y] = new Tile(x, y, this);
            }
        }
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        GenerateRooms();
        GenerateCorridors();
        AssignRegions();
        GeneratePossibleConnectors();
        // TODO: Setup with regions
        // DONE

        //foreach (Room r in Rooms)
        //{
        //    r.GenerateConnectors(RoomConnectorChance);
        //}
        DeadEndFilling();
    }

    /**
     * Create an array of regions, 0 is corridor, null is wall, 1+ are rooms
     * for each tile:
     *  if corridor == false && room == null
     *      set regioin to null
     *  else if corridor == true
     *      set region to regions[0]
     *  else if room != null
     *      set region to regions[Rooms.IndexOf(tile.room) + 1]
     *      
     *      
     *  For connectors:
     *      for each tile:
     *          get regions of 4 neighboring tiles
     *              set connector if there are 2 different non-null regions
     *              {
     *                  Region r = null;
     *                  for(int i = 0; i less regions.Length; i++)
     *                  {
     *                      if(regions[i] == null)
     *                          continue;
     *                      else if(r == null)
     *                          r = regions[i];
     *                      else if(r != regions[i])
     *                          isConnector = true;
     *                          break;
     *                      else
     *                          continue;
     *                  }
     *              }
     */
    private void GeneratePossibleConnectors()
    {

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile t = GetTileAt(x, y);
                Region[] neighborTileRegions = t.GetNeighbors().Select<Tile, Region>((neighbor) => { return neighbor.region; }).ToArray<Region>();
                //if(x == 6 && y == 16)
                //{
                //    foreach (Region nr in neighborTileRegions)
                //    {
                //        if (nr != null)
                //            Debug.Log(nr.index);
                //        else
                //            Debug.Log("Null");
                //    } 
                //}
                // set connector if there are at least two different regions that are not null in the list (0, 1, null, null) works, but (0, 0, null null) doesn't
                Region r = null;
                for (int i = 0; i < neighborTileRegions.Length; i++)
                {
                    if (neighborTileRegions[i] == null)
                        continue;
                    else if (r == null)
                        r = neighborTileRegions[i];
                    else if (r != neighborTileRegions[i])
                    {
                        Connector c = new Connector(t, r, neighborTileRegions[i]);
                        r.connectorTiles.Add(c);
                        neighborTileRegions[i].connectorTiles.Add(c);
                        //t.isConnector = true;
                        break;
                    }
                    else
                        continue;
                }

            }
        }
        ChooseConnectors();
    }

    Region mainRegion = null;

    private void ChooseConnectors()
    {
        // STEP 1: Pick a random room to be the main region.
        mainRegion = allRegions[1]; // NOTE: Regions[0] is the corridor, this could work
        while (mainRegion.connectorTiles.Count > 0)
        {
            //for (int j = 0; j < 3; j++)
            //{
            //if (j == 2)
            //{
            //    Connector connector2 = mainRegion.connectorTiles[rand.Next(0, mainRegion.connectorTiles.Count)];
            //    connector2.Tile.isConnector = true;
            //    return;
            //}
            // STEP 2: Pick a random connector that touches the main region and open it up.
            Connector connector = mainRegion.connectorTiles[rand.Next(0, mainRegion.connectorTiles.Count)];
            connector.Tile.isConnector = true;

            // STEP 3: The connected region is now part of the main one. Unify it.
            Region otherRegion = connector.GetOtherRegion(mainRegion, true);
            mainRegion.connectorTiles.AddRange(otherRegion.connectorTiles);
            otherRegion.connectorTiles = null;

            //connector.CheckMainRegion(mainRegion);

            // STEP 4: Remove any extraneous connectors.
            for (int i = mainRegion.connectorTiles.Count - 1; i >= 0; i--)
            {
                Connector c = mainRegion.connectorTiles[i];
                if (c.GetOtherRegion(mainRegion,true) == otherRegion)
                {
                    if (rand.NextDouble() <= ExtraConnectorChance)
                    {
                        // This is another connector between main region and the other region
                        // This connector should be garbage collected, because other region's list has been set to null
                        mainRegion.connectorTiles[i].Tile.isConnector = true;
                    }
                    mainRegion.connectorTiles.RemoveAt(i);
                }
                else if (c.GetOtherRegion(otherRegion, false) == null)
                    continue;
                else if (c.GetOtherRegion(otherRegion, false).PartOfMain)
                {
                    if (rand.NextDouble() <= ExtraConnectorChance)
                    {
                        // This is another connector between main region and the other region
                        // This connector should be garbage collected, because other region's list has been set to null
                        mainRegion.connectorTiles[i].Tile.isConnector = true;
                    }
                    mainRegion.connectorTiles.RemoveAt(i);
                    // Effectivily, this is testing for the same thing as the other if statemt
                    //mainRegion.connectorTiles[i].Tile.debug = true;
                    //if (rand.NextDouble() <= ExtraConnectorChance)
                    //{
                    //    // This is another connector between main region and the other region
                    //    // This connector should be garbage collected, because other region's list has been set to null
                    //    mainRegion.connectorTiles.RemoveAt(i);
                    //}
                    //else
                    //{
                    //    mainRegion.connectorTiles[i].Tile.isConnector = true;
                    //}
                }
            }

        }
        // STEP 5: If there are still connectors left, go to #2.
    }

    //private void ChooseConnectors()
    //{
    //    // STEP 1: Pick a random room to be the main region.
    //    Region mainRegion = regions[1]; // The first ROOM / Rooms[0]
    //    partOfMain.Add(mainRegion);
    //    // STEP 2: Pick a random connector that touches the main region and open it up.
    //    Tile connector = mainRegion.connectorTiles[rand.Next(0, mainRegion.connectorTiles.Count)];
    //    connector.isConnector = true;
    //    // STEP 3: The connected region is now part of the main one. Unify it.
    //    // We need to add the other regions that connector connects to to the main region
    //    Region otherRegion = connector.GetOtherRegion(mainRegion);
    //    partOfMain.Add(otherRegion);

    //    // STEP 4: Remove any extraneous connectors.
    //    foreach (Tile c in otherRegion.connectorTiles)
    //    {
    //        if (c.GetOtherRegion(otherRegion) == mainRegion)
    //        {

    //        }

    //    }

    //}

    //HashSet<Region> partOfMain = new HashSet<Region>();

    Region[] allRegions;

    private void AssignRegions()
    {
        allRegions = new Region[Rooms.Count + 1]; // NOTE: We are used regions[0] as the corridor, and null as the wall

        for (int i = 0; i < allRegions.Length; i++)
        {
            allRegions[i] = new Region(i);
        }

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile t = GetTileAt(x, y);
                if (t.IsWall)
                    t.region = null;
                else if (t.isCorridor)
                    t.region = allRegions[0];
                else if (t.room != null)
                {
                    t.region = allRegions[Rooms.IndexOf(t.room) + 1];
                }
                else
                    Debug.LogError("Tile (" + x + ", " + y + ") is not a wall, corridor, or part of a room!");
            }
        }
    }

    private void DeadEndFilling()
    {
        /* 
         * Find all tiles with 3 wall neighbors
         */
        Queue<Tile> tilesToRemove = new Queue<Tile>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile t = GetTileAt(x, y);

                if (t.EvaluateDeadEnd && rand.NextDouble() <= DeadEndRemovalChance)
                {
                    //Debug.Log(t.X + " " + t.Y);
                    tilesToRemove.Enqueue(t);
                }
            }
        }

        while (tilesToRemove.Count > 0)
        {
            Tile t = tilesToRemove.Dequeue();
            t.isCorridor = false;
            Tile[] neighbors = t.GetNeighbors();
            foreach (Tile neighbor in neighbors)
            {
                if (neighbor.EvaluateDeadEnd && rand.NextDouble() <= DeadEndRemovalChance)
                    tilesToRemove.Enqueue(neighbor);
            }
        }
    }



    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return null;
        return Tiles[x, y];
    }

    List<Tile> corridorTiles = new List<Tile>();

    private void GenerateCorridors()
    {
        int startX = 0;
        int startY = 0;
        while (startX % 2 == 0 || startY % 2 == 0 || Tiles[startX, startY].room != null)
        {
            startX = rand.Next(1, Width);
            startY = rand.Next(1, Height);
        }
        Tile current = GetTileAt(startX, startY);
        current.isCorridor = true;
        corridorTiles.Add(current);
        Stack<Tile> tileStack = new Stack<Tile>();
        do
        {
            Tile[] neighbors = current.GetWallNeighbors();
            if (neighbors.Length > 0)
            {
                Tile randNeighbor = neighbors[rand.Next(0, neighbors.Length)];
                tileStack.Push(current);
                randNeighbor.isCorridor = true;
                corridorTiles.Add(randNeighbor);
                // Remove wall BETWEEN current and randNeighbor
                int xDiff = randNeighbor.X - current.X;
                int yDiff = randNeighbor.Y - current.Y;
                if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
                {
                    Tile t = GetTileAt(current.X + (int)Mathf.Sign(xDiff), current.Y);
                    t.isCorridor = true;
                    corridorTiles.Add(t);
                }
                else
                {
                    Tile t = GetTileAt(current.X, current.Y + (int)Mathf.Sign(yDiff));
                    t.isCorridor = true;
                    corridorTiles.Add(t);
                }
                current = randNeighbor;
            }
            else if (tileStack.Count > 0)
            {
                current = tileStack.Pop();
            }
        } while (tileStack.Count > 0);
    }

    private void GenerateRooms()
    {
        int attempts = 0;
        while (attempts < MaxRoomAttempts && Rooms.Count < MaxRooms)
        {
            attempts++;
            int xPos = 0;
            while (xPos % 2 == 0) // While x is even, try to make x odd
                xPos = rand.Next(1, Width);
            int yPos = 0;
            while (yPos % 2 == 0) // While y is even, try to make x odd
                yPos = rand.Next(1, Height);

            int sizeX = 0;
            while (sizeX % 2 == 0) // While sizeX is even
                sizeX = rand.Next(MinRoomSize, MaxRoomSize);

            int sizeY = 0;
            while (sizeY % 2 == 0) // While sizeY is even
                sizeY = rand.Next(MinRoomSize, MaxRoomSize);
            bool roomPositionFailed = false;
            // FIXME: Iterating from -2 to +2 to ensure that each room is bordered by enough space for a corridor. 
            // HACK - can be fixed by making connectors follow regions
            for (int x = xPos - 1; x < xPos + sizeX + 1; x++)
            {
                if (roomPositionFailed)
                    break;
                for (int y = yPos - 1; y < yPos + sizeY + 1; y++)
                {
                    //// Debug.Log(x + "_" + y + " : " + sizeX + "_" + sizeY);
                    //if ((x < 1 && xPos > 1) || x >= Width || y >= Height || (y < 1 && yPos > 1))
                    //{
                    //    roomPositionFailed = true;
                    //    break;
                    //}
                    Tile t = GetTileAt(x, y);
                    if (t == null)
                    {
                        roomPositionFailed = true;
                        break;
                    }
                    if (Tiles[x, y].room != null)
                    {
                        roomPositionFailed = true;
                        break;
                    }
                }
            }
            if (roomPositionFailed)
                continue;
            Room room = new Room(xPos, yPos, sizeX, sizeY);
            Tile[,] roomTiles = new Tile[sizeX, sizeY];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x >= xPos && x < xPos + sizeX && y >= yPos && y < yPos + sizeY)
                    {
                        Tiles[x, y].room = room;
                        roomTiles[x - xPos, y - yPos] = Tiles[x, y];
                    }
                }
            }
            room.TilesInRoom = roomTiles;
            Rooms.Add(room);
        }
    }
}
