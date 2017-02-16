using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    // HACK: Because we cannot require T to inherit from Enum
    public class FiniteStateMachine<TState, TActions> where TState : struct, IComparable, IFormattable, IConvertible
    {
        // FIXME: Add this back in later
        //public bool[,] TransitionMatrix;

        // ALERT: ONly needed to improve speed. Getvalues is slow
        protected int NumOfStates { get {
                if (numOfStates == 0)
                    return Enum.GetValues(typeof(TState)).Length;
                else
                    return numOfStates;
            } }
        protected int numOfStates;

        public FiniteStateMachine(/*bool[,] transitionMatrix*/)
        {
            if (typeof(TState).IsEnum == false)
            {
                throw new ArgumentException(typeof(TState).ToString() + " is not an enum", "T");

            }

            if (NumOfStates < 1) throw new ArgumentException(typeof(TState).ToString() + " includes only 1 visible value");

            // FIXME: Add this back in later

            //if(transitionMatrix.GetLength(0) != NumOfStates || transitionMatrix.GetLength(1) != NumOfStates)
            //{
            //    throw new ArgumentException("Transition Matrix is not the same length as the enum", "transitionMatrix");
            //}

            //this.TransitionMatrix = transitionMatrix;
        }

        State CurrentState;
        Dictionary<TState, State> EnumStateMap;

        public void Initialize(TState StartState)
        {
            EnumStateMap = new Dictionary<TState, State>();
            Type actionType = typeof(TActions);
            foreach (TState state in Enum.GetValues(typeof(TState)))
            {

                State s = new State((Action<float>)Delegate.CreateDelegate(actionType, actionType.GetMethod(state.ToString() + "_Update")),
                    // QUESTION: Does the enter function need to know the last state?
                    (Action)Delegate.CreateDelegate(actionType, actionType.GetMethod(state.ToString() + "_Enter")),
                    (Action)Delegate.CreateDelegate(actionType, actionType.GetMethod(state.ToString() + "_Exit")));

                EnumStateMap.Add(state, s);
            }
            CurrentState = EnumStateMap[StartState];
        }

        public void ChangeState(TState state)
        {

            GameManager.instance.OnUpdate -= CurrentState.updateAction;
            CurrentState.exitAction();
            CurrentState = EnumStateMap[state];
            CurrentState.enterAction();
            GameManager.instance.OnUpdate += CurrentState.updateAction;
        }

    }
}