using UnityEngine;
using System.Collections;
using Gankx;
using DragonBones;

namespace RTS
{
    public sealed class RoleEntityManager
    {
        private static RoleEntityManager _instance;
        public static RoleEntityManager Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = new RoleEntityManager();
                }
                return _instance;
            } 
        }

        public void CreateRoleEntity(WorldBase world, TeamEnum team, uint posId, GameObject parent)
        {
            string identifier = "Role_" + team.ToString() + "_" + posId;
            TeamComponent tc = new TeamComponent();
            tc.Team = team;
            tc.TeamPos = posId;

            DBAnimComponent dbc = SpawnDBComponent(team);


            dbc.PerfabGo.transform.parent = parent.transform;
            dbc.PerfabGo.transform.localPosition = Vector3.zero;
            dbc.RootGo = parent;
            // init stand position
            RolePositionComponent rpc = new RolePositionComponent();
            rpc.StandPos = dbc.RootGo.transform.localPosition;
            rpc.ParentGo = parent;
            rpc.SpawnPos = (team == TeamEnum.TeamLeft) ? rpc.StandPos + new Vector3(-1920 / 2.0f, 0) : rpc.StandPos + new Vector3(1920 / 2.0f, 0);
            dbc.RootGo.transform.localPosition = new Vector3(rpc.SpawnPos.x, rpc.SpawnPos.y);

            MoveComponent mc = new MoveComponent();
            mc.Pos = new SyncVector3().FromVector(dbc.RootGo.transform.localPosition);  // 初始化 MoveComponent Pos
            mc.Dir = (team == TeamEnum.TeamLeft) ? RoleDir.FaceRight : RoleDir.FaceLeft;

            RoleStatusComponent rsc = new RoleStatusComponent();

            SkillComponent sc = InitSkillComp(0);

            var role = world.CreateEntity(identifier, 
                tc, 
                dbc, 
                rpc, 
                mc,
                rsc,
                sc
                );

            //role.GetComp<RoleStatusComponent>().RoleStatus = RoleStatusEnum.Appearance;
        }

        private DBAnimComponent SpawnDBComponent(TeamEnum team)
        {
            DBAnimComponent dbc = new DBAnimComponent();
            bool flipX = team == TeamEnum.TeamRight;
            string asset_path = !flipX ? "hero_db/10396" : "hero_db/10496";
            var go = ResourceService.loadDBRes(asset_path);
            dbc.PerfabGo = Object.Instantiate(go);

            if (dbc.PerfabGo == null)
            {
                Debug.LogError("loadDBRes Failed:" + asset_path);
                return null;
            }
            dbc._armature_component = dbc.PerfabGo.GetComponentInChildren<UnityArmatureComponent>();
            if (dbc._armature_component == null)
            {
                Debug.LogError(string.Format("InitDragonBones Error, missing UnityArmatureComponent path: {0}", asset_path));
            }
            else
            {
                dbc._armature_component.armature.flipX = flipX;
                // Add Listeners
                dbc._armature_component.AddDBEventListener(EventObject.START, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.LOOP_COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.FADE_IN, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.FADE_IN_COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.FADE_OUT, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.FADE_OUT_COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.AddDBEventListener(EventObject.FRAME_EVENT, OnAnimationEventHandler);
            }

            return dbc;
        }

        private void OnAnimationEventHandler(string type, EventObject eventObject)
        {
            //Debug.Log(string.Format("animationName:{0},eventType:{1},eventName:{2}", eventObject.animationState.name, type, eventObject.name));
            string animName = eventObject.animationState.name;
            switch (type)
            {
                case EventObject.COMPLETE:
                    if (animName == "Death")
                    {
                        // do nothing
                    }
                    else if (animName != "Idle" && eventObject.armature != null && eventObject.armature.animation != null)
                    {
                        eventObject.armature.animation.Play("Idle", 0);
                        Debug.Log(string.Format("Play Anim COMPLETE, Change animation From [{0}] ---> [Idle]", animName));
                    }
                    break;
                default:
                    break;
            }
        }

        public void PlayIdle(EntityBase entity)
        {
            var dbc = entity.GetComp<DBAnimComponent>();
            if (dbc._armature_component != null)
            {
                dbc._armature_component.animation.Play("Idle", 0);
            }
        }

        public void PlayNormalAtk(EntityBase entity)
        {
            var dbc = entity.GetComp<DBAnimComponent>();
            if (dbc._armature_component != null)
            {
                dbc._armature_component.animation.Play("BasicAttack", 1);
            }
        }

        public void PlayHit(EntityBase entity)
        {
            var dbc = entity.GetComp<DBAnimComponent>();
            if (dbc._armature_component != null)
            {
                dbc._armature_component.animation.Play("Hit", 1);
            }
        }

        public void PlayDeath(EntityBase entity)
        {
            var dbc = entity.GetComp<DBAnimComponent>();
            if (dbc._armature_component != null)
            {
                dbc._armature_component.animation.Play("Death", 1);
            }
        }

        public void DestroyDBComp(EntityBase entity)
        {
            var dbc = entity.GetComp<DBAnimComponent>();
            if (dbc != null && dbc._armature_component != null)
            {
                dbc._armature_component.RemoveDBEventListener(EventObject.START, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.LOOP_COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.FADE_IN, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.FADE_IN_COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.FADE_OUT, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.FADE_OUT_COMPLETE, OnAnimationEventHandler);
                dbc._armature_component.RemoveDBEventListener(EventObject.FRAME_EVENT, OnAnimationEventHandler);

                if (dbc._armature_component.armature != null)
                {
                    dbc._armature_component.armature.Dispose();
                    dbc._armature_component = null;
                }
            }
        }

        private SkillComponent InitSkillComp(int roleId)
        {
            SkillComponent sc = new SkillComponent();
            // TODO 读表初始化

            return sc;
        }
    }
}
