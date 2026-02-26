using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

[State("SpinningState")]
public class SpinningState : FSMState
{

    [Enter]
    public void Enter()
    {
        Debug.Log("ENTER: SPINNING");
        Model.Set("IsStopEnabled", true);
        Model.Set("SpinMode", "Spinning");
    }

    [Bind("StopPressed")]
    void OnStopPressed()
    {
        Model.Set("SpinMode", "Stopping");
        Debug.Log("Spinning → Stopping");
        Parent.Change("StoppingState");

    }
}