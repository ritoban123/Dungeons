using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void Update(float deltaTime)
    {
        // This update is set in the GuardData constructor
        // Defined in the GuardActions class, which will eventually be extracted out and be moddable
        Data.UpdateAction(this, deltaTime);
    }

}
