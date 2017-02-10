using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IGuardState
{
    public GuardState state
    {
        get
        {
            return GuardState.ChaseState;
        }
    }

    public Action<Guard, float> UpdateAction
    {
        get
        {
            return GuardActions.BasicChaseStateUpdate;
        }
    }

    public Guard guard { get; set; }

    // QUESTION: Should we make IGuardState an abstract class, so we can provide this as a base constructor?
    public ChaseState(Guard g)
    {
        guard = g;
    }



    public void ChangeState(GuardState state)
    {
        switch (state)
        {
            case GuardState.PatrolState:
                // We lost the thing we were chasing
                guard.CurrentState = guard.patrolState;
                break;
            case GuardState.AlertState:
                Debug.LogError("ChaseState::ChangeState - Can't switch from ChaseState to AlertState");
                break;
            case GuardState.ChaseState:
                Debug.LogError("ChaseState::ChangeState - Can't switch from ChaseState to ChaseState");
                break;
            case GuardState.FightingState:
                guard.CurrentState = guard.fightingState;
                break;
        }
    }
}
