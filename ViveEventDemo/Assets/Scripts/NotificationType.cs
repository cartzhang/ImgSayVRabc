namespace SLQJ
{
    //消息常量
    public enum NotificationType
    {  
        Gun_Fire,           // 开枪
        Gun_KeyUp,          // 抬起
        Throw_Bomb,         // 扔雷
        Gathering_Stength,  // 蓄力
        Controller_HandState,  // 手柄在左节点还是右节点。
        Controller_Shake,   // 手柄震动消息
        Switch_Controller,  // 左右手不匹配，需要交换手柄。
    } 
}

