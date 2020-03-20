using UnityEngine;
using System;
using System.Collections.Generic;

namespace RTS
{
    public class MoveSystem : SystemBase
    {
        public override Type[] GetFilter()
        {
            return new Type[]
            {
                typeof(MoveComponent),
            };
        }

        public override void Update(int deltaTime)
        {
            //Debug.Log("MoveSystem Update: " + Time.deltaTime.ToString() + "|" + deltaTime.ToString());
            List<EntityBase> list = GetEntityList();

            for (int i = 0; i < list.Count; i++)
            {
                UpdateMove(list[i], deltaTime);
            }
        }

        public override void FixedUpdate(int deltaTime)
        {
            //Debug.Log("MoveSystem FixedUpdate: " + Time.deltaTime.ToString() + "|" + deltaTime.ToString());
        }

        void UpdateMove(EntityBase entity, int deltaTime)
        {
            RoleStatusComponent rsc = entity.GetComp<RoleStatusComponent>();
//            SkillComponent sc = entity.GetComp<SkillComponent>();
            switch (rsc.RoleStatus)
            {
                case RoleStatusEnum.Appearance: // move to stand pos
                    {
                        MoveComponent mc = entity.GetComp<MoveComponent>();
                        var rpc = entity.GetComp<RolePositionComponent>();
                        var dbc = entity.GetComp<DBAnimComponent>();
                        // 计算更新SyncVector3 MoveComponent Pos
                        Vector3 currPos = mc.Pos.ToVector();
                        if (ShouldMovingStop(rpc.StandPos, currPos))
                        {
                            dbc.RootGo.transform.localPosition = rpc.StandPos;
                            m_world.eventSystem.DispatchEvent(EventDefine.Role_Moving_Stop, entity, null);
                        }
                        else
                        {
                            Vector3 MoveDir = (rpc.StandPos - currPos).normalized;
                            Vector3 MoveValue = mc.Velocity * MoveDir * (deltaTime / 1000.0f);


                            dbc.RootGo.transform.localPosition = currPos + MoveValue;

                        }
                        // set new position
                        mc.Pos = new SyncVector3().FromVector(dbc.RootGo.transform.localPosition);
                    }
                    break;
                default:
                    break;
            }
        }

        private bool ShouldMovingStop(Vector3 targetPos, Vector3 currPos)
        {
            if ((targetPos - currPos).magnitude <= 5)
            {
                return true;
            }
            return false;
        }
    }
}