using UnityEngine;
using System.Collections;
using SLQJ;

/// <summary>
/// 可以自定義添加事件，然後實現消息的傳遞。
/// </summary>
// 實現手柄的案件事件功能
public class ViveEvent : MonoBehaviour
{
    void Start()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.TriggerClicked += new ClickedEventHandler(OnTriggerClicked);
        trackedController.TriggerPressDown += new ClickedEventHandler(OnTriggerPressDn);
        trackedController.TriggerUnclicked += new ClickedEventHandler(OnTriggerUnclicked);

        trackedController.PadClicked += new ClickedEventHandler(OnPadClicked);
        trackedController.PadUnclicked += new ClickedEventHandler(OnPadUnclicked);
    }
    
    void OnTriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "trigger clicked");
        // 开火
        NotificationManager.Instance.Notify(NotificationType.Gun_Fire.ToString());
    }

    void OnTriggerPressDn(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "trigger press down");
        // 
        NotificationManager.Instance.Notify(NotificationType.Gathering_Stength.ToString());
    }

    void OnTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "trigger  unclicked");
        NotificationManager.Instance.Notify(NotificationType.Gun_KeyUp.ToString());
    }
        
    void OnPadClicked(object sender, ClickedEventArgs e)
    {
        // 扔雷
        NotificationManager.Instance.Notify(NotificationType.Throw_Bomb.ToString());
        Debug.Log(e.controllerIndex + "pad clicked");
    }

    void OnPadUnclicked(object sender, ClickedEventArgs e)
    {
        Debug.Log(e.controllerIndex + "padd  un clicked");
    }
}
