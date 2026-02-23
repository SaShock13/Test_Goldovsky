using AxGrid;
using AxGrid.Base;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonInvoker : MonoBehaviourExt
{
    public Button button;

    [OnStart]
    void Init()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log("Start Button Clicked");
        Model.Set("StartPressed", true);
        Settings.Invoke("StartPressed", Model.Get<bool>("StartPressed"));
    }
}