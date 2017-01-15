using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn
{
    public PawnCardData Data;
    public float X { get; protected set; }
    public float Y { get; protected set; }

    public Pawn(PawnCardData data, float startX, float startY)
    {
        Data = data;
        X = startX;
        Y = startY;
    }

    internal void Update(float deltaTime)
    {
        // TODO: Add A* pathfinding to get to the destination the player set
        X += deltaTime;
        Y += deltaTime;
    }
}
