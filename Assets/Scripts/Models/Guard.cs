using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;

public class Guard
{
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public Vector2 Position
    {
        get
        {
            return new Vector2(X, Y);
        }
        set
        {
            X = value.x;
            Y = value.y;
        }
    }

    public GuardData Data { get; protected set; }

    public Guard(float startX, float startY, GuardData guardData)
    {
        X = startX;
        Y = startY;
        Data = guardData;

        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        chaseState = new ChaseState(this);

        CurrentState = patrolState;

        PatrolWayPoints = new List<Vector2>();
        SetRandomPatrolPoints(guardData.numberOfWaypoints);

        GuardParameters = new Dictionary<string, float>();
    }

    Random rand
    {
        get
        {
            return DungeonController.instance.dungeon.rand; // FIXME: Create a gameManager that stores the System.Random
        }
    }

    Dungeon dungeon
    {
        get
        {
            return DungeonController.instance.dungeon;
        }
    }

    private void SetRandomPatrolPoints(int numOfPoints)
    {
        for (int i = 0; i < numOfPoints; i++)
        {
            Vector2 pos = Vector2.zero;
            if (dungeon == null)
            {
                Debug.Log("Dungeon is Null!");
                return;
            }
            Tile t = null;
            while (t == null || t.room == null)
            {
                float r = (float)rand.NextDouble();
                r = Mathf.Sqrt(r);
                r *= Data.waypointRadius;
                float theta = (float)rand.NextDouble();
                theta *= Mathf.PI * 2;
                pos = (new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta)));
                t = dungeon.GetTileAt((int)pos.x + (int)X, (int)pos.y + (int)Y);
            }
            // At this point, we have found a valid position
            PatrolWayPoints.Add(t.Position);
        }
        WaypointQueue = new Queue<Vector2>(PatrolWayPoints);
        AdvancePath(MoveToNextWaypoint);
    }

    public IGuardState CurrentState { get; set; } // QUESTION: Should we override the default settter?
    public PatrolState patrolState;
    public AlertState alertState;
    public ChaseState chaseState;

    public List<Vector2> PatrolWayPoints { get; protected set; }
    Queue<Vector2> WaypointQueue;
    public Vector2 NextWayPoint;
    public Vector2 NextTargetPosition;

    public Path_AStar<Path_TileGraph> aStar { get; protected set; }

    public void MoveToNextWaypoint()
    {
        //Debug.Log("Guard::MoveToNextWaypoint");
        if (WaypointQueue.Count <= 0)
            WaypointQueue = new Queue<Vector2>(PatrolWayPoints);
        NextWayPoint = WaypointQueue.Dequeue();
        IPath_Node endNode = PathfindingController.instance.tileGraph.dungeon.GetTileAt(Mathf.RoundToInt(NextWayPoint.x), Mathf.RoundToInt(NextWayPoint.y));
        ChangeDestination(endNode);
        if (aStar.path == null || aStar.path.Count == 0)
        {
            Debug.LogError("A* path is null or empty!");
            //return;
        }
        //aStar.path.Dequeue(); // HACK: Disposing of the first element (where we are right now)
        //aStar.path.Dequeue(); // HACK: The first points in the path seems to be (0,0) for some reaon
    }

    public void AdvancePath(Action onPathComplete)
    {
        if ((aStar == null || aStar.path == null || aStar.path.Count == 0) && onPathComplete != null)
        {
            onPathComplete();
        }
        if(aStar.path.Count > 0)
            NextTargetPosition = aStar.path.Dequeue().Position;
        //Debug.Log(NextTargetPosition);
    }

    public void ChangeDestination(IPath_Node dest)
    {
        IPath_Node startNode = PathfindingController.instance.tileGraph.dungeon.GetTileAt(Mathf.RoundToInt(X), Mathf.RoundToInt(Y));
        if (aStar == null)
            aStar = new Path_AStar<Path_TileGraph>(PathfindingController.instance.tileGraph, startNode, dest);
        else
        {
            aStar.startNode = startNode;
            aStar.endNode = dest;
        }
        aStar.CalculatePath();
    }
    /*
     * The Guards follow a state machine:
     *  PatrolState - The Guards are moving between several randomly selected waypoints
     *  AlertState - The Guard has either seen or heard a noise, and is looking around for the threat 
     *      Pawn can hide during Alert State. Guard can ignore Alert
     *  ChaseState - The Guard has identified the threat, and is attempting to follow it
     *      The threat must remain withing 20 units of the guards current position
     *  
     *  After each alert, the chance that the guard will ignore an alert decreases
     *  A Chasing gaurd can alert other gaurds nearby
     */

    public Pawn AlertedPawn = null;

    // This is a dictionary that allows the guardActions to set parameters that belong to a specific guard
    public Dictionary<string, float> GuardParameters { get; protected set; }

    /// <summary>
    /// Returns a parameter set in the GuardParameters dictioanry.
    /// </summary>
    /// <param name="key">The string key that you are trying to get</param>
    /// <param name="defaultValue">The value that should be returned if the key is not in the dictionary</param>
    /// <returns></returns>
    public float GetParameter(string key, float defaultValue = 0)
    {
        if (GuardParameters.ContainsKey(key) == false)
        {
            GuardParameters.Add(key, defaultValue);
        }
        return GuardParameters[key];
    }

    /// <summary>
    /// Sets a parameter in the GuardParameters dictionary. Will add parameter if doesn't exist.
    /// </summary>
    /// <param name="key">The string key that you want to set</param>
    /// <param name="value">The value that you want to set it to</param>
    public void SetParameter(string key, float value)
    {
        if (GuardParameters.ContainsKey(key) == false)
        {
            GuardParameters.Add(key, value);
            return;
        }
        GuardParameters[key] = value;
    }

    /// <summary>
    /// Adds value to the current value in the dictioanry
    /// </summary>
    /// <param name="key">The key to change</param>
    /// <param name="value">The value you want to change by</param>
    /// <param name="defaultValue">If the key doesn't exist, it will be created with this default value, and the value will then be added</param>
    public void ChangeParameter(string key, float value, float defaultValue = 0)
    {
        SetParameter(key, value + GetParameter(key, defaultValue));
    }

    public void Update(float deltaTime)
    {
        // This update is set in the GuardData constructor
        // Defined in the GuardActions class, which will eventually be extracted out and be moddable
        Data.UpdateAction(this, deltaTime);
    }

}
