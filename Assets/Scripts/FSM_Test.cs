using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public enum FSM_Test_States { Good, Bad, OK }
public class FSM_Test : MonoBehaviour
{
    FiniteStateMachine<FSM_Test_States, FSM_Test> fsm;
    // Use this for initialization
    void Start()
    {
        fsm = new FiniteStateMachine<FSM_Test_States, FSM_Test>(this);
        fsm.Initialize(FSM_Test_States.Good);
    }

    private void Update()
    {
        //Debug.Log(fsm.StateEnumMap[fsm.CurrentState]);
        if (Input.GetMouseButtonUp(0))
        {
            fsm.ChangeState(FSM_Test_States.Bad);
        }
    }

    public static void Good_Enter(object obj)
    {
        Debug.Log("I entered Good State");
    }

    public static void Good_Update(float dt, object obj)
    {
        if (obj == null)
            Debug.Log("I am in Good State");
        else
            Debug.Log(obj.ToString());
    }

    public static void Good_Exit(object obj)
    {
        Debug.Log("I exited Good State");
    }

    public static void Bad_Enter(object obj)
    {
        Debug.Log("I entered Bad State");
    }

    public static void Bad_Update(float dt, object obj)
    {
        Debug.Log("I am in Bad State");
    }

    public static void Bad_Exit(object obj)
    {
        Debug.Log("I exited Bad State");
    }

}
