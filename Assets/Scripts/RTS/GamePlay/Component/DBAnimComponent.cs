using UnityEngine;
using System.Collections;
using DragonBones;

namespace RTS
{
    public class DBAnimComponent : ComponentBase
    {
        public GameObject PerfabGo;
        public GameObject RootGo;   // 骨骼动画的父节点，也是游戏实际控制的GameObject
        public UnityArmatureComponent _armature_component;
    }
}