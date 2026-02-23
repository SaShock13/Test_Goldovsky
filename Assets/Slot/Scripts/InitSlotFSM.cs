using UnityEngine;
using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;

public class InitSlotFsm : MonoBehaviourExt
{
    [OnAwake]
    void Create()
    {
        Model.Set("IsStartEnabled", true);
        Model.Set("IsStopEnabled", true);
        Settings.Fsm = new FSM();

        Settings.Fsm.Add(new IdleState());
        Settings.Fsm.Add(new AcceleratingState());
        Settings.Fsm.Add(new SpinningState());
        Settings.Fsm.Add(new StoppingState());

        Debug.Log("MY FSM Created");
    }

    [OnStart]
    void StartFsm()
    {
        Debug.Log($"StartFsm {this}");
        Settings.Fsm.Start("IdleState");
    }


    [OnUpdate]
    void UpdateFsm()
    {
        Settings.Fsm.Update(Time.deltaTime);
    }
}
