using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTS
{
    public class RTSGamePlayManager : MonoBehaviour
    {
        public static RTSGamePlayManager Instance { get; set; }

        #region MonoBehaviour 生命期方法
        public void Awake()
        {
            Instance = this;
            Init();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        void Update()
        {
            OnGamePlayUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnGamePlayLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnGamePlayFixedUpdate?.Invoke();
        }

        void OnGUI()
        {
            OnGamePlayOnGUI?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnGamePlayOnDrawGizmos?.Invoke();
        }
        #endregion

        #region 程序生命周期事件派发
        //public static VoidCallback s_OnApplicationQuit = null;
        //public static BoolCallback s_OnApplicationPause = null;
        //public static BoolCallback s_OnApplicationFocus = null;
        public static VoidCallback OnGamePlayUpdate = null;
        public static VoidCallback OnGamePlayFixedUpdate = null;
        public static VoidCallback OnGamePlayOnGUI = null;
        public static VoidCallback OnGamePlayOnDrawGizmos = null;
        public static VoidCallback OnGamePlayLateUpdate = null;
        #endregion

        #region Custom 方法
        private void Init()
        {
            WorldManager.Init(1000);
            var world = WorldManager.CreateWorld<PveBattleWorld>();
            world.IsClient = true;
            world.IsStart = true;
            world.IsLocal = true;
        }
        #endregion
    }
}

public delegate void BoolCallback(bool status);
public delegate void VoidCallback();
