
using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;
using UnityEngine.UI;

public class StopButtonBinder : MonoBehaviourExtBind
{
    public Button button;

    //[AxGrid.Model.Bind("IsStopEnabled")]
    [Bind("OnIsStopEnabledChanged")]
    void OnStopEnabledChanged(bool value)
    {
        Debug.Log("StopButton interactable = " + value);
        button.interactable = value;
    }

    [OnStart]
    void CheckModel()
    {
        Debug.Log("Binder Model Hash: " + Model.GetHashCode());
    }
}