using System;
using System.Reflection;
using System.Collections;
using UnityEngine;


namespace StateMachine
{
    public class State
    {
        public Action<float, object> updateAction;
        public Action<object> enterAction;
        public Action<object> exitAction;

    }
}