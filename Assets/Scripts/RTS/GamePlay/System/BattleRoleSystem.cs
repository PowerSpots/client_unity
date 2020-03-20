using UnityEngine;
using System.Collections;
using Gankx;
using DragonBones;

namespace RTS
{
    // 管理一场战斗中的所有双发角色
    public class BattleRoleSystem : SystemBase
    {
        public override void Init()
        {
            AddEntityOptimizeCreaterLisnter();
            AddEntityOptimizeDestroyLisnter();
        }

        public override void Dispose()
        {
            RemoveEntityOptimizeCreaterLisnter();
            RemoveEntityOptimizeDestroyLisnter();
        }

        public override void OnGameStart()
        {
            // create all 12 roles
            //for (uint i = 0; i < 12; i++)
            //{
            //    CreateRoleEntity((i <= 5) ? TeamEnum.TeamLeft : TeamEnum.TeamRight, (i <= 5) ? i : (i - 6));
            //}

            //CreateRoleEntity(TeamEnum.TeamLeft, 0);
        }

        public override void OnEntityOptimizeCreate(EntityBase entity)
        {
            Debug.Log("BattleRoleSystem Entity Create");

        //    PlayIdle(entity);
        }

        public override void OnEntityOptimizeDestroy(EntityBase entity)
        {
            Debug.Log("BattleRoleSystem Entity Destroy");
        }
    }
}