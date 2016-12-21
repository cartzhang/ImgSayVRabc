本来想先介绍国产的deepoon头盔的VR入门。

可能会有人疑惑，这TM不都一样么?
对啊，正是有一样，才有不一样，也就是有一样的地方，也有不同之处。

大部分都是可通用Oculus，那就先说Dk2，然后有空在介绍与Dk2不同的地方。

本篇图说目标，OC安装及其与Unity版本直接的搭配，实现一个简单的VR场景。
图片编号依旧与github上保持一致，有需要的可以下载。

下载地址：

当前使用Unity版本为5.3.2f1,Oculus的SDK版本为0.8.0.

Unity安装可以参考：
http://blog.csdn.net/cartzhang/article/details/52959035

我的unity版本：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/0.png)

图0

## 一、OC安装

1.若使用大朋头盔，一开始插上头盔，在没有安装OC驱动的情况下

如图1
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/1.png)

这意思是说，其实大朋就是在OC的基础上做的一些封装和添加了自己的功能，比如说它的模式切换，这个是最突出的了，有需要了解详情的，可以随后说，这里就不作为重点说明了。

2. OC版本

  安装的是0.8.0版本的runtime:
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/2.png) 

图2

然后就是一步步的next既可。
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/3.png)

图3

安装完毕后，需要重启电脑。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/4.png)

图4


3. 重启后
插上大朋头盔或DK2头盔，可以看到是否正常。
若出现下图的样子，说明有东西没有安装正确。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/5.png)

图5


4. 可能的问题和需要下载的东西

对于OCulus，因为它是facebook的，所以是被墙的。若有问题，你要学会科学上网才可以。

runtime下载地址:
https://developer3.oculus.com/downloads/

直接下载链接：
https://static.oculus.com/sdk-downloads/0.8.0.0/Public/1445451746/oculus_runtime_sdk_0.8.0.0_win.exe

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/6.1.png)

图6.1

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/6.2.png)

图6.2

当然还有驱动，我使用的是英伟达的960，之前使用的是660，OCulus对驱动版本有不同的影响。

若出现下面：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/6.png)

图6

点击了show demo后没有画面。
那原因可能，需要换一下驱动版本，或若是win7，需要一个windows7的Windows6.1-KB2670838-x64补丁。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/7.1.png)

图7.1

补丁可以从官方下载，也可以从我的github上下载。

https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/software/Windows6.1-KB2670838-x64.msu

都更新完毕，一般应该就没有问题了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/7.png)

图7

画面可以显示，并且画面跟随头盔左右移动，就可以了。
当前头盔不管的DK2还是大朋就正常工作了。

## 二、Unity样例

unity本身有很好的样例：

下载地址：
https://unity3d.com/cn/learn/tutorials/topics/virtual-reality

打开Asset store

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/8.1.png)

图8.1

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/8.png)

图8

点击下载，然后可以导入看看，自己研究下。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/9.1.png)

图9.1

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/9.png)

图9

我这里就不过多的展开来看了。

## 三、我的样例

还是导入我可爱的unity酱。
老方法：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/10.1.png)

图10.1

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/10.png)

图10

打开场景

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/11.png)

图11

然后设置VR模式

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/12.png)

图12

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/13.png)

图13

最后，点击运行按钮，就可以在头盔中看到我们可以的unity酱了，也可以拽动相机，近距离接触可爱的unity酱了。

看我们unity飘逸的长发，是不是特别炫酷啊！

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/14.png)

图14

她的旋风腿：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/15.png)

图15

跑都跑的这么帅：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/16.png)

图16

各种姿势你自己都可以看看。

## 五、Unity与Oculus runtime版本匹配关系

由于unity在新的版本里面都集成了OC的SDK，所以很多功能都被封装好了。没有之前那么笨重，还需要导入各种插件，然后自己添加预制体，调配等等。

当然，这需要Unity版本与OC runtime之间的匹配。
Unity 5.0及其以上版本的匹配关系：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/17.png)

图17

Unity 5.0及其以下版本的匹配关系：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/18.png)

图18

若还有需要可以自己查看下面网址：

https://developer3.oculus.com/documentation/game-engines/latest/concepts/unity-sdk-version-compatibility/

若不可访问，请记得科(fan)学(qiang)上(la)网。

## 六、好东西要分享

1. 免费分享一个去掉Health warning的方法。
 
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/20.jpg)

图20

之前也分享过，这里在说下，很简单，就是下载一个注册表文件OCHiddenWarnnig.reg，然后运行下就可以了。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/Img/19.png)

图19

下载地址：

https://github.com/cartzhang/ImgSayVRabc/blob/master/DK2VR_1/software/OCHiddenWarnnig.reg



2. unity蛮牛的免费中文版

直接给出地址：

http://www.manew.com/thread-45174-1-1.html

这个是我直接汇总的，由四角钱翻译的。

单个章节地址：


教程《一》，VR开发介绍

http://www.manew.com/thread-45158-1-1.html?_dsign=090b0c7a

教程《二》，基础VR开发

http://www.manew.com/thread-45160-1-1.html

教程《三》，VR中的交互

http://www.manew.com/thread-45161-1-1.html?_dsign=311db2f8

教程《四》，VR的用户界面

http://www.manew.com/thread-45162-1-1.html?_dsign=e2a20b0d

教程《五》，VR中的运动

http://www.manew.com/thread-45163-1-1.html?_dsign=007dc5f3

教程《六》，部署发布VR项目

http://www.manew.com/thread-45164-1-1.html?_dsign=1b690372

教程《七》，优化Unity中的VR

http://www.manew.com/thread-45165-1-1.html?_dsign=1317e98f

教程《八》，VR开发阅读列表

http://www.manew.com/thread-45166-1-1.html?_dsign=933b234d



## 七、参考
[1] http://developer.deepoon.com/

[2] https://www3.oculus.com/en-us/rift/

[3] https://unity3d.com/cn/learn/tutorials/topics/virtual-reality

[4] https://github.com/cartzhang/ImgSayVRabc/tree/master/DK2VR_1

[5] http://www.manew.com/forum-136-1.html


----------------------THE------------------END---------------------

若有问题，请随时联系！！

非常感谢！！！
