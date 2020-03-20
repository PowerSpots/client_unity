using UnityEngine;
using System.Collections;

namespace RTS
{
    public class EventDefine
    {
        public const string Role_Moving_Stop = "role_moving_stop";
        public const string DBAnim_Create_Finish = "db_anim_create_finish";
        public static string GetEventKey(int entityID, CharacterEventType EventType)
        {
            return entityID + EventType.ToString();
        }
    }
}