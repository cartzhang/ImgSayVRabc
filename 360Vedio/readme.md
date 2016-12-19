

本章用使用较早的UnityOC 插件来实现一个360全景视频，且通过使用不同的路径配置，可以随意切换视频内容。这样省去了多次打包的过程，简单易用。
当然，你可以花费40刀来购买一个。
https://www.assetstore.unity3d.com/cn/#!/content/35102
这个还是听贵的。
制作一个免费的吧！！！

##一、所需资源

简单制作一个360的全景视频，需要资源有：
AVPro Windows Media2.9.0 unity插件
下载地址：
http://pan.baidu.com/s/1bz7pc6

需要安装OC的0.4.4runtime:
http://pan.baidu.com/s/1jIJPXCa
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/1.png)
图1

Untiy所使用版本：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/2.png)
图2


## 二、制作一个360的VR视频

### 1.导入插件

导入AVPro Windows Media2.9.0插件

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/6.png)
图6

找到他给出的预制体如下图：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/7.png)
图7

然后选择视频文件：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/8.png)
图8
### 2.解码器安装

若这时候运行，就会报错，如下
根据报错：
For MP4 files you need an MP4 splitter such as Haali Media Splitter or GDCL.

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/9.png)
图9

若感兴趣想深入了解的，可以看看如下地址：

http://haali.su/mkv/

若来这里直接下载所需要的解码器：
下载地址：

然后安装：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/10.png)
图10

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/11.png)
图11

然后再次加载Load测试：
若看到如下就说明解码器安装成功

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/12.png)
图12

### 3.设置视频加载配置和运行设置

首先，需要把原来的主相机屏蔽，然后添加OC的相机预制体

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/13.png)
图13

然后，添加写好的配置文件代码：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/15.png)
图15

最后要注意，不要勾选Virtual Reality supported选项，因为我们使用的是低版本的OCruntime插件。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/14.png)
图14

当然，我也打包测试了OC 0.8.0的Runtime,可能相对与0.4.4版本，OC造成的崩溃可能性会小点。这个没有详细测试，只是相信Unity嵌入和OC的版本升级优化能力。
打包后的文件也会在后面给出下载链接地址。

## 三、使用配置文件来打包后随时修改视频内容

直接修改AVProWindowsMediaMovie.cs中的LoadMovie函数。

```

		bool allowNativeFormat = (_colourFormat != ColourFormat.RGBA32);

        //string filePath = GetFilePath();
        string filePath = ConfigFile.Cconfig.VediofullPathName;

        if (_moviePlayer.StartVideo(filePath, allowNativeFormat, _colourFormat == ColourFormat.YCbCr_HD, _allowAudio, _useAudioDelay, _useAudioMixer, _useDisplaySync, _ignoreFlips, _textureFilterMode, _textureWrapMode))
		{
			if (_allowAudio)
			{
				_moviePlayer.Volume = _volume;
				_moviePlayer.AudioBalance = _audioBalance;
			}
			_moviePlayer.Loop = _loop;
			if (autoPlay)
			{
				_moviePlayer.Play();
			}
		}
		else
```
其实所要修改的只是视频的加载路径，仅此而已。
只需更改：
 
```
//string filePath = GetFilePath();
        string filePath = ConfigFile.Cconfig.VediofullPathName;
```
这样就可以了。

配置文件的脚本：

```
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;

public class ConfigFile : MonoBehaviour {

    public static ConfigFile Instance { private set; get; }
    public static ClientConfigReference Cconfig;
    
    void Awake()
    {
        try
        {
            ReadConfigFile(Application.streamingAssetsPath + "/Configfile.xml");
        }
        catch (Exception ex)
        {
            Debug.LogError("Read XML Failed !  " + ex.Message);
        }
    }

    void ReadConfigFile(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            XmlSerializer xs = new XmlSerializer(typeof(ClientConfigReference));
            Cconfig = xs.Deserialize(fs) as ClientConfigReference;
            fs.Dispose();
            Debug.Log("Configfile :" + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Read ConfigFile Failed !  " + ex.Message);
        }
    }
}

// configure class
public class ClientConfigReference
{
    public string VediofullPathName;
}


```

这个只需要挂载在场景中即可，不需要任何操作。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/15.png)
图15


## 四、球内使用的shader

360SphereVideo中sphere中使用的材质的Shader.

```
Shader "Unlit/InsideSphere"
{
    Properties
    {
        // we have removed support for texture tiling/offset,
        // so make them not be displayed in material inspector
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 vertex : SV_POSITION; // clip space position
            };

			uniform float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);                
				o.uv.xy = TRANSFORM_TEX(float2(1-v.uv.x, v.uv.y), _MainTex);

                return o;
            }
            
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                // sample texture and return it
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
```

有兴趣的自行研究下。

