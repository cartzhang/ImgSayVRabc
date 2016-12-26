using UnityEngine;
using SLQJ;

public class ControlButtonAns : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        NotificationManager.Instance.Subscribe(NotificationType.Gun_Fire.ToString(), GunFire);
        NotificationManager.Instance.Subscribe(NotificationType.Gathering_Stength.ToString(), GatheringStength);
        NotificationManager.Instance.Subscribe(NotificationType.Throw_Bomb.ToString(), ThrowBomb);
        NotificationManager.Instance.Subscribe(NotificationType.Gun_KeyUp.ToString(), GunKeyUp);
    }

    void GunFire(MessageObject obj)
    {
        Debug.Log("response gun fire , trigger button click");
    }

    void GatheringStength(MessageObject obj)
    {
        Debug.Log("response gathering stength, trigger button hold");
    }

    void GunKeyUp(MessageObject obj)
    {
        Debug.Log("response key up, trigger button unclicked");
    }

    void ThrowBomb(MessageObject obj)
    {
        Debug.Log("response throw bomb , pad button click");
    }
}
