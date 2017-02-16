using System;
using System.Collections;
using UnityEngine;


namespace FiniteStateMachine
{
    public class State
    {
        public Action<float> updateAction;
        public Action enterAction;
        public Action exitAction;

        public State(Action<float> updateAction, Action enterAction, Action exitAction)
        {
            this.updateAction = updateAction;
            this.enterAction = enterAction;
            this.exitAction = exitAction;
        }
    }
}