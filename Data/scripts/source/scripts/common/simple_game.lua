local Coroutine = Gankx.Coroutine
local SystemService = Gankx.SystemService
local Resources = UnityEngine.Resources
local PanelUnloadService = Gankx.UI.PanelUnloadService
local NetworkService = Gankx.NetworkService

local SwitchScopeAnalysisSystem = SwitchScopeAnalysisSystem

local UIImageExport = CS.UIImageExport
local GamePortalExport = CS.GamePortalExport
local ResourceServiceExport = CS.ResourceServiceExport
local ApplicationExport = CS.ApplicationExport
local SceneManager = UnityEngine.SceneManagement.SceneManager
local UIWindowExport = CS.UIWindowExport
local AtlasManagerExport = CS.AtlasManagerExport
local BundleSpriteManagerExport = CS.BundleSpriteManagerExport
local RawImageAssetTextureManagerExport = CS.RawImageAssetTextureManagerExport

service("SimpleGame")

local unloadUnusedAssetsWeight = 2

local function unloadUnusedAssets(async)
    while SceneManager.hasUnloadOp() do
        Coroutine.yieldNull()
    end

    if nil ~= PanelUnloadService.instance then
        PanelUnloadService.instance:clearAll()
    end

    UIImageExport.UnloadUnusedAssets()
    UIWindowExport.UnloadUnusedSlotControls()
    AtlasManagerExport.UnloadAllAssets()
    BundleSpriteManagerExport.UnloadUnusedAssets()
    RawImageAssetTextureManagerExport.UnloadUnusedAssets()

    collectgarbage("collect")
    ResourceServiceExport.TryReleaseUnuseBundle()

    local asyncOperation = Resources.unloadUnusedAssets()
    if async ~= nil then
        async:yield(asyncOperation, 0)
    else
        -- sync modec
    end

    ResourceServiceExport.Collect()
end

function startup(self)
    print("SimpleGame startup")
    Console.info("SimpleGame startup")
    SystemService.instance.unloadUnusedAssets = unloadUnusedAssets
    SystemService.instance.unloadUnusedAssetsWeight = unloadUnusedAssetsWeight

    SystemService.instance:switchScope(SystemScope.Tutorial)
    self._curScope = SystemScope.Tutorial

    self:listenEvent("OnQuitApp", onQuitApp)
end

function onQuitApp(self)
    if MessageBoxSystem.instance:getCount() > 0 then
        return
    end

    MessageBoxSystem.instance:show("确定要退出游戏吗？", quitApp, self)
end

function quitApp(self, isOk)
    if isOk == true then
        GamePortalExport.QuitApp()
    end
end

local function loadingBegin()
    LoadingSystem.instance:start()
end

local function loadingIncreaseTo(to)
    LoadingSystem.instance:increaseTo(to)
end

local function loadingEnd()
    LoadingSystem.instance:stop()
end

