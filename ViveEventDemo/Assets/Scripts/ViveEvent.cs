using UnityEngine;
using System.Collections;
using SLQJ;
using Valve.VR;

public enum HandleControllerType:int
{
    Invalid = 0,
    LeftHand = 1,
    RightHand = 2,
}     

/// <summary>
/// 可以自定義添加事件，然後實現消息的傳遞。
/// </summary>
// 實現手柄的案件事件功能
public class ViveEvent : MonoBehaviour
{
    private HandleControllerType controllerType = HandleControllerType.Invalid;
    private SteamVR_TrackedController trackedController;
    void Start()
    {
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.TriggerClicked += new ClickedEventHandler(OnTriggerClicked);
        trackedController.TriggerPressDown += new ClickedEventHandler(OnTriggerPressDn);
        trackedController.TriggerUnclicked += new ClickedEventHandler(OnTriggerUnclicked);

        trackedController.PadClicked += new ClickedEventHandler(OnPadClicked);
        trackedController.PadUnclicked += new ClickedEventHandler(OnPadUnclicked);

        CheckLeftOrRightContoller(null);
        NotificationManager.Instance.Subscribe(NotificationType.Controller_Change.ToString(), CheckLeftOrRightContoller);
        Invoke("HiddenControllerModel", 1f);
    }
    
    void OnTriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "trigger clicked");
        // 开火
        NotificationManager.Instance.Notify(NotificationType.Gun_Fire.ToString(),(int)controllerType);
    }

    void OnTriggerPressDn(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "trigger press down");
        // 
        NotificationManager.Instance.Notify(NotificationType.Gathering_Stength.ToString(), (int)controllerType);
    }

    void OnTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "trigger  unclicked");
        NotificationManager.Instance.Notify(NotificationType.Gun_KeyUp.ToString(), (int)controllerType);
    }
        
    void OnPadClicked(object sender, ClickedEventArgs e)
    {
        // 扔雷
        NotificationManager.Instance.Notify(NotificationType.Throw_Bomb.ToString(), (int)controllerType);
        Debug.Log(e.controllerIndex + "pad clicked");
    }

    void OnPadUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "padd  un clicked");
    }

    void HiddenControllerModel()
    {
        SteamVR_Utils.Event.Send("hide_render_models", true);
    }

    private void CheckLeftOrRightContoller(MessageObject obj)
    {
        ETrackedControllerRole trackerdRole = ETrackedControllerRole.Invalid;
        var system = Valve.VR.OpenVR.System;
        if (system != null && null != trackedController)
        {
            trackerdRole = system.GetControllerRoleForTrackedDeviceIndex(trackedController.controllerIndex);
        }

        #region use by string name as legacy
#if By_Name_String
        controllerType = HandleControllerType.Invalid;
        if (this.name.Contains("left"))
        {
            controllerType = HandleControllerType.LeftHand;
        }
        if (this.name.Contains("right"))
        {
            controllerType = HandleControllerType.RightHand;
        }
#endif
        #endregion
        controllerType = (HandleControllerType)((int)trackerdRole);
    }
}
