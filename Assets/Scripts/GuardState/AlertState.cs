using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : IGuardState
{
    public GuardState state
    {
        get
        {
            return GuardState.AlertState;
        }
    }

    public Action<Guard, float> UpdateAction
    {
        get
        {
            return GuardActions.BasicAlertStateUpdate;
        }
    }

    public Guard guard { get; set; }

    public AlertState(Guard g)
    {
        guard = g;
    }



    public void ChangeState(GuardState state)
    {
        switch (state)
        {
            case GuardState.PatrolState:
                // The alerted guard could not find anything to chase
                guard.CurrentState = guard.patrolState;
                break;
            case GuardState.AlertState:
                Debug.LogError("AlertState::ChangeState - Can't switch from AlertState to AlertState");
                break;
            case GuardState.ChaseState:
                guard.CurrentState = guard.chaseState;
                break;
        }
    }
}
