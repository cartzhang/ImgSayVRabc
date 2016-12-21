
《图说VR入门》——VR大朋的枪

之前在VR的射击游戏中，使用了大鹏的枪，其实也就是他们的陀螺仪。现在不用了，我们自己开发了新的枪。但是还是要简单的介绍一下，使用的方法和使用过程中的一些问题和可能的解决方法。
大鹏的枪做的还是可以用的。我这里用的是有线的版本。算是给自己做的一个记录了。

## 一、所需资源

### 1. 在Software下的DeePoonUnityPC0.2.8b_forUnity5.0，这个是deepoon为配合枪给出的Unity插件。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/1.png)
图1

#### 2. 使用了Unity5.3.0f4版本

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/0.png)
图0
### 3. 硬件接入

这个并没有驱动，是一个免驱的，插到USB后，如下图：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/2.png)
图2
但是虽然有黄色的叹号，但是这个不影响正常使用。

## 二、Deepoon gun的样例

首先，导入大鹏的插件。

然后，自己制作了一个场景，自己也作了一个DeeponGun的预制体。

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/3.png)
图3
保存的场景如图：

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/4.png)
图4

其次，给枪的预制体，添加DeepoonSensor.cs代码组件.

随后，跟枪添加了一个校正的目标：
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/5.png)
图5
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/5.1.png)
图5.1

运行结果:

![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/6.png)
图6
![image](https://github.com/cartzhang/ImgSayVRabc/blob/master/DeeponGun_1/Img/7.png)
图7
## 三、DeepoonSensor的代码

### 1. 大鹏的枪有几种不同的类型：

```
public enum PERIPHERAL_TYPE
	{
		Custom ,
		XRoverTest ,
		XRover1 ,
		DeePoonE2 ,
	};
```
其实主要不同的在于枪的初始化方向问题，然后可以在使用中使用不同的按键来进行校正。
如下代码在Update中进行实现。
```
if ( Input.GetKeyDown (KeyCode.T) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].x += -90;
		}
		if ( Input.GetKeyDown (KeyCode.Y) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].x += 90;
		}
		if ( Input.GetKeyDown (KeyCode.U) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].y += -90;
		}
		if ( Input.GetKeyDown (KeyCode.I) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].y += 90;
		}
		if ( Input.GetKeyDown (KeyCode.O) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].z += -90;
		}
		if ( Input.GetKeyDown (KeyCode.P) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].z += 90;
		}
```


### 2. 枪的初始化
枪初始化，是直接调用大鹏的DLL底层接口

```
void Start ()
	{
		imp = new DeepoonSensorImp();
		imp.Init( index , DEEPOON_PRODUCT_NAMES[(int)product] );
	}
```

### 3. 更新旋转


### 4. 对齐位置
枪由于各种原因，比如磁偏，造成的方向不对的情况，这时候就需要进行校正。

```
Quaternion Alignment( Quaternion cur_rotation , Quaternion object_to_align )
	{
		Vector3 ecur = cur_rotation.eulerAngles;
		Vector3 eobj = object_to_align.eulerAngles;
		return Quaternion.AngleAxis( eobj.y - ecur.y , new Vector3( 0 , 1 , 0 ) );
	}

```
校正直接对Y轴进行旋转操作，由校正对象欧拉角度减去当前角度，实现校正。

### 5. 退出

```
void OnApplicationQuit()
	{
		imp.Uninit();
	}
```
这个就是反注册DLL。

### 6. 更多接口调用
更多的接口和使用，在DeepoonImp.cs和DeepoonSensorImp.cs文件中，可以看到DLL的导入函数。
这里就不过多的分析和研究。

## 四、可能的问题

使用大鹏头盔有一段时间，发现的问题在于磁场造成的头盔或陀螺仪的漂移问题，在旧的版本上一旦出现，不重新启动游戏肯定是不能自动或通过代码来校正过来。但是至于现在大鹏最新的头盔和陀螺仪有没有做更新算法的陀螺仪磁偏校正算法，这个现在不太清楚。
但是意思不是说，之前它没有磁偏校正算法，官方说也是有的，就是还有这样的问题。因为同样都是陀螺仪，是一个类型的产品。所以，大鹏头盔之前有的问题，这个枪上还有，且一模一样。

但是作为经济实惠的选择，还是比较合适的。（大鹏官方看到，请找我充值!）

虽然还是不那么难，主要讲下应用和使用方面的问题。若是有时候枪模型和旋转方向与预期不一样，很可能就是模型的初始化方向不对，旋转正负90度就可能解决问题了。

## 五、资源地址

所有图片下载地址：
https://github.com/cartzhang/ImgSayVRabc/tree/master/DeeponGun_1/Img

大鹏枪的插件下载地址：
https://github.com/cartzhang/ImgSayVRabc/tree/master/DeeponGun_1/software

工程下载地址:
https://github.com/cartzhang/ImgSayVRabc/tree/master/DeeponGun_1/DeepoonGunTest/GunTest


---------------------THE-----END----------------------

如有问题，请留言！！
非常感谢！