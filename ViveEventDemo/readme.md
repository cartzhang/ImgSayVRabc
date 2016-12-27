
Unity的 Steam VR插件本身也带有事件处理。但是我还想把事件给解耦出来，这样方便在各个项目中，不用关心硬件的各种处理而只用关心使用的，且可以任意的通过接受事件来触发相应的操作。
今天我们说谈论的就是下面这个东西：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/1.1.png)

## 一、所需资源

所需资源，很少。
需要用Steam VR插件 ，可以从Untiy商店下载。当然你可以使用文章后面给出本工程的导出包，文章后面有下载地址：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/0.png)

但是电脑还是需要安装steam的，这个暂时还是需要翻墙的。你懂的，翻墙是一项技能。
安装后：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/4.png)

点击右上角的VR字样，链接你的Vive设备，然后就可以看到他们的状态了。
这里设备的各种设置方法和使用就不逐个说明讲解了。网上搜索下吧，或去官方最正宗的。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/6.png)

当然也可以使用桌面的快捷方式，当然前提是你有：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/5.png)

## 二、制作Demo

首先，打开Unity 导入插件

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/1.png)

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/2.png)

然后，可以打开其给点样例来看看:

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/3.png)

接着就是，添加代码，给Controller添加控制代码：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/7.png)


在然后就是需要自己写代码。


## 三、消息解耦

先说下，这个代码来自于其他同事，我基本没太多修改。但是确实很好用，非常感谢！！若有问题，请及时告知。


消息发送机制：


```

namespace SLQJ
{
    /// <summary>
    /// 消息分发，解耦
    /// </summary>
    public class NotificationManager
    {
        public static NotificationManager Instance { get { return SingletonProvider<NotificationManager>.Instance; } }

        public delegate void MsgCallback(MessageObject eb);
        /// <summary>
        /// 回调队列
        /// </summary>
        private Dictionary<string, List<MsgCallback>> registedCallbacks = new Dictionary<string, List<MsgCallback>>();
        /// <summary>
        /// 延迟消息队列
        /// </summary>
        private readonly List<MessageObject> delayedNotifyMsgs = new List<MessageObject>();
        /// <summary>
        /// 主消息队列
        /// </summary>
        private readonly List<MessageObject> realCallbacks = new List<MessageObject>();
        private static bool isInCalling = false;

        public  void Init()
        {

        }

        public void Update()
        {
            lock (this)
            {
                if (realCallbacks.Count == 0)
                {
                    //主消息隊列處理完時,加入延時消息到主消息列表 
                    foreach (MessageObject eb in delayedNotifyMsgs)
                    {
                        realCallbacks.Add(eb);
                    }
                    delayedNotifyMsgs.Clear();
                    return;
                }
                //調用主消息處理隊列
                isInCalling = true;
                foreach (MessageObject eb in realCallbacks)
                {
                    if (registedCallbacks.ContainsKey(eb.MsgName))
                    {
                        for (int i = 0; i < registedCallbacks[eb.MsgName].Count; i++)
                        {
                            MsgCallback ecb = registedCallbacks[eb.MsgName][i];
                            if (ecb == null)
                            {
                                continue;
                            }
							#if UNITY_EDITOR
                            ecb(eb);
							#else
                            try
                            {
                                 ecb(eb);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("CallbackError:" + eb.MsgName + " : " + e.ToString());
                            }    
							#endif
                        }
                    }
                    else
                    {
                        Debug.Log("MSG_ALREADY_DELETED:" + eb.MsgName);
                    }

                }
                realCallbacks.Clear();
            }
            isInCalling = false;
        }

        public void Reset()
        {
            Dictionary<string, List<MsgCallback>> systemMsg = new Dictionary<string, List<MsgCallback>>();
            foreach (KeyValuePair<string, List<MsgCallback>> item in this.registedCallbacks)
            {
                if (item.Key.StartsWith("_"))
                {
                    systemMsg.Add(item.Key, item.Value);
                }
            }
            this.registedCallbacks = systemMsg;
        }

        public void Destroy()
        {
            Reset();
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="msgCallback"></param>
        public void Subscribe(string msgName, MsgCallback msgCallback)
        {
            lock (this)
            {
                if (!registedCallbacks.ContainsKey(msgName))
                {
                    registedCallbacks.Add(msgName, new List<MsgCallback>());
                }
                {
                    //防止重复订阅消息回调
                    List<MsgCallback> list = registedCallbacks[msgName];
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Equals(msgCallback))
                        {
                            return;
                        }
                    }
                    list.Add(msgCallback);
                }

            }
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="msgCallback"></param>
        public void UnSubscribe(string msgName, MsgCallback msgCallback)
        {
            lock (this)
            {
                if (!registedCallbacks.ContainsKey(msgName))
                {
                    return;
                }
                //Debug.Log(msgName + ":-s-" + registedCallbacks[msgName].Count);
                registedCallbacks[msgName].Remove(msgCallback);
                //Debug.Log(msgName + ":-e-" + registedCallbacks[msgName].Count);
            }
        }

        public void PrintMsg()
        {
            string content = "";
            foreach (KeyValuePair<string, List<MsgCallback>> registedCallback in registedCallbacks)
            {
                int total = registedCallback.Value.Count;
                if (total > 0)
                {
                    content += registedCallback.Key + ":" + total + "\n";
                    for (int i = 0; i < total; i++)
                    {
                        content += "\t" + registedCallback.Value[i].Method.Name + "--" + registedCallback.Value[i].Target + "\n";
                    }
                }
            }
        }

        /// <summary>
        /// 派发消息
        /// </summary>
        /// <param name="MsgName"></param>
        /// <param name="MsgParam"></param>
        public void Notify(string MsgName, params object[] MsgParam)
        {

            object msgValueParam = null;
            if (MsgParam != null)
            {
                if (MsgParam.Length == 1)
                {
                    msgValueParam = MsgParam[0];
                }
                else
                {
                    msgValueParam = MsgParam;
                }
            }


            lock (this)
            {
                if (!registedCallbacks.ContainsKey(MsgName))
                {
                    return;
                }
                if (isInCalling)
                {
                    delayedNotifyMsgs.Add(new MessageObject(MsgName, msgValueParam));
                }
                else
                {
                    realCallbacks.Add(new MessageObject(MsgName, msgValueParam));
                }
            }
        }
    }

    public class MessageObject
    {
        public object MsgValue;
        public string MsgName;

        public MessageObject()
        {
            MsgName = this.GetType().FullName;
        }

        public MessageObject(string msgName, object ev)
        {
            MsgValue = ev;
            MsgName = msgName;
        }
    }
}

```

