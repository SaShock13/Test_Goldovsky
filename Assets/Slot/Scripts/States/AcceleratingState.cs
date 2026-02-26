using AxGrid;
using AxGrid.FSM;
using UnityEngine;

[State("AcceleratingState")]
public class AcceleratingState : FSMState
{
    [Enter]
    public void Enter()
    {
        Debug.Log("ENTER: ACCELERATING");

        Model.Set("IsStartEnabled", false);
        Model.Set("SpinMode", "Accelerating"); 
    }

    [One(3f)]
    void AfterDelay()
    {
        Debug.Log("Accelerating → Spinning");
        Parent.Change("SpinningState");
       
    }
}