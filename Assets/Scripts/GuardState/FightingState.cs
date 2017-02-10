using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingState : IGuardState
{
    public GuardState state
    {
        get
        {
            return GuardState.FightingState;
        }
    }

    public Action<Guard, float> UpdateAction
    {
        get
        {
            return GuardActions.BasicFightingStateUpdate;
        }
    }

    public Guard guard { get; set; }

    public FightingState(Guard g)
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
                guard.CurrentState = guard.fightingState;
                break;
            case GuardState.ChaseState:
                guard.CurrentState = guard.chaseState;
                break;
            case GuardState.FightingState:
                Debug.LogError("FightingState::ChangeState - Can't switch from FightingState to FightingState");
                break;
        }
    }
}
