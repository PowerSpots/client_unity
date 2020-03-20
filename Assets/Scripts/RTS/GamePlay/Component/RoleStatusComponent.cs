using UnityEngine;
using System.Collections;

namespace RTS
{
    public class RoleStatusComponent : SyncComponentBase
    {
        public RoleStatusEnum RoleStatus = RoleStatusEnum.Idle;
    }
}