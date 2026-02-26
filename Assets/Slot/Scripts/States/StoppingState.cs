using AxGrid;
using AxGrid.FSM;
using UnityEngine;

[State("StoppingState")]
public class StoppingState : FSMState
{
    [Enter]
    public void Enter()
    {
        Debug.Log("ENTER: STOPPING");

        Model.Set("IsStopEnabled", false);
        Model.EventManager.AddAction("OnSnaped", OnSnaped);
    }

    //[One(2f)]
    //void AfterDelay()
    //{
    //}
   
    void OnSnaped()
    {        
        Debug.Log("!!!!!!Stopping → Idle");
        Parent.Change("IdleState");
    }
}