local function enterRoutine(self, scope, oldScope)
    self.isEntering = true
    local s = os.clock()

    NetworkService.instance:startRecvCache()
    
    local needWait = loadingBegin(scope, oldScope) or 0
    if needWait > 0 then
        Coroutine.yieldWaitForSeconds(needWait)
    end

    ApplicationExport.SetBackgroundLoadingPriority(4)

    SwitchScopeAnalysisSystem.instance:beginAccumulate("start loading")

    SwitchScopeAnalysisSystem.instance:beginAccumulate("OnApplicationEnterStart")
    fireEvent("OnApplicationEnterStart", scope, oldScope)
    SwitchScopeAnalysisSystem.instance:endAccumulate("OnApplicationEnterStart")

    SwitchScopeAnalysisSystem.instance:beginAccumulate("yieldNull")
    Coroutine.yieldNull()
    SwitchScopeAnalysisSystem.instance:endAccumulate("yieldNull")

    SwitchScopeAnalysisSystem.instance:beginAccumulate("OnScopeSwitchingStart")
    self._switchingScope = true
    fireEvent("OnScopeSwitchingStart", scope, oldScope)
    SwitchScopeAnalysisSystem.instance:endAccumulate("OnScopeSwitchingStart")

    SwitchScopeAnalysisSystem.instance:beginAccumulate("switchScopeAsync")
    local switchScopeRequest = SystemService.instance:switchScopeAsync(scope)
    while not switchScopeRequest:getIsDone() do
        local progress = 0
        if switchScopeRequest:getTotalWeight() > 0 then
            progress = switchScopeRequest:getProgress()
        end

        loadingIncreaseTo(0.9 * progress)

        Coroutine.yieldNull()
    end
    SwitchScopeAnalysisSystem.instance:endAccumulate("switchScopeAsync")

    SwitchScopeAnalysisSystem.instance:beginAccumulate("OnScopeSwitchingStop")
    self._switchingScope = false
    fireEvent("OnScopeSwitchingStop", scope, oldScope)
    SwitchScopeAnalysisSystem.instance:endAccumulate("OnScopeSwitchingStop")

    loadingIncreaseTo(0.9)
    SwitchScopeAnalysisSystem.instance:beginAccumulate("yieldNull")
    Coroutine.yieldNull()
    SwitchScopeAnalysisSystem.instance:endAccumulate("yieldNull")

    NetworkService.instance:handleRecvCachesUntilReenter()

    SwitchScopeAnalysisSystem.instance:beginAccumulate("yieldNull")
    Coroutine.yieldNull()
    SwitchScopeAnalysisSystem.instance:endAccumulate("yieldNull")

    loadingIncreaseTo(0.92)

    SwitchScopeAnalysisSystem.instance:beginAccumulate("OnApplicationEnterWillStop")
    fireEvent("OnApplicationEnterWillStop", scope, oldScope)
    SwitchScopeAnalysisSystem.instance:endAccumulate("OnApplicationEnterWillStop")

    self._curScope = scope
    self.isEntering = false

    SwitchScopeAnalysisSystem.instance:beginAccumulate("yieldNull")
    Coroutine.yieldNull()
    SwitchScopeAnalysisSystem.instance:endAccumulate("yieldNull")
    loadingIncreaseTo(0.95)

    SwitchScopeAnalysisSystem.instance:beginAccumulate("OnApplicationEnterStop")
    fireEvent("OnApplicationEnterStop", scope, oldScope)
    SwitchScopeAnalysisSystem.instance:endAccumulate("OnApplicationEnterStop")

    loadingIncreaseTo(0.98)
    fireEvent("OnSceneControlLoaded", scope, oldScope)
    loadingIncreaseTo(1)

    SwitchScopeAnalysisSystem.instance:beginAccumulate("yieldNull")
    Coroutine.yieldNull()
    SwitchScopeAnalysisSystem.instance:endAccumulate("yieldNull")
    SwitchScopeAnalysisSystem.instance:beginAccumulate("yieldNull")
    Coroutine.yieldNull()
    SwitchScopeAnalysisSystem.instance:endAccumulate("yieldNull")

    ApplicationExport.SetBackgroundLoadingPriority(2)

    loadingEnd(scope, oldScope)

    SwitchScopeAnalysisSystem.instance:endAccumulate("start loading")

    NetworkService.instance:stopRecvCache()
end

function _handleCaches(self, scope, oldScope)
    if NetworkService.instance:handleRecvCaches() then
        self:_ForceStopEnterBattle(scope, oldScope)

        NetworkService.instance:handleFirstRecvCache()
        return true
    end

    return false
end

function enter(self, scope)
    if self._switchingScope then
        self:error("enter", string.substitute("cannot enter scope {1} while switching scope {2}", scope, self._enteringScope))
    end

    local oldScope = SystemService.instance:getCurrentScope()
    self._LastScope = oldScope
    self._enteringScope = scope
    self:startCoroutine(enterRoutine, scope, oldScope)
end

function forceStopEnter(self)
    self:stopAllCoroutines()
end

function getLastScope(self)
    return self._LastScope
end

function getCurScope(self)
    return self._curScope
end