## 五、打包后运行测试：
### 1. 先使用的星空：
资源可以在文章后面地址下载：
看看美丽星空：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/16.png)
图16

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/17.png)
图17

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/18.png)
图18

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/19.png)
图19

可以看到，这是单眼效果，因为使用大鹏头盔，使用复制屏的效果，分辨率不对造成的，但是在直接运行过中，不影响使用和体验。

### 2. 来看美女了
通过切换配置文件中

```
 <VediofullPathName>H:\Unity\UnitySay\360Video\360Demo\360VedioTest\Assets\StreamingAssets\猫耳娘SEASON 2.mp4</VediofullPathName>
```
来切换视频内容了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/20.png)
图20

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/21.png)
图21

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/22.png)
图22

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/360Vedio/Img/23.png)
图23

由于纹理清晰度不是很高，这个需要提高的。
还有就是由于使用旧版的OC，所以可能会崩溃。崩溃日志如下：
检重要的贴出来

```

========== OUTPUTING STACK TRACE ==================

  ERROR: SymGetSymFromAddr64, GetLastError: '试图访问无效的地址。

' (Address: 6CF02036)
6CF02036 (OculusPlugin) 
6CF03012 (OculusPlugin) OVR_SetViewport
6CF030D3 (OculusPlugin) UnitySetGraphicsDevice
01040016 (001) RectT<int>::Contains
01040058 (001) RectT<int>::Contains
0104054B (001) RectT<int>::Contains
100399D8 (mono) mono_lookup_pinvoke_call
100484DE (mono) mono_marshal_string_to_utf16
100CF9DD (mono) mono_inst_name
100EF02C (mono) mono_set_defaults
100EFE79 (mono) mono_set_defaults
100F03D4 (mono) mono_set_defaults
100F059D (mono) mono_set_defaults
1005D872 (mono) mono_runtime_invoke
00FC47FE (001) scripting_gchandle_get_target
01077709 (001) ScriptingArguments::AddString
0107777E (001) ScriptingArguments::AddString
010777DC (001) ScriptingArguments::AddString
00FB3462 (001) ReportScriptingObjectsTransfer::TransferTypeless
00FB3669 (001) ReportScriptingObjectsTransfer::TransferTypeless
00FB5988 (001) GetMonoBehaviourInConstructor
00FB3B7C (001) ReportScriptingObjectsTransfer::TransferTypeless
0109768B (001) RegisterAllowNameConversionInDerivedTypes
0109785D (001) RegisterAllowNameConversionInDerivedTypes
01050908 (001) CallbackArray3<std::basic_string<char,std::char_traits<char>,stl_allocator<char,59,16> > const &,AwakeFromLoadQueue &,enum LoadSceneOperation::LoadingMode>::Invoke
01050B0F (001) CallbackArray3<std::basic_string<char,std::char_traits<char>,stl_allocator<char,59,16> > const &,AwakeFromLoadQueue &,enum LoadSceneOperation::LoadingMode>::Invoke
....

========== END OF STACKTRACE ===========
```
就这样。

所以，还是刚才说的，你可以使用更高版本的OC的0.8.0版本的打包版本。

## 六、资源下载地址：

猫耳娘视频地址云盘下载：  https://pan.baidu.com/s/1qYtVhfe

星空视频云盘下载：https://pan.baidu.com/s/1c2rBNo8

更多可地址：http://vr.diyiapp.com/vrsp/

工程下载github地址：
https://github.com/cartzhang/ImgSayVRabc/tree/master/360Vedio/360Demo/360VedioTest 
工程下载后，需要把视频文件放在StreamingAssets下，然后使用配置文件Configfile.xml就可以使用了。

工程release地址：打包后，直接在StreamingAssets下添加新的视频文件，然后使用配置文件Configfile.xml就可以使用。就是这么简单，快捷。


解码器下载地址： 
http://pan.baidu.com/s/1o80fjXS 
http://pan.baidu.com/s/1mhVbk5U 

OC 0.4.4插件下载地址： 
http://pan.baidu.com/s/1gfA2TwR 

AVPro Windows Media2.9.0 下载地址： 
http://pan.baidu.com/s/1gfPvXyr 

360视频游戏打包完整地址： 
OC版本0.4.4云盘地址： 
http://pan.baidu.com/s/1jI8uDRC 

OC 版本 0.8.0 云盘地址： 
http://pan.baidu.com/s/1nv4QLuD


## 七、参考
 [1]. http://www.panduoduo.net/s/name/%E5%85%A8%E6%99%AF%E8%A7%86%E9%A2%91
 
 [2]. http://bernieroehl.com/360stereoinunity/
 
 [3]. http://download.videolan.org/pub/videolan/x264/binaries/win64/
 
 [4]. http://www.divx.com/zh-hans/software/download/start
 
 [5].http://vr.diyiapp.com/vrsp/