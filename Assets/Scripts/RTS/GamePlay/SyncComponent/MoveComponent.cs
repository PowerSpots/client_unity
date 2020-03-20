using UnityEngine;
using System.Collections;

namespace RTS
{
    /**
     * 角色移动组件
     */
    public class MoveComponent : MomentComponentBase
    {
        public SyncVector3 Pos = new SyncVector3();

        public RoleDir Dir;

        public int Velocity = 500; //速度(每秒移动100像素)

        public override MomentComponentBase DeepCopy()
        {
            MoveComponent mc = new MoveComponent();

            mc.ID = ID;
            mc.Frame = Frame;

            mc.Pos = Pos.DeepCopy();
            mc.Dir = Dir;

            mc.Velocity = Velocity;

            return mc;
        }
    }

}