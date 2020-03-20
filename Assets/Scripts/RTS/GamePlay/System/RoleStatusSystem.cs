using UnityEngine;
using System;
using System.Collections.Generic;

namespace RTS
{
    /**
     * 管理角色对象状态
     */
    public class RoleStatusSystem : SystemBase
    {
        public override void Init()
        {
            AddEntityCreaterLisnter();

            m_world.eventSystem.AddListener(EventDefine.Role_Moving_Stop, onRoleMovingStop);
        }

        public override void Dispose()
        {
            RemoveEntityCreaterLisnter();
        }

        public RoleStatusEnum ChangeRoleStatus(EntityBase entity, RoleStatusEnum newStatus)
        {
            RoleStatusEnum roleStatus = entity.GetComp<RoleStatusComponent>().RoleStatus;
            if (roleStatus != newStatus)
            {
                Debug.Log(string.Format("Role Status Change From {0} ---> {1}", roleStatus.ToString(), newStatus.ToString()));
                entity.GetComp<RoleStatusComponent>().RoleStatus = newStatus;
            }

            return newStatus;
        }

        public override void OnEntityCreate(EntityBase entity)
        {
            Debug.Log(string.Format("RoleStatusSystem OnEntityCreate {0} - {1}", entity.ID, entity.name));

            ChangeRoleStatus(entity, RoleStatusEnum.Appearance);
        }

        void onRoleMovingStop(EntityBase entity, params object[] objs)
        {
            ChangeRoleStatus(entity, RoleStatusEnum.Idle);
        }
    }
}