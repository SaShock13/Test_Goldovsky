using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

[State("IdleState")]
public class IdleState : FSMState
{
    [Enter]
    void Enter()
    {
        Debug.Log("!!!!!!!!!!!!ENTER: IDLE");
        Debug.Log("FSM Model Hash: " + Model.GetHashCode());
        Model.Set("IsStopEnabled", false);    
        Model.Set("IsStartEnabled", true);
    }


    [Bind("StartPressed")]
    void OnStartPressed()
    {
        Debug.Log("Idle → Accelerating");
        Parent.Change("AcceleratingState");
    }
}