你可以看到原著者写的还是很严谨的，使用消息队列来实现的，然后在unity某组件的Update中实现轮询调用。

先看看这个消息机制的启动，特别简单：


```
public class main : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
        NotificationManager.Instance.Init();
    }
	
	// Update is called once per frame
	void Update ()
    {
        NotificationManager.Instance.Update();
    }
}
```

与上面说的一模一样，初始化，然后update。

至于说机制怎么用，这个在后面会接实战给出。

## 四、手柄Controller消息触发

手柄的事件很多，我就捡了几个常用的来做个例子来说明问题，若需要，你们自己可以来添加自己的需要。


```

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

```

主要写了按键Trigger 按下，按住和弹起和Pad的按下和弹起事件。

然后是触发事件的接受，这里就体现了解耦事件的好处。这里真的不止于使用在vive按键处理这里。


```

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

```


这个就根据个人的需要来添加自己的代码。这里仅仅是举例说明。

代码写完了，添加吧！！

手柄contorller接受事件：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/7.1.png)

消息触发解耦代码：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/7.2.png)

相应消息脚本：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/7.3.png)

这样基本就搞定了。

## 五、结果

一图胜千言：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/Img/8.png)

就这样。

## 六、下载地址

工程下载地址：github

https://github.com/cartzhang/ImgSayVRabc/tree/master/ViveEventDemo

steam 插件工程导出地址：

https://github.com/cartzhang/ImgSayVRabc/blob/master/ViveEventDemo/SteamViveControllerEventDemoCartzhang.unitypackage

##七、 参考

[1] http://www.cnblogs.com/czaoth/p/5610883.html

[2] http://www.htc.com/managed-assets/shared/desktop/vive/Vive_PRE_User_Guide.pdf