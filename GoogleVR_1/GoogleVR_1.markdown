
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/1.png)

本篇为基础篇，适合用纯正小白，还请各位大神绕行。

VR先从外形看起来最简单的google VR的Cardboard说起。
google发布Cartboar的目的，用简单好玩廉价的方式来体验虚拟现实。
其用来看全景视频的居多，当然也有其他不一样的用途，能在手机上玩的，肯定都可以玩出不同的花样来。

先说下，本图说的所有教程都是基于Window下，
图片引用字github,地址：https://github.com/cartzhang/UnitySay/tree/master/GoogleVR_1/Img

若是看不到图片，可自行下载。

首先，看看我们需要的资源：

## 一、所需资源

### 1.Unity

  当前是使用的Unity较新的版本5.4.0f3,
  
  Untiy 各个版本地址：
  
  https://unity3d.com/cn/get-unity/download/archive?_ga=1.41972967.311279473.1476870291
  ![image](https://github.com/cartzhang/ImgSayVRabc/tree/master/GoogleVR_1/Img/2.png)
  
  1> 其中5.4.0f3其官方的下载地址：
  
  http://download.unity3d.com/download_unity/a6d8d714de6f/Windows64EditorInstaller/UnitySetup64-5.4.0f3.exe?_ga=1.147305973.311279473.1476870291
  
  下载后在文件中找到如图：
  
   ![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/3.png)
  
  2> 当然你也可以通过下载器来下载：
  
  https://unity3d.com/cn/get-unity/download?thank-you=update&download_nid=41345&os=Win
  
  要说的是，若为Unity的较早版本，需要自己来下载其中的
  UnitySetup-Windows-Support-for-Editor-5.*.*.*来实现对Window平台的支持。
  
  当然你的根据你的电脑平台来选择，Android，IOS，Windows等等。
  下载器下载完毕是这个样子：
  
  ![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/4.png)
  
  然后安装，一路点击next，就会来到这个界面，请跟需要选择，这里就选图上这些就够用了。
  ![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/5.png)
  
  后面就是选择安装和下载路径的问题：
  ![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/6.png)
  
  下载完毕后会自动安装到你刚才所选的文件夹内，若不选择保存目录，则刚才下载的安装文件就会在安装完毕后自动删除。
  
  3> 剩下就是安装了，一路Next。
  
### 2.GoogleVR 插件

Google VR对Unity的支持是通过插件形式，其代码是开源的，github地址为：
  https://github.com/googlevr/gvr-unity-sdk/tree/v1.0.1

其实我们暂时所关心的只是插件：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/7.png)

点击打开，看到下载，直接下载就搞定了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/8.png)


## 二、 实例
### 1.建立一个项目工程
使用Unity正常的打开一个项目，我这里使用的是空项目，然后从官方导入的Unity酱。
若已经下载可以在这里找到，直接导入：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/9.png)

然后：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/10.png)

结果如下图：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/11.png)

Unity 酱就出现在场景中了。


### 2.导入googleVR插件
导入下载的插件：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/12.png)

打开
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/13.png)

也可以直接把插件拖拽到project中，也可以实现导入。
点击导入：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/14.png)

中间会出现这样的提示：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/15.png)

这个不重要，主要是对版本的兼容性问题。
若是gogleVR使用0.9版本在unity5.4.0f3就不会出现这个问题。
这里选那个都不关紧要。但是对于以后版本最好还是选Import package.

 ### 3.设置相机

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/16.png)

需要设置相机位置。先建立一个空节点，然后把Main Camera拖拽到Cam下面。
如图：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/17.png)


![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/18.png)

注意：要把Main Camera下的Camera Controller脚本勾选掉，因为它会控制相机，而google VR 也需控制相机的。这是有冲突的。

最终项目如下图：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/19.png)

### 4. 结果：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/20.png)


旁边有各个不同按钮，对应不同的动画动作，你可以测试下。
可爱的Unity酱就出来了。

## 三、打包

安装Unity 的Andriod包,

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/21.png)

其中设置的时候考虑到需要横屏来玩游戏，需要设置default Orientation
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/22.png)


若没有安装的一些环境会点击安装，自动安装更新

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/23.png)

漫长的等待，等等....
若你安装过Andriod的相关软件，就会直接打包。
关于unity的Andriod打包使用：
参考：
http://blog.csdn.net/techtiger/article/details/21534893

需要安装安卓SDK和JDK，相关内容参考连接。

说下设置：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/24.png)

为了看到这个包，我花费了两天，网速有限速，实在是困难。
没有压缩图片，25M左右。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/25.png)

## 四、手机截图

**说明下，手机使用的是按照Andriod 5.0版本。**

自己倒持到手机上。由于代码里写的有控制，可以直接看的。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/s1.png)
导入到手机中，来看看我们千辛万苦的弄好的Unity chan。


![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/s2.png)

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/s4.png)

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/s5.png)

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/GoogleVR_1/Img/s6.png)

这个是什么鬼?冲到脸上来了，吓到我了，难度被视锥给裁剪了？

反正，终于完成了。

## 五、源码下载地址

源码地址:https://github.com/cartzhang/UnitySay/tree/master/GoogleVR_1

游戏APK包地址：

https://github.com/cartzhang/UnitySay/blob/master/GoogleVR_1/Gvr01/mRelease/unitychan.apk

## 六、 参考

[1].https://developers.google.com/vr/unity/

[2].https://developers.google.com/vr/unity/release-notes

[3].https://vr.google.com/cardboard/

[4].https://developers.google.com/vr/

[5].http://blog.csdn.net/techtiger/article/details/21534893

[6].http://bbs.9ria.com/thread-219753-1-1.html

[7].http://www.cnblogs.com/nsky/p/4594371.html

[8].http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html


--------------------THE-----END------------------

若有问题，请随时联系！！
非常感谢！！
