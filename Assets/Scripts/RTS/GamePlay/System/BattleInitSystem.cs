using UnityEngine;
using System.Collections;
using Gankx;
using DragonBones;

namespace RTS
{
    public class BattleInitSystem : InitSystemBase
    {
        public override void OnGameStart()
        {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("BattleLayer");

            foreach (var go in playerObjects)
            {
                Debug.Log("OnGameStart BattleLayer Go Name:" + go.name);
                TeamEnum team = (go.name.CompareTo("TeamR_") >= 0) ? TeamEnum.TeamRight : TeamEnum.TeamLeft;
                uint posId = 0;
                try
                {
                    posId = uint.Parse(go.name.Substring(6, 1));
                    Debug.Assert(posId >= 0 && posId <= 5);
                }
                catch
                {
                    Debug.LogError("Error Team Object Name: " + go.name);
                }
                //CreateRoleEntity(team, posId, go);
                RoleEntityManager.Instance.CreateRoleEntity(m_world, team, posId, go);
            }
        }

        public override void OnEntityCreate(EntityBase entity)
        {
            Debug.Log("BattleInitSystem OnEntityCreate entity:" + entity.ID + "|" + entity.name);
            RoleEntityManager.Instance.PlayIdle(entity);
        }

        public override void OnEntityCompAdd(EntityBase entity, int compIndex, ComponentBase component)
        {
            Debug.Log("BattleInitSystem OnEntityCompAdd entity:" + entity.ID + "|" + entity.name);
        }

        public override void OnEntityCompRemove(EntityBase entity, int compIndex, ComponentBase component)
        {
            Debug.Log("BattleInitSystem OnEntityCompRemove entity:" + entity.ID + "|" + entity.name);
            RoleEntityManager.Instance.DestroyDBComp(entity);
        }
    }
}