using System;

namespace RTS
{
    public class PveBattleWorld : GameWorldBase<CommandComponent>
    {
        public override Type[] GameSystems()
        {
            return new Type[]
            {
                //表现层
                typeof(BattleRoleSystem),

                //逻辑层
                typeof(MoveSystem),
                typeof(RoleStatusSystem),

                // init
                typeof(BattleInitSystem),
            };
        }

        public override Type[] GetRecordTypes()
        {
            return new Type[]
            {
            };
        }

        public override Type[] GetRecordSingletonTypes()
        {
            return new Type[]
            {
                //typeof(TestSingleComponent),
            };
        }
    }
}