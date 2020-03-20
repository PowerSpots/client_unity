local SceneManager = UnityEngine.SceneManagement.SceneManager
local BattleServiceExport = CS.BattleServiceExport

system("BattleSystem", SystemScope.Battle)

function onInit(self)
    Console.info("BattleSystem onInit")
    navigateTo(BattleSystem)
    setNavigateHome(BattleSystem)
    self:onShow()
end

function test_api(self, param1)
    Console.info("BattleSystem test_api param1:" .. param1)
end

function onShow(self)
    Console.info("BattleSystem onShow")

    -- self.BattleRoot:setVisible(true)
    GalaxyBattle.instance:StartBattle()
end

function onHide(self)
end

function onButtonBattle(self)
    -- body
end

function onButtonReturnHall(self)
    -- 切换scene之前做battle service 清理
    BattleServiceExport.Release()
    GalaxyGame.instance:enter(SystemScope.Hall)
end