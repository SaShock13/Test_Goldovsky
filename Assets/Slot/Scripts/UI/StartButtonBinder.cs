using AxGrid;
using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonBinder : MonoBehaviourExtBind
{
    public Button button;

    [Bind("OnIsStartEnabledChanged")]
    void OnStartEnabledChanged(bool value)
    {
        Debug.Log("StartButton interactable = " + value);
        button.interactable = value;
    }
}