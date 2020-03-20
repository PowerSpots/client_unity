local NetworkTalker = Gankx.NetworkTalker
local PanelService = Gankx.UI.PanelService
local CompositedAsyncOperation = Gankx.CompositedAsyncOperation
local CameraExport = CS.CameraExport
local Coroutine = Gankx.Coroutine
local SceneManager = UnityEngine.SceneManagement.SceneManager
local LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode
local UIPanelServiceExport = CS.UIPanelServiceExport

module("Gankx", package.seeall)

class("SystemComponent", "Component")

function __load(self, systemClass, async)
    self.systemClass = systemClass

    local panelDefine = self.systemClass.panelDefine
    if nil ~= panelDefine then
        self.panel = PanelService.instance:createPanel(self.systemClass.__classname, panelDefine, self)

        local delayLoad = table.tryGetValue(panelDefine, "delayLoad", false)
        if delayLoad ~= true then
            if async ~= nil then
                self.panel:load(true, async)
            else
                self.panel:load(false)
            end
        end
    end

    CompositedAsyncOperation.beginGroup(async, table.tryGetValue(systemClass.tags, "loadWeight", 0))
    self:_receiveMessage("onLoad", async)
    CompositedAsyncOperation.endGroup(async)
end

function __unload(self)
    if nil ~= self.panel then
        PanelService.instance:destroyPanel(self.panel)
        self.panel = nil
    end

    self:destroyNetTalker()
end

function createNetTalker(self, scope)
    self:destroyNetTalker()
    self.netTalker = NetworkTalker:new(self, scope)
end

function destroyNetTalker(self)
    if self.netTalker ~= nil then
        self.netTalker:destroy()
        self.netTalker = nil
    end
end

local camHideCount = 0

local function InnerShowCamera(self)
    if table.tryGetValue(self.systemClass.tags, "Overlay", false) then
        if not table.tryGetValue(self.systemClass.tags, "OverlayManualControlCamera", false) then
            camHideCount = camHideCount + 1
            if camHideCount > 0 then
                CameraExport.SetMainCameraActive(false)
            end
        end
    end
end

local function InnerHideCamera(self)
    if table.tryGetValue(self.systemClass.tags, "Overlay", false) then
        if not table.tryGetValue(self.systemClass.tags, "OverlayManualControlCamera", false) then
            camHideCount = camHideCount - 1
            if camHideCount <= 0 then
                CameraExport.SetMainCameraActive(true)
            end
        end
    end
end

function _OnShow(self, ...)
    InnerShowCamera(self)

    self:_receiveMessage("onShow", ...)

    fireEvent("OnSystemPanelShow", self.systemClass)
end

function _OnHide(self, ...)
    InnerHideCamera(self)

    self:_receiveMessage("onHide", ...)

    fireEvent("OnSystemPanelHide", self.systemClass)
end

function _onRelease(self, ...)
    if self.panel then
        if table.tryGetValue(self.systemClass.panelDefine, "loadSync", true) and nil == self.systemClass.sceneDefine then
            if self.panel:isLoaded() then
                self.panel:hide()
            end
        else
            self:hidePanelAsync()
        end
    end

    self:_receiveMessage("onRelease", ...)
end

function _onReload(self, ...)
    if nil ~= self.OnReload then
        local _analysisName = "_onReload " .. self.systemClass.__classname
        SwitchScopeAnalysisSystem.instance:beginAccumulate(_analysisName)
        self:_receiveMessage("onReload", ...)
        SwitchScopeAnalysisSystem.instance:endAccumulate(_analysisName)
    end
end

function __loadAsync(self)
    if self.panel:isLoaded() then
        return
    end

    while self._inPanelAsync
    do
        Coroutine.yieldNullUnlimit()
    end

    self._inPanelAsync = true
    self.panel:load(true)
    while not self.panel:isLoaded() do
        Coroutine.yieldNullUnlimit()
    end

    self._inPanelAsync = false

    self:_doCallbacks()
end

function showPanel(self)
    if nil == self.systemClass.panelDefine then
        return
    end

    if self.panel.visible then
        return
    end

    if table.tryGetValue(self.systemClass.panelDefine, "loadSync", true) and nil == self.systemClass.sceneDefine then
        if not self.panel:isLoaded() then
            self:loadPanelSync()
        end
        self.panel:show()
    else
        self:startCoroutine(loadAndShowPanelAsync)
    end
end

function loadPanelSync(self)
    self.panel:load()
end

function loadPanelAsync(self)
    self:__loadAsync()
end

function loadAndShowPanelAsync(self)
    self:loadPanelAsync()
    self:showPanelAsync()
end

