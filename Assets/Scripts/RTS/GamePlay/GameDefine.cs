using UnityEngine;
using System.Collections;

namespace RTS
{
    public enum CharacterEventType
    {
        Init,   //初始化
        Move,   //移动
        Attack, //攻击
        Damage, //受伤
        Recover,//恢复
        Die,    //死亡
        SKill,  //使用技能
        BeBreak,//被打断
        Resurgence, //复活
        EnterArea,  //进入某区域
        ExitArea,   //离开某区域
        Destroy,
    }

    public enum TeamEnum
    {
        TeamLeft = 0,
        TeamRight,
    }

    // 角色方向, 骨骼动画默认朝右
    public enum RoleDir
    {
        FaceLeft = 0,
        FaceRight = 1,
    }

    // 角色状态定义
    public enum RoleStatusEnum
    {
        Idle = 0,
        Appearance,
        Moving,
        NormalAtk,
        CastSkill,
        Death,
    }

    public class GameDefine
    {

    }
}