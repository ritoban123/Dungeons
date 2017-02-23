using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    // HACK: Because we cannot require T to inherit from Enum
    public class FiniteStateMachine<TState, TActions> where TState : struct, IComparable, IFormattable, IConvertible
    {
        // FIXME: Add this back in later
        //public bool[,] TransitionMatrix;

        // ALERT: ONly needed to improve speed. Getvalues is slow
        protected int NumOfStates
        {
            get
            {
                if (numOfStates == 0)
                    return Enum.GetValues(typeof(TState)).Length;
                else
                    return numOfStates;
            }
        }
        protected int numOfStates;

        public object objectToPass { get; protected set; }

        public FiniteStateMachine(object objectToPass = null)
        {
            if (typeof(TState).IsEnum == false)
            {
                throw new ArgumentException(typeof(TState).ToString() + " is not an enum", "T");

            }

            if (NumOfStates < 1) throw new ArgumentException(typeof(TState).ToString() + " includes only 1 visible value");

            this.objectToPass = objectToPass;

            // FIXME: Add this back in later

            //if(transitionMatrix.GetLength(0) != NumOfStates || transitionMatrix.GetLength(1) != NumOfStates)
            //{
            //    throw new ArgumentException("Transition Matrix is not the same length as the enum", "transitionMatrix");
            //}

            //this.TransitionMatrix = transitionMatrix;
        }

        public State CurrentState { get; protected set; }
        Dictionary<TState, State> EnumStateMap;
        public Dictionary<State, TState> StateEnumMap { get; protected set; }

        public void Initialize(TState StartState)
        {
            EnumStateMap = new Dictionary<TState, State>();
            StateEnumMap = new Dictionary<State, TState>();
            Type actionType = typeof(TActions);
            foreach (TState state in Enum.GetValues(typeof(TState)))
            {
                MethodInfo update = actionType.GetMethod(state.ToString() + "_Update");
                MethodInfo enter = actionType.GetMethod(state.ToString() + "_Enter");
                MethodInfo exit = actionType.GetMethod(state.ToString() + "_Exit");

                State s = new State();
                if (update != null)
                    s.updateAction = (Action<float, object>)Delegate.CreateDelegate(typeof(Action<float, object>), update);
                if (enter != null)
                    s.enterAction = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), enter);
                if (exit != null)
                    s.exitAction = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), exit);

                EnumStateMap.Add(state, s);
                StateEnumMap.Add(s, state);
            }
            ChangeState(StartState);
            //Debug.Log(StateEnumMap[CurrentState]);
        }

        Action<float> currentUpdateAction;

        public void ChangeState(TState state)
        {
            if (CurrentState != null)
            {
                if (CurrentState.updateAction != null)
                    GameManager.instance.OnUpdate -= currentUpdateAction;
                if (CurrentState.exitAction != null)
                    CurrentState.exitAction(objectToPass);
            }



            CurrentState = EnumStateMap[state];

            if (CurrentState.enterAction != null)
                CurrentState.enterAction(objectToPass);
            if (CurrentState.updateAction != null)
                currentUpdateAction = (dt) => { CurrentState.updateAction(dt, objectToPass); };
            GameManager.instance.OnUpdate += currentUpdateAction;
        }

    }
}