using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn
{
    public PawnCardData Data;
    public float X { get; protected set; }
    public float Y { get; protected set; }

    public Vector2 TargetPosition/* = UnityEngine.Random.insideUnitCircle * 20*/;
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
        if ((Position - TargetPosition).sqrMagnitude <= 0.0005)
            return;

        Vector2 movement = TargetPosition - Position;
        Position += movement.normalized * Data.movementSpeed * deltaTime;
    }
}
