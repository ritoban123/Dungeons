using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GuardController : MonoBehaviour
{
    public static GuardController instance;

    public int NumberOfGuards = 10; // TODO: Let the level number decide the number of guards OR Let the guardData decide how many guards in each level
    public GuardData[] AllGuardData;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitUntilDungeonGenerationComplete());
    }

    IEnumerator WaitUntilDungeonGenerationComplete()
    {
        while (DungeonController.instance == null || DungeonController.instance.dungeon == null)
            yield return null;
        // FIXME: We are using whether gaurds is null as an indicator of whther SpawnAllGuards has finished
        guardGameObjectMap = new Dictionary<Guard, GameObject>(NumberOfGuards);
        gameObjectGuardMap = new Dictionary<GameObject, Guard>(NumberOfGuards);
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
            AllGuardData[i].UpdateAction = GuardActions.BasicGuardUpdate; // This function should handle everything (including states and specific values for each guard)
        }
        for (int i = 0; i < NumberOfGuards; i++)
        {
            // Spawn a Guard
            SpawnGuard(i);
        }
    }

    //Guard[] guards;
    Dictionary<Guard, GameObject> guardGameObjectMap;
    Dictionary<GameObject, Guard> gameObjectGuardMap; // FIXME: Create a data structure to handle both of the dictioanries at the same time

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
        GameObject obj = CreateGameObjectForGuard(guard);
        guardGameObjectMap[guard] = obj;
        gameObjectGuardMap[obj] = guard;
    }

    public Guard GetGuardForGameObject(GameObject obj)
    {
        if(gameObjectGuardMap.ContainsKey(obj) == false)
        {
            Debug.LogError(obj.name + " is not a guard");
            return null;
        }
        return gameObjectGuardMap[obj];
    }


    public GameObject GetGameObjectForGuard(Guard g)
    {
        if (guardGameObjectMap.ContainsKey(g) == false)
        {
            Debug.LogError("Guard at " +  g.Position + " is not a guard");
            return null;
        }
        return guardGameObjectMap[g];
    }
    GameObject CreateGameObjectForGuard(Guard g)
    {
        GameObject obj = new GameObject("Guard");
        obj.transform.parent = this.transform;
        obj.transform.position = g.Position;
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/Guards/Triangle");
        sr.color = Color.red; // TODO: Eventually, use a separate graphic for the guards (and animation maybe?)
        return obj;
    }

    private void Update()
    {
        if (guardGameObjectMap == null)
            return;
        foreach (Guard g in guardGameObjectMap.Keys)
        {
            g.Update(Time.deltaTime);
            guardGameObjectMap[g].transform.position = g.Position;
        }
    }

    private void OnDrawGizmos()
    {
        if (guardGameObjectMap == null)
            return;
        foreach (Guard g in guardGameObjectMap.Keys)
        {
            foreach (Vector2 pos in g.PatrolWayPoints)
            {
                Gizmos.color = Color.red;
                //Gizmos.DrawCube(pos, Vector2.one * 0.3f);
            }
            Gizmos.DrawWireSphere(g.Position, g.Data.alertRadius);
        }
    }
}
