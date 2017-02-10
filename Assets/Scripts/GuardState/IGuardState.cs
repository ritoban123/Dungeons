using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME: separate into attacking and defending
public enum GuardState { PatrolState, AlertState, ChaseState, FightingState }

public interface IGuardState 
{
    void ChangeState(GuardState state);
    Action<Guard, float> UpdateAction { get;  }
    GuardState state { get; }
    Guard guard { get; set; }
}
