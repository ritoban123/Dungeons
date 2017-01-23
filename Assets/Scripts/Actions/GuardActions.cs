using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GuardActions 
{
    public static void BasicGuardUpdate(Guard g, float deltaTime)
    {
        g.CurrentState.UpdateAction(g, deltaTime);
    }

    public static void BasicPatrolStateUpdate(Guard g, float deltaTime)
    {
        Debug.Log("Patrol" + g.Position);
        if (Input.GetButtonDown("Fire1"))
            g.CurrentState.ChangeState(GuardState.AlertState);
    }

    public static void BasicAlertStateUpdate(Guard g, float deltaTime)
    {
        Debug.Log("Alert" + g.Position);
        if (Input.GetButtonDown("Fire1"))
            g.CurrentState.ChangeState(GuardState.PatrolState);
        if (Input.GetButtonDown("Fire2"))
            g.CurrentState.ChangeState(GuardState.ChaseState);
    }
    public static void BasicChaseStateUpdate(Guard g, float deltaTime)
    {
        Debug.Log("Chase" + g.Position);
        if (Input.GetButtonDown("Fire1"))
            g.CurrentState.ChangeState(GuardState.PatrolState);   
    }

}
