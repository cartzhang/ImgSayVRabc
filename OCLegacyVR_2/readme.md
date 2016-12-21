
为什么说是旧版本？
很明显，就是Unity对VR的支持，下手还是很快的。但是之前没有直接嵌入到Unity编辑器里面，Oculus也积极与大厂合作，分别出来Unity和Unreal4的官方指导和插件。所以这里所说的旧版本，其实也没有太旧，依旧为Unity5的版本。

今天就谈怎么利用插件在在Unity中建立一个VR的过程。
今天主要谈的是DK2的使用插件版本。顺带说下，更早的Dk2使用，至于DK1的设置和各种匹配，就不说了，估计没人继续关注了。

本文依旧为纯小白教程，烦请大神绕行。

## 一、 OCVR的Dk2头盔的的Unity插件

使用Unity5.3.2f1版本 OC使用0.1.3.0beta版本，
他们是可以正使用的。
这个其实也不是官方推荐的最佳匹配。但是可以用。

而使用5.1.1f1版本，同一个项目工程打开后运行就会有报错。

还是要根据官方推荐来对应版本。
之前的图片：
![tst](http://img.blog.csdn.net/20161115183835502)

这其中原因在于Unity版本和OC直接的嵌入，及其对应的OC版本有差异， OC大版本之间差异还是比较明显的。

可以看到在使用5.3.2f1版本时候，在Unity中的画面是不可以在Game界面上分屏的。这与之前更早的版本，处理起来的方法就不一样。

DK2 0.8.0.0版本runtime对应插件：
下载地址：

https://developer3.oculus.com/downloads/game-engines/0.1.3.0-beta/Oculus_Utilities_for_Unity_5/

可以看到相关修改和特性。
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/1.png)
图1
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/2.png)
图2
下载完毕后，等待导入Unity使用。

这里再次强调一下Unity 与OC直接对应关系！！
先给出还在之前的对于的Oculus官方连接：

https://developer3.oculus.com/documentation/game-engines/latest/concepts/unity-sdk-version-compatibility/

为什么呢？接下来我会尝试一下，看看会有什么情况发生。
而早期的版本，主要针对Dk1或DK2，现在从某版本开始，不在提供Dk2的支持版本。所以，若你是Dk2用户，需要使用对应的unity版本和OC版本。

![image](H:\Unity\UnitySay\OCLegacyVR_2Img\3.png)
图3
## 二、制作一个使用插件的样例
我使用的版本并不是Oculus官方所说的5.2.p2版本，而是5.3.2f1版本。
OC插件为为0.1.3.0-beta版本。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/4.png)
图4
导入插件：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/5.png)
图5
导入后可以看到，主要的场景和预制体

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/6.png)
图6
然后设置一下，不在使用Unity内置的Oculus的SDK了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/8.1.png)
图8.1
其实这样就可以直接运行搞定了。
因为我已经插好了Dk2或大朋的头盔了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/6.1.png)
图6.1
但是为了看到漂亮场景，我添加了一个卡通农场的场景。
导入之后，发现代码有报错。
简单粗暴的删除掉就好。咱们不要它的代码运行，只需要它的场景。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/7.png)
图7
删除上图所框出的脚本。

然后把OC插件中的预制体OVRCameraRig，拖到场景中一个合适的位置。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/8.png)
图8

这时候运行就可以了：
可以在头盔中看到场景了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/9.png)
图9
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/10.png)
图10
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/11.png)
图11

但是发现没有，这里在Unity中game窗口中并没有分屏。
这个问题也是之前有网友给留言说，为啥没有分屏呢。
这个就是Unity内嵌OC SDK做的处理和优化了， 因为它现在在编辑器下，并不是在直接需要渲染双目的，那样在屏幕上渲染毫无意，这样反而可以减少GPU渲染的浪费。

那什么情况下，是可以分屏的呢？
这个专门做了个测试。在更早起版本，可以的，也就是Dk1下的Unity插件，比如下面写的大家常用的0.4.4。

在此之前，我们来看看使用5.1.1f1版本，能否正常的开启VR模式。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/11.5.1.1.png)
图11.5.1.1

