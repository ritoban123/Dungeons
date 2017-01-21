using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GuardController : MonoBehaviour
{
    public int NumberOfGuards = 10; // TODO: Let the level number decide the number of guards OR Let the guardData decide how many guards in each level
    public GuardData[] AllGuardData;

    private void Start()
    {
        StartCoroutine(WaitUntilDungeonGenerationComplete());
    }

    IEnumerator WaitUntilDungeonGenerationComplete()
    {
        while (DungeonController.instance == null || DungeonController.instance.dungeon == null)
            yield return null;
        // FIXME: We are using whether gaurds is null as an indicator of whther SpawnAllGuards has finished
        guards = new Guard[NumberOfGuards];
        SpawnAllGuards();
    }

    Dungeon Dungeon
    {
        get
        {
            return DungeonController.instance.dungeon;
        }
    }

    // Dammit Unity! I want to use C# 6.0 and expression bodied properties, or whaterever the hell there called!
    Random rand
    {
        get
        {
            return DungeonController.instance.dungeon.rand;
        }
    }

    private void SpawnAllGuards()
    {
        for (int i = 0; i < AllGuardData.Length; i++)
        {
            AllGuardData[i].Initialize(); // HACK: This is just to assign the UpdateAction
        }
        for (int i = 0; i < NumberOfGuards; i++)
        {
            // Spawn a Guard
            SpawnGuard(i);
        }
    }

    Guard[] guards;

    private void SpawnGuard(int index)
    {
        // NOTE: Tile 0, 0 should always be a wall, meaning its room is null
        int startX = 0;
        int startY = 0;
        // This should be executed the first time since 0,0 is a wall
        while (Dungeon.GetTileAt(startX, startY).room == null)
        {
            // The position was not in a room. Get another tile
            startX = rand.Next(Dungeon.Width);
            startY = rand.Next(Dungeon.Height);
        }

        // TODO: The GuardData also stores information about what level it should first appear in and how many it should spawn in that level
        Guard guard = new Guard(startX, startY, AllGuardData[rand.Next(AllGuardData.Length)]);
        guards[index] = guard;
        
    }

    private void Update()
    {
        if (guards == null)
            return;
        for (int i = 0; i < guards.Length; i++)
        {
            guards[i].Update(Time.deltaTime);
        }
    }
}
