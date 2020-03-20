using System.Collections;
using System.Collections.Generic;
using Gankx;
using UnityEngine.SceneManagement;

public static class BattleServiceExport
{
    //public static void InitBattlePve()
    //{
    //    BattleService.instance.Init();
    //}

    //public static void Clean()
    //{
    //    BattleService.instance.DestroyInstance();
    //}

    // 启动战斗场景 并enable
    public static void Start()
    {
        SceneManagerExport.LoadSceneAsyncByName("battle_pve", (int)LoadSceneMode.Single);
    }

    public static void Release()
    {
    //    BattleService.instance.DestroyInstance();
        SceneManagerExport.LoadSceneAsyncByName("update", (int)LoadSceneMode.Single);
    }
}
