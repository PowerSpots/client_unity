local Coroutine = Gankx.Coroutine
local List = List
local BlockSystem = BlockSystem
local PanelUnloadService = Gankx.UI.PanelUnloadService

system("NavigationSystem", SystemScope.Global)

local _systemList = List.new()
local _isAsync = false
local _rootSystem

function setRootSystem(self, system)
    _rootSystem = system
end

function setAsync(isAsync)
    _isAsync = isAsync

    if isAsync then
        BlockSystem.instance:showPanel()
    else
        BlockSystem.instance:hidePanel()
    end
end

function _clear(system)
    while _systemList:front() ~= nil and _systemList:front() ~= system do
        _close()
    end

    setAsync(false)
end

function onInit(self)
    _clear()
end

function onRelease(self)
    _clear()
end

function isAsync(self)
    return _isAsync
end

function _close()
    local system = _systemList:popFront()
    if nil == system then
        return
    end

    local systemInstance = system.instance
    if nil ~= systemInstance then
        systemInstance:hidePanel()
        systemInstance:_receiveMessage("onLeave")
    end

    fireEvent("OnNavigateLeaveSystem", system)

    if nil ~= systemInstance then
        systemInstance:_receiveMessage("afterLeave")
    end
end

local function _preHideSystemAsync(system, shouldDoSceneTask)
    if nil == system or nil == system.instance then
        return
    end
    if table.tryGetValue(system.instance.panelDefine.tags, "loadSync", true) and nil == system.instance.systemClass.SceneDefine then

    else
        system.instance:preHidePanelAsync(shouldDoSceneTask)
    end
end

local function _preShowSystemAsync(system, shouldDoSceneTask)
    if nil == system or nil == system.instance then
        return
    end
    if table.tryGetValue(system.instance.panelDefine.tags, "loadSync", true) and nil == system.instance.systemClass.SceneDefine then
        system.instance:loadPanelSync()
    else
        system.instance:loadPanelAsync()
        system.instance:preShowPanelAsync(shouldDoSceneTask)
    end

    PanelUnloadService.instance:onSystemPanelShow(system)
end

local function checkShouldDoSceneTask(preSystem, nextSystem)
    if nil == nextSystem or nil == nextSystem.instance then
        return true
    end

    if nil == preSystem or nil == preSystem.instance then
        return true
    end

    if nil == nextSystem.instance.systemClass.SceneDefine then
        return true
    end

    if nil == preSystem.instance.systemClass.SceneDefine then
        return true
    end

    if preSystem.instance.systemClass.SceneDefine.name == nextSystem.instance.systemClass.SceneDefine.name then
        return false
    end

    return true
end

local function _switchSystemAsync(preSystem, nextSystem, isEnter)
    local shouldDoSceneTask = checkShouldDoSceneTask(preSystem, nextSystem)

    _preShowSystemAsync(nextSystem, shouldDoSceneTask)
    _preHideSystemAsync(preSystem, shouldDoSceneTask)

    if nil ~= preSystem and nil ~= preSystem.instance then
        if isEnter then
            preSystem.instance:_receiveMessage("onOverride")
        else
            preSystem.instance:_receiveMessage("onLeave")
        end

        if preSystem == nextSystem and preSystem.instance:isVisible() then
            preSystem.instance:_receiveMessage("onHide")
        else
            preSystem.instance.panel:hide()
        end

        if not isEnter then
            preSystem.instance:_receiveMessage("afterLeave")
        end
    end

    if nil ~= nextSystem and nil ~= nextSystem.instance then
        if isEnter then
            nextSystem.instance:_receiveMessage("onEnter")
        else
            nextSystem.instance:_receiveMessage("onResume", preSystem)
        end

        if preSystem == nextSystem and nextSystem.instance:isVisible() then
            nextSystem.instance:_receiveMessage("onShow")
        else
            nextSystem.instance.panel:show()
        end
    end
end

local function _enterOrOnShowSystemAsync(context, system)
    while _isAsync
    do
        Coroutine.yieldNullUnlimit()
    end

    local preSystem = _systemList:front()
    local nextSystem = system

    setAsync(true)

    if preSystem ~= system then
        _systemList:pushFront(system)
    end

    _switchSystemAsync(preSystem, nextSystem, true)

    setAsync(false)
    fireEvent("OnNavigateEnterSystem", system)
end

local function _leaveSystemAsync(context, system)
    while _isAsync
    do
        Coroutine.yieldNullUnlimit()
    end

    local curSystem = _systemList:front()
    if nil ~= _rootSystem and curSystem == _rootSystem then
        return
    end

    if nil == system or curSystem == system then
        setAsync(true)
        local preSystem = _systemList:popFront()
        local nextSystem = _systemList:front()
        _switchSystemAsync(preSystem, nextSystem, false)
        setAsync(false)
        fireEvent("OnNavigateLeaveSystem", preSystem)
    end
end

function enter(self, system)
    if nil == system or nil == system.instance then
        return
    end

    fireEvent("OnNavigateExecuteEnter", system)
    self:startCoroutine(_enterOrOnShowSystemAsync, system)
end

function leave(self, system)
    local curSystem = _systemList:front()
    if nil ~= system and system ~= curSystem then
        return
    end

    self:startCoroutine(_leaveSystemAsync, system)
end

function leaveAll(self)
    _clear()
end

function leaveTo(self, system)
    _clear(system)
    if system ~= nil and system.instance ~= nil then
        system.instance:_receiveMessage("onResume")
        system.instance:showPanel()
    end
end

function isEmpty(self)
    return _systemList:empty()
end

function getFrontSystem(self)
    return _systemList:front()
end

function isFrontSystem(self, system)
    local curSystem = _systemList:front()
    return system == curSystem
end

function contains(self, system)
    return _systemList:contains(system)
end

function clear(self)
    _clear()
end

_G.navigateTo = function(system)
    NavigationSystem.instance:enter(system)
end

_G.navigateBack = function()
    NavigationSystem.instance:leave()
end

_G.navigateBackTo = function(system)
    NavigationSystem.instance:leaveTo(system)
end

_G.navigateHome = function()
    NavigationSystem.instance:leaveTo(_rootSystem)
end

_G.setNavigateHome = function(system)
    NavigationSystem.instance:setRootSystem(system)
end