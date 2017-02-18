using System;
using System.Reflection;
using System.Collections;
using UnityEngine;


namespace StateMachine
{
    public class State
    {
        public Action<float> updateAction;
        public Action enterAction;
        public Action exitAction;

    }
}