运行不但报错，而且就算没有勾选Player Settins中的Virtual Reality Supported的选项，在运行中，会自动给勾选上。
也就是说，不可以正常运行。
究其原因还是Unity版本内嵌的Oculus的SDK版本与当前的0.1.3.0beta版本不兼容。

## 三、Dk2分屏版本

我使用的通过的资源问题，但是OC使用更古老的插件ovr_unity_0.4.4_lib。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/12.png)
图12
可能比较早的玩家使用过，这些东西。
通过在导入农场场景的过程中，需要点击I made a Backup,Go Ahead!

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/13.png)
图13
同样的不需要使用Unity的内置的VR模式。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/14.png)
图14
把原来的相机参数复制后，粘贴到OVRCameraRig对象上。如下图

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/15.png)
图15
然后运行就可以看到结果了：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/16.png)
图16
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/17.png)
图17

这里只是用来说明一下问题，也就是之前的插件是直接在屏幕上看到分屏效果的。
这个问题之前有网友问过，为啥旧版本没有分屏呢。那说不够旧而已。
更多的人应该没有在研究过时的DK2的需求了，不过我还是安装runtime来测试了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/19.png)

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/19.1.png)

有人可能会发现，这里使用的是deepoon大朋的头盔。
对了，大朋头盔在大部分情况下都是可以替代DK2的。

**但是由于屏幕为竖屏，需要把屏幕进行反转，然后才可以用。
并且在Unity中使用，就0.4.4 runtime，测试了Unity5.1.1f1和5.3.0f4版本都是可以正常使用的。
但是若在编辑器模式下，都必须把game窗口手动的推拽到头盔的屏幕上才可以。**

## 四、版本差异

其实比较没有太大的意义。还是来请看下吧。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/18.1.png)
图18.1
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/18.2.png)
图18.2

很明显OVRCameraRig这个对象就是最明显区别。

这里就不具体展看了，有需要的先看看吧。

## 五、大鹏头盔

为啥要单独说下大鹏呢，因为大鹏有兼容模式，兼容DK1和DK2，还有他们自己的大鹏模式。最早想单独写一篇大朋头盔的这个使用方法，包括安装、使用、切换以及它的优缺点。但是，现在不打算这么做了，当然若有人需要看，还是可以根据时间考虑来写的。

为了测试，使用了大朋头盔，发现新版的大朋头盔与0.4.4是兼容，也就是可以识别到头盔，但是在播放Demo的过程中，不是左右分屏，而是上线分屏了，这说明你的屏幕忘了进行反转，在调节。

但是这并不说明Deepoon不行，而是大朋具有很好的兼容性，有直接的DK1模式的。若发现运行不行，可能是需要切换模式，从DK1切换到DK2，因为他们出厂的模式是DK1模式，这个其实很不方面。

我这里并没有测试，把它调节为DK1模式。因为DK1这个太老了。
但是再之前的其他项目中，DK2模式的切换是没有问题的。这点请各位放心。

说一下的是：大朋的头盔是经过更新换代的，我本想比较一下新旧版本头盔的差别，发现官方网址上只有一个大朋头盔的参数。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/OCLegacyVR_2/Img/20.png)

肯定是升级换代了。但是官方没有任何参数和版本直接的差异说明。估计也是时光一去不回头，往事只能回味。

## 六、资源下载地址：

图片下载地址：
https://github.com/cartzhang/ImgSayVRabc/tree/master/OCLegacyVR_2/Img

Unity OC插件下载地址:

https://github.com/cartzhang/ImgSayVRabc/tree/master/OCLegacyVR_2/software

Unity资源包卡通农场下载地址：

https://github.com/cartzhang/ImgSayVRabc/blob/master/Cartoon%20Town%20and%20Farm.unitypackage

项目可以下载地址：
https://github.com/cartzhang/ImgSayVRabc/tree/master/OCLegacyVR_2/DK2PluginVR_0.1.3.0

本文所有可下载问题：
https://github.com/cartzhang/ImgSayVRabc/tree/master/OCLegacyVR_2

## 七、参考

[1] https://developer3.oculus.com/doc/0.1.0.0-unity/

[2]https://developer3.oculus.com/downloads/game-engines/0.1.3.0-beta/Oculus_Utilities_for_Unity_5/

[3] http://www.deepoon.com/dapengtoukui.html

