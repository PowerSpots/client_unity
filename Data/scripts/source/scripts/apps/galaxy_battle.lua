local SceneManager = UnityEngine.SceneManagement.SceneManager
local BattleServiceExport = CS.BattleServiceExport

service("GalaxyBattle")

function startup(self)
    Console.info("GalaxyBattle startup")
end

function StartBattle(self)
    Console.info("GalaxyBattle StartBattle")

    BattleServiceExport.Start()
end