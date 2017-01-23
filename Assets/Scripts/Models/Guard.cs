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

        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        chaseState = new ChaseState(this);

        CurrentState = patrolState;
    }

    public IGuardState CurrentState { get; set; } // QUESTION: Should we override the default settter?
    public PatrolState patrolState;
    public AlertState alertState;
    public ChaseState chaseState;

    /*
     * The Guards follow a state machine:
     *  Patrol - The Guards are moving between several randomly selected waypoints
     *  Alert - The Guard has either seen or heard a noise, and is looking around for the threat 
     *      Pawn can hide during Alert State. Guard can ignore Alert
     *  Chase - The Guard has identified the threat, and is attempting to follow it
     *      The threat must remain withing 20 units of the guards current position
     *  
     *  After each alert, the chance that the guard will ignore an alert decreases
     *  A Chasing gaurd can alert other gaurds nearby
     */ 

    public void Update(float deltaTime)
    {
        // This update is set in the GuardData constructor
        // Defined in the GuardActions class, which will eventually be extracted out and be moddable
        Data.UpdateAction(this, deltaTime);
    }

}
