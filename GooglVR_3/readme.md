

之前分析了一些GoogleVR的代码，画了一些图。
常言：他山之石，可以攻玉。个人觉得googleVR这不能说是石头了吧，入宝山空手而归，岂是程序的风格。我们希望可以拿着googleVR的玉，最起码刻个玉玺吧。哈哈！！

这篇记录一下，google VR 在Unity中代码的写的不错的地方。
只是比较不错的代码。

## 一、 补上googleVR实现过程
googleVR 之前仍旧觉得对它的实现过程没有说的很明白，上篇重点在于它的代码的运行过程。
现在补上一张整体实现过程，希望对有需要的人有帮助：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GooglVR_3/Img/1.png)

要说的是，它这个写的还是很不错的，赞一个。
因为不像其他各种插件，你需要导入到unity工程中后，需要各种配置预制体，这个只需要把一个预制体拽到场景中，并且位置随意，真是智能的很。
再看看人家的实现流程，不吝赞美之词。所以，要看看他的代码。若对我来说，需要说的是，就是我这大括号对齐的毛病。哎呀，代码风格，兼容并包吧。

## 二、gvrViewer的值得借鉴的代码

### 1. 类的单例模式


```
/// The singleton instance of the GvrViewer class.
  public static GvrViewer Instance {
    get {
    #if UNITY_EDITOR
      if (instance == null && !Application.isPlaying) {
        instance = UnityEngine.Object.FindObjectOfType<GvrViewer>();
      }
   #endif
      if (instance == null) {
        Debug.LogError("No GvrViewer instance found.  Ensure one exists in the scene, or call "
            + "GvrViewer.Create() at startup to generate one.\n"
            + "If one does exist but hasn't called Awake() yet, "
            + "then this error is due to order-of-initialization.\n"
            + "In that case, consider moving "
            + "your first reference to GvrViewer.Instance to a later point in time.\n"
            + "If exiting the scene, this indicates that the GvrViewer object has already "
            + "been destroyed.");
      }
      return instance;
    }
  }
  
   private static GvrViewer instance = null;
```

首字母小写的instance为私有变量，首字母大写的为共有静态变量，来实现单例模式。

在编辑器模式下，就直接找到GvrViewer的对象为单例。但是这个是配合它Awake的代码来写的，自己要这么写也要看自己的需要。

### 2. 静态对象变量

StereoController类的Controller，是一个只有get属性的变量。


```
public static StereoController Controller {
    get {
     #if !UNITY_HAS_GOOGLEVR || UNITY_EDITOR
      Camera camera = Camera.main;
      // Cache for performance, if possible.
      if (camera != currentMainCamera || currentController == null) {
        currentMainCamera = camera;
        currentController = camera.GetComponent<StereoController>();
      }
      return currentController;
     #else
      return null;
     #endif  // !UNITY_HAS_GOOGLEVR || UNITY_EDITOR
    }
  }
  private static StereoController currentController;
  private static Camera currentMainCamera;

```
首先，我们看到整个googleVR中都有宏定义

```
	#if !UNITY_HAS_GOOGLEVR || UNITY_EDITOR
	
	#else
	
	#endif  // !UNITY_HAS_GOOGLEVR || UNITY_EDITOR


```
代码功能是找到主相机，然后把组件StereoController给添加到主相机上。实现的功能就是分屏的控制，左右眼设置等操作。

主要在于看这个代码形式。

### 3.带get和set属性的变量


```
public RenderTexture StereoScreen {
    get {
      // Don't need it except for distortion correction.
      if (!distortionCorrectionEnabled || !VRModeEnabled) {
        return null;
      }
      if (stereoScreen == null) {
        // Create on demand.
        StereoScreen = device.CreateStereoScreen();  // Note: uses set{}
      }
      return stereoScreen;
    }
    set {
      if (value == stereoScreen) {
        return;
      }
      if (stereoScreen != null) {
        stereoScreen.Release();
      }
      stereoScreen = value;
      if (OnStereoScreenChanged != null) {
        OnStereoScreenChanged(stereoScreen);
      }
    }
  }
```

这与我们常用的int a;这样变量的区别在于它的get属性和set属性。这个是渲染纹理(RenderTexture)类。
Set的过程，先需要把之前的渲染纹理给Release释放掉，然后在set还添加了OnStereoScreenChanged的委托事件。



## 三、GvrProfile可鉴代码

这个是枚举值


```
/// Some known screen profiles.
  public enum ScreenSizes {
    Nexus5,
    Nexus6,
    GalaxyS6,
    GalaxyNote4,
    LGG3,
    iPhone4,
    iPhone5,
    iPhone6,
    iPhone6p,
  };

```

这个是Screen类的对象：

```
/// Parameters for a Nexus 5 device.
  public static readonly Screen Nexus5 = new Screen {
    width = 0.110f,
    height = 0.062f,
    border = 0.004f
  };

  /// Parameters for a Nexus 6 device.
  public static readonly Screen Nexus6 = new Screen {
    width = 0.133f,
    height = 0.074f,
    border = 0.004f
  };

  // 不全部列举了
  // .. .
```

发现奥秘了么？
看图
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GooglVR_3/Img/2.png)
红框内的在vs中代码颜色都一样。我辨认都混淆了。

看到ScreenSizes这个是个枚举类。他里面的枚举值Nexus5 等，都是枚举值类型，隐形继承的是int。
但是在同文件内，就还有一个Nexus5的变量，其类型为Screen类。

这个做的很神奇啊。一开始在其他地方使用的时候，我还以为是写错了呢，结果编译器也不吭气，表示抗议。看来gogole写的还真是有玄机。

那两个不相关的东西怎么对应和关联的呢？

```

  /// Returns a profile with the given parameters.
  public static GvrProfile GetKnownProfile(ScreenSizes screenSize, ViewerTypes deviceType) {
    Screen screen;
    switch (screenSize) {
      case ScreenSizes.Nexus6:
        screen = Nexus6;
        break;
      case ScreenSizes.GalaxyS6:
        screen = GalaxyS6;
        break;
      case ScreenSizes.GalaxyNote4:
        screen = GalaxyNote4;
        break;
      case ScreenSizes.LGG3:
        screen = LGG3;
        break;
      case ScreenSizes.iPhone4:
        screen = iPhone4;
        break;
      case ScreenSizes.iPhone5:
        screen = iPhone5;
        break;
      case ScreenSizes.iPhone6:
        screen = iPhone6;
        break;
      case ScreenSizes.iPhone6p:
        screen = iPhone6p;
        break;
      default:
        screen = Nexus5;
        break;
    }
    Viewer device;
    switch (deviceType) {
      case ViewerTypes.CardboardMay2015:
        device = CardboardMay2015;
        break;
      case ViewerTypes.GoggleTechC1Glass:
        device = GoggleTechC1Glass;
        break;
      default:
        device = CardboardJun2014;
        break;
    }
    return new GvrProfile { screen = screen, viewer = device };
  }
```
这样就看明白了吧！！
就是这样switch就做了关联。其在EditorDevice.cs中的UpdateScreenData中调用。

## 四、 告一段落

老生长谈，路漫漫其修远兮。

googleVR就代码还有UI部分代码及其controller的代码都没有具体涉及到。

还有一些就是在宏作用下，暂时在Unity或googlVR模式下，被屏蔽的代码。
但是还是希望这些东西对需要的人，有所帮助。



## 五、参考

[1] https://blogs.unity3d.com/2016/09/22/daydream-technical-preview-available-now/

[2]  https://github.com/googlevr/gvr-unity-sdk/tree/master/GoogleVR
