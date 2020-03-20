local UIPanelServiceExport = CS.UIPanelServiceExport
local SystemService = Gankx.SystemService
local HallSystem = HallSystem

system("LoginPage", SystemScope.Login)

function onPanelLoad(self)
    local systemName = self.systemClass.__classname
    local panelName = self.systemClass.panelDefine.file
    UIPanelServiceExport.CachePanel(systemName, panelName)
end

function onInit(self)
    navigateTo(LoginPage)
    setNavigateHome(LoginPage)
end

function onBtnLoginClick( ... )
    Console.info("LoginPage onBtnLoginClick Click")
    GalaxyGame.instance:enter(SystemScope.Hall)
    -- HallSystem.instance:test_api("ppp")
    --navigateTo(HallSystem)
end
