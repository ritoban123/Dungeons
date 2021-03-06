﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn
{
    public PawnCardData Data;
    public float X { get; protected set; }
    public float Y { get; protected set; }

    private Vector2 targetPosition;
    
    // FIXME: This is a misnomer. It get returns the next one in the patch, set calculates a path to the final target position.
    // we may need to separate these two out later
    public Vector2 TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set
        {
            if (value != targetPosition)
            {
                FinalTargetPosition = value;
                IPath_Node startNode = PathfindingController.instance.tileGraph.dungeon.GetTileAt(Mathf.RoundToInt(X), Mathf.RoundToInt(Y));
                IPath_Node endNode = PathfindingController.instance.tileGraph.dungeon.GetTileAt(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
                aStar = new Path_AStar<Path_TileGraph>(PathfindingController.instance.tileGraph, startNode, endNode);
                aStar.CalculatePath();
            }
            //List<IPath_Node> nodes = new List<IPath_Node>(aStar.path);
            //targetPosition = nodes[nodes.Count - 1].Position;
            
            targetPosition = aStar.path.Dequeue().Position;
        }
    }

    public Vector2 FinalTargetPosition;


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

    public Path_AStar<Path_TileGraph> aStar;

    public Pawn(PawnCardData data, float startX, float startY)
    {
        Data = data;
        X = startX;
        Y = startY;
        TargetPosition = Position; // We don't want to start out moving!
    }

    /// <summary>
    /// Should be called evry frame. Moves X and Y Position toward Target Position
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        // TODO: Add A* pathfinding to get to the destination the player set
        if ((Position - TargetPosition).sqrMagnitude <= deltaTime * 0.4f) 
        {
            if (aStar.path.Count > 0)
                targetPosition = aStar.path.Dequeue().Position;
            else
                return;
        }
        Vector2 movement = TargetPosition - Position;
        Position += movement.normalized * Data.movementSpeed * deltaTime;
    }

    //public void OnDrawGizmos()
    //{
    //    if (aStar == null)
    //    {
    //        return;
    //    }
    //    Queue<IPath_Node> path = new Queue<IPath_Node>(aStar.path);
    //    Debug.Log("OnDrawGizmos");
    //    while (path.Count > 0)
    //    {
    //        IPath_Node current = path.Dequeue();
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawCube(new Vector3(current.X, current.Y), Vector3.one * 0.3f);
    //    }
    //}
}
