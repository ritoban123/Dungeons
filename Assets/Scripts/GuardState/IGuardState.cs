using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuardState { PatrolState, AlertState, ChaseState }

public interface IGuardState 
{
    void ChangeState(GuardState state);
    Action<Guard, float> UpdateAction { get;  }
    GuardState state { get; }
    Guard guard { get; set; }
}
