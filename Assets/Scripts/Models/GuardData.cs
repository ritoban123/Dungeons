using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GuardData
{
    // For now, the Guard Data, which serves as a template for creating a guard, only contains the movement speed
    // However, it should eventually also contain values like the chance that the guard will ignore a sound
    // or how far the guard can see
    // It should also store which level the guard first appears in, and how many of it should appear in each level 
    // (probably a first level, then defined values for the next several levels,
    // then using the last defined value for all levels greater than firstLevel + levelValues.Count)

    public float MovementSpeed;
    // These functions will probably be read from a separate file (that will be easily moddable)
    public Action<Guard, float> UpdateAction;

    public int numberOfWaypoints = 3;

    public float waypointRadius = 20f;

    public float alertRadius = 12f;

    public float alertTime = 3f;

    public float maxChaseRadius = 12f;

    // FIXME: This may not be the most accurate, but it works
    public float hearingDistance = 5f;

    public float sqrHearingDist
    {
        get
        {
            return hearingDistance * hearingDistance;
        }
    }

    //public void Initialize()
    //{
    //    UpdateAction = GuardActions.BasicGuardUpdate; // TODO: Make some system where a modder can choose the update function
    //}
}
