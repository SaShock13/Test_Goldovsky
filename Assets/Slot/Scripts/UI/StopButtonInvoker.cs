using AxGrid;
using AxGrid.Base;
using UnityEngine;
using UnityEngine.UI;

public class StopButtonInvoker : MonoBehaviourExt
{
    public Button button;

    [OnStart]
    void Init()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log("Stop Button Clicked");
        Model.Set("StopPressed", true);
        Settings.Invoke("StopPressed", Model.Get<bool>("StopPressed"));
    }
}