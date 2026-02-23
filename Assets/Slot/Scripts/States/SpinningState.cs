using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

[State("SpinningState")]
public class SpinningState : FSMState
{
    private SlotScroller scroller;

    [Enter]
    public void Enter()
    {
        Debug.Log("ENTER: SPINNING");
        Model.Set("IsStopEnabled", true);
        scroller = GameObject.FindAnyObjectByType<SlotScroller>();
        scroller.StartAccelerating(300f); // стартовая скорость
    }

    [Bind("StopPressed")]
    void OnStopPressed()
    {
        scroller.StopSlot();
        Debug.Log("Spinning → Stopping");
        Parent.Change("StoppingState");

    }
}