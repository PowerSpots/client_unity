using UnityEngine;
using System.Collections;


namespace RTS
{
    // 保存角色队伍相关数据
    public class TeamComponent : ComponentBase
    {
        public TeamEnum Team;
        public uint TeamPos;
    }
}