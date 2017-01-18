using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public static DungeonController instance;

    public Dungeon dungeon { get; protected set; }
    //int width = 551;
    //int height = 451;
    int width = 101;
    int height = 51;
    int maxRooms = 1024;
    int maxRoomAttempts = 1024;
    int minRoomSize = 4;
    int maxRoomSize = 16;
    float deadEndRemovalChance = 0.99f;
    float extraConnectorChance = 0.025f;

    bool drawGizmos = false;
    private void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        dungeon = new Dungeon(width, height, maxRooms, maxRoomAttempts, minRoomSize, maxRoomSize, extraConnectorChance, deadEndRemovalChance, 1236);
        if (dungeon == null)
        {
            // the dungeon generation failed for some reason
            Debug.LogError("DungeonController::Start - The dungeon generation failed!");
            return;
        }
        if (drawGizmos)
            return;
        GetSprites();
        CreateGameObjects();

        Camera.main.transform.position = new Vector3(dungeon.Width / 2, dungeon.Height / 2, -10);
    }

    Dictionary<string, Sprite> AllSpritesByName = new Dictionary<string, Sprite>();

    private void GetSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
        foreach (Sprite s in sprites)
        {
            AllSpritesByName.Add(s.name, s);
            spriteNrmMap.Add(s, 
                Resources.Load<Texture>("Sprites/" + s.name + "_nrm")
            ); // FIXME: Hard coding in name
        }
    }


    private void CreateGameObjects()
    {
        // This is just an empty material with a basic bumped sprite shader
        Material diffuseMat = Resources.Load<Material>("Materials/Sprites_Diffuse");
        for (int y = 0; y < dungeon.Height; y++)
        {
            // Create a parent for each row, just to keep stuff organised
            Transform parent = new GameObject("Row " + y.ToString()).transform;
            parent.parent = this.transform;
            parent.position = new Vector3(0, y/* - dungeon.Height / 2f + 0.5f*/);
            for (int x = 0; x < dungeon.Width; x++)
            {
                // Get the tile from the dungeon
                Tile tile_data = dungeon.GetTileAt(x, y);
                // Create an empty game object
                GameObject tile_obj = new GameObject("Tile " + x + "_" + y);
                // Position that gameObject
                tile_obj.transform.position = new Vector3(x/* - (dungeon.Width / 2f + 0.5f)*/, y /*- (dungeon.Height / 2f + 0.5f)*/);
                // Set the parent accordingly
                tile_obj.transform.SetParent(parent);
                // Add a sprite renderer
                SpriteRenderer sr = tile_obj.AddComponent<SpriteRenderer>();
                // Assign the sprite. GetSpriteForTile uses some data from the Tile class and finds the appropriate sprite from a dictionary of sprites
                sr.sprite = GetSpriteForTile(tile_data);
                // Make sure the sprite renderer is using the bumped diffuse material (should allow batching)
                sr.sharedMaterial = diffuseMat;
                // If we successfully found a sprite, check our Dictionary that links sprites and normal maps to assign the appropriate normal map.
                if(sr.sprite != null && spriteNrmMap[sr.sprite] != null)
                    sr.material.SetTexture("_BumpMap", spriteNrmMap[sr.sprite]);
            }
        }
    }

    Dictionary<Sprite, Texture> spriteNrmMap = new Dictionary<Sprite, Texture>();

    public Sprite GetSpriteForTile(Tile t)
    {
        if (t.room != null)
        {
            return AllSpritesByName["Cobblestone"]; // FIXME: Hard coding! Should use TileType enum or store sprite name on tile
        }
        if (t.isCorridor)
            return AllSpritesByName["Stones"];
        if (t.IsWall)
            return AllSpritesByName["RockWall"];
        if (t.isConnector)
            return AllSpritesByName["Door"];
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