function loadSceneAsync(self, doLoadScene)
    if nil == self.systemClass.sceneDefine then
        return
    end

    local sceneName = self.systemClass.sceneDefine.name

    if false == doLoadScene then
        SceneManager.setActiveScene(sceneName)
        return
    end

    if nil == self._loadSceneAsyncOperationDeque then
        self._loadSceneAsyncOperationDeque = Deque.new()
    end

    local lsAsyncOperation = SceneManager.loadSceneAsync(sceneName, LoadSceneMode.Additive)
    Deque.pushFront(self._loadSceneAsyncOperationDeque, lsAsyncOperation)

    while not lsAsyncOperation:getIsDone() do
        Coroutine.yieldNullUnlimit()
    end

    SceneManager.unloadSceneAssetbundle(sceneName)
end

function unloadSceneAsync(self, doUnloadScene)
    if nil == self.systemClass.sceneDefine then
        return
    end

    if false == doUnloadScene then
        return
    end

    local sceneName = self.systemClass.sceneDefine.name
    if nil == self._loadSceneAsyncOperationDeque then
        return
    end

    local lsAsyncOperation = Deque.popFront(self._loadSceneAsyncOperationDeque)

    if nil == lsAsyncOperation then
        return
    end

    while not lsAsyncOperation:getIsDone()
    do
        Coroutine.yieldNullUnlimit()
    end

    SceneManager.setSceneGOActiveByNameAndActivatePreScene(sceneName, false)
    SceneManager.unLoadSceneAsync(sceneName)
end

function preShowPanelAsync(self, shouldDoSceneTask)
    while self._inPanelAsync
    do
        Coroutine.yieldNullUnlimit()
    end

    self._inPanelAsync = true

    if table.tryGetValue(self.systemClass.tags, "overlay", false) and not table.tryGetValue(self.systemClass.tags, "overlayManualControlCamera", false) then
        UIVisibilitySystem.instance:hide(PanelDataConfig.PanelGroupType.World)
    end

    self:loadSceneAsync(shouldDoSceneTask)

    self._inPanelAsync = false
end

function showPanelAsync(self)
    self:preShowPanelAsync(true)
    self.panel:show()
end

function preHidePanelAsync(self, shouldDoSceneTask)
    while self._inPanelAsync
    do
        Coroutine.yieldNullUnlimit()
    end

    self._inPanelAsync = true
    self:unloadSceneAsync(shouldDoSceneTask)
    self._inPanelAsync = false
end

function hidePanelAsync(self)
    self:preHidePanelAsync(true)
    if self.panel then
        self.panel:hide()
    end
end

function hidePanel(self)
    if table.tryGetValue(self.systemClass.panelDefine, "loadSync", true) and nil == self.systemClass.sceneDefine then
        if not self.panel:isLoaded() then
            return
        end
        self.panel:hide()
    else
        self:startCoroutine(hidePanelAsync)
    end
end

function addAsyncCallback(self, context, callback, ...)
    if nil == callback or self.panel:isLoaded() then
        return
    end

    self._asyncCallbacks = self._asyncCallbacks or {}
    self._asyncContexts = self._asyncContexts or {}
    self._asyncParams = self._asyncParams or {}

    local index = #self._asyncCallbacks + 1

    self._asyncCallbacks[index] = callback
    self._asyncContexts[index] = context
    self._asyncParams[index] = { ... }
end

function _doCallbacks(self)
    if nil == self._asyncCallbacks or #self._asyncCallbacks < 0 then
        return
    end

    for i, callback in ipairs(self._asyncCallbacks) do
        if "function" == type(callback) then
            local params = self._asyncParams[i]
            local context = self._asyncContexts[i]
            callback(context or self, unpack(params))
        end
    end

    self._asyncCallbacks = {}
    self._asyncContexts = {}
    self._asyncParams = {}
end

function _clearHideCallback(self)
    if nil ~= self._asyncCallbacks then
        for i, v in ipairs(self._asyncCallbacks) do
            if v == hidePanel then
                self._asyncCallbacks[i] = ""
            end
        end
    end
end

function popup(self, system, ...)
    AdditivePopupSystem.instance:Popup(system, self.systemClass, ...)
end

function isVisible(self)
    return self.panel ~= nil and self.panel:isLoaded() and self.panel.visible
end

function doCacheUIPanel(self)
    local systemName = self.systemClass.__classname
    local panelName = self.systemClass.panelDefine.File
    UIPanelServiceExport.CachePanel(systemName, panelName)
end

function afterLeave(self)
end

function unloadPanel(self)
    if nil == self.panel then
        return
    end

    self.panel:Unload()
end
