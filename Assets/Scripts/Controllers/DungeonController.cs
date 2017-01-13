using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    Dungeon dungeon;
    int width = 301;
    int height = 201;
    int maxRooms = 1024;
    int maxRoomAttempts = 1024;
    int minRoomSize = 4;
    int maxRoomSize = 16;
    float deadEndRemovalChance = 0.99f;
    float extraConnectorChance = 0.025f;

    bool drawGizmos = false;
    private void Start()
    {
        dungeon = new Dungeon(width, height, maxRooms, maxRoomAttempts, minRoomSize, maxRoomSize, extraConnectorChance, deadEndRemovalChance, 1236);
        if(dungeon == null)
        {
            // the dungeon generation failed for some reason
            Debug.LogError("DungeonController::Start - The dungeon generation failed!");
            return;
        }
        if (drawGizmos)
            return;
        GetSprites();
        CreateGameObjects();
    }

    Dictionary<string, Sprite> AllSpritesByName = new Dictionary<string, Sprite>();

    private void GetSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
        foreach (Sprite s in sprites)
        {
            AllSpritesByName.Add(s.name, s);
        }
    }


    private void CreateGameObjects()
    {
        Material diffuseMat = Resources.Load<Material>("Materials/Sprites_Diffuse");
        for (int y = 0; y < dungeon.Height; y++)
        {
            Transform parent = new GameObject("Row " + y.ToString()).transform;
            parent.parent = this.transform;
            parent.position = new Vector3(0, y - dungeon.Height/2f + 0.5f);
            for (int x = 0; x < dungeon.Width; x++)
            {
                Tile tile_data = dungeon.GetTileAt(x, y);
                GameObject tile_obj = new GameObject("Tile " + x + "_" + y);
                tile_obj.transform.position = new Vector3(x - (dungeon.Width/2f + 0.5f), y - (dungeon.Height / 2f + 0.5f));
                tile_obj.transform.SetParent(parent);
                SpriteRenderer sr = tile_obj.AddComponent<SpriteRenderer>();
                sr.sprite = GetSpriteForTile(tile_data);
                sr.sharedMaterial = diffuseMat;
            }
        }
    }


    public Sprite GetSpriteForTile(Tile t)
    {
        if(t.room != null)
        {
            return AllSpritesByName["Cobblestone"];
        }
        if (t.isCorridor)
            return AllSpritesByName["Stones"];
        if (t.IsWall)
            return AllSpritesByName["RockWall"];

        return null;
    }

    private void OnDrawGizmos()
    {
        if (dungeon == null || drawGizmos == false)
            return;
        foreach (Tile t in dungeon.Tiles)
        {
            Gizmos.color = Color.white;
            Gizmos.color = (t.room == null) ? Gizmos.color : t.room.c;
            Gizmos.color = (t.isCorridor && t.room == null) ? new Color(0.2f, 0.2f, 0.2f) : Gizmos.color;
            //if (t.region == null) // Wall
            //    Gizmos.color = Color.white;
            //else if(t.region.PartOfMain)
            //{
            //    Gizmos.color = Color.blue;
            //}
            //else
            //{
            //    Gizmos.color = new Color((t.region.index * 50) / 255f, (t.region.index * 50) / 255f, (t.region.index * 50F) / 255f);
            //}

            Gizmos.color = (t.isConnector == true) ? Color.green : Gizmos.color;
            Gizmos.color = (t.debug == true) ? Color.red : Gizmos.color;
            //if (t.X == 6 && t.Y == 16)
            //{
            //    Gizmos.color = Color.red;
            //}
            Gizmos.DrawCube(new Vector3(t.X, 0, t.Y) + new Vector3(0.5f, 0, 0.5f), new Vector3(.9f, 0.1f, .9f));
        }
    }
}
