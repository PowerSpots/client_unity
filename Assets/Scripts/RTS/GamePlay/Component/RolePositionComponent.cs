using UnityEngine;
using System.Collections;


namespace RTS
{
    public class RolePositionComponent : ComponentBase
    {
        public GameObject ParentGo;
        public Vector2 SpawnPos; // 出生点，第一个事情是朝StandPos移动到位
        public Vector3 StandPos; // 开场固定站位
    }
}