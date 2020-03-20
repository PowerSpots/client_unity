local SystemComponent = Gankx.SystemComponent
local CompositedAsyncOperation = Gankx.CompositedAsyncOperation

module("Gankx", package.seeall)

service("SystemService")

local systemScopeClasses = {}
local allSystemClasses = {}

function startup(self)
    self._currentScope = nil

    self._scopeObjectList = {}
    self._switchScopeOperation = nil
end

function shutdown(self)
end

local function _loadScope(scope, async)
    if nil == scope then
        return
    end

    local scopeSystemClasses = systemScopeClasses[scope]
    if nil == scopeSystemClasses then
        Console.error("_loadScope failed " .. scope)
        return
    end

    local scopeName = SystemScope.toString(scope)
    local scopeGameObject = GameObject:new("systems[" .. tostring(scopeName) .. "]")

    scopeGameObject:setActive(false)

    Console.info("_loadScope:" .. scopeName .. "|" .. #scopeSystemClasses)

    for i, systemClass in ipairs(scopeSystemClasses) do
        local shouldLoad = true
        if systemClass.__shouldLoad ~= nil then
            local bOK, result = pcall(systemClass.__shouldLoad)
            shouldLoad = bOK and result
        end

        if shouldLoad then
            systemClass.instance = scopeGameObject:addComponent(systemClass)
            systemClass.instance:__load(systemClass, async)
        end
    end

    scopeGameObject:setActive(true)
    scopeGameObject:sendMessage("onInit")

    return scopeGameObject
end

local function _unloadScope(scope, scopeGameObject)
    if nil == scope or nil == scopeGameObject then
        return
    end

    local scopeSystemClasses = systemScopeClasses[scope]
    if nil == scopeSystemClasses then
        return
    end

    scopeGameObject:sendMessage("_onRelease")

    for i, systemClass in ipairs(scopeSystemClasses) do
        if nil ~= systemClass.instance then
            systemClass.instance:__unload()
            systemClass.instance = nil
        end
    end

    scopeGameObject:destroy()
end

local function _getScopeLoadWeight(scope)
    if nil == scope then
        return 0
    end

    local scopeSystemClasses = systemScopeClasses[scope]
    if nil == scopeSystemClasses then
        return 0
    end

    local loadWeight = 0

    for i, systemClass in ipairs(scopeSystemClasses) do
        loadWeight = loadWeight + table.tryGetValue(systemClass.tags, "loadWeight", 0)
        if systemClass.panelDefine ~= nil then
            loadWeight = loadWeight + table.tryGetValue(systemClass.panelDefine.tags, "loadWeight", 1)
        end
    end

    return loadWeight
end

local function _unloadUnusedAssets(self, async)
    local unloadUnusedAssets = self.unloadUnusedAssets
    if nil == unloadUnusedAssets then
        return
    end

    CompositedAsyncOperation.beginGroup(async, self.unloadUnusedAssetsWeight)
    local bOk, result = pcall(unloadUnusedAssets, async)
    if not bOk then
        self:error("unloadUnusedAssets", result)
    end
    CompositedAsyncOperation.endGroup(async)
end

function _switchScope(self, scope, isAsync)
    CS.ResourceServiceExport.Collect()

    local switchScopeOperation = self._switchScopeOperation
    local ignoreSameScope = true
    if switchScopeOperation ~= nil then
        self:stopCoroutine(_switchScope)
        switchScopeOperation._isDone = true
        self._switchScopeOperation = nil
        ignoreSameScope = false
    end

    switchScopeOperation = CompositedAsyncOperation:new(scope)
    if isAsync then
        self._switchScopeOperation = switchScopeOperation
    end

    if not SystemScope.isValid(scope) then
        self:error("switchScope", debug.traceback("Invalid Parameters!"))
        switchScopeOperation._isDone = true
        return
    end

    if not ignoreSameScope and scope == self._currentScope then
        local scopeSystemClasses = systemScopeClasses[scope]
        if nil ~= scopeSystemClasses then
            local scopeGameObject = self._scopeObjectList[#self._scopeObjectList]
            switchScopeOperation:setTotalWeight(100)
            scopeGameObject:sendMessage("_onReload", switchScopeOperation)
        end
        switchScopeOperation._isDone = true
        return
    end

    local targetScopePath = {}
    local result = SystemScope.findScopeInTree(scope, targetScopePath)
    if result ~= SystemScope.FindResult.TRUE then
        self:error("switchScope", "Only leaf scope node can be switched! " .. SystemScope.toString(scope) .. " is not leaf scope")
        return
    end

    local diffIndex = 1
    local iScope
    local iScopeObject

    for i = #self._scopeObjectList, 1, -1 do
        iScopeObject = self._scopeObjectList[i]
        iScope = iScopeObject.__scope
        if iScope ~= targetScopePath[i] then
            _unloadScope(iScope, iScopeObject)
            self._scopeObjectList[i] = nil
            diffIndex = i
        end
    end

    CS.ResourceServiceExport.Collect()

    local totalWeight = self.unloadUnusedAssetsWeight or 0
    if totalWeight < 0 then
        totalWeight = 0
    end

    for i = diffIndex, #targetScopePath, 1 do
        totalWeight = totalWeight + _getScopeLoadWeight(targetScopePath[i])
    end

    switchScopeOperation:setTotalWeight(totalWeight)

    _unloadUnusedAssets(self, self._switchScopeOperation)

    for i = diffIndex, #targetScopePath, 1 do
        iScopeObject = _loadScope(targetScopePath[i], self._switchScopeOperation) or {}
        iScopeObject.__scope = targetScopePath[i]
        self._scopeObjectList[i] = iScopeObject
    end

    self._currentScope = scope

    switchScopeOperation._isDone = true

    CS.ResourceServiceExport.Collect()

    fireEvent("OnSystemScopeChanged", self._currentScope)
end

function getCurrentScope(self)
    return self._currentScope
end

function isInScope(self, scope)
    local targetScopePath = {}
    local result = SystemScope.findScopeInTree(self._currentScope, targetScopePath)
    for i, v in ipairs(targetScopePath) do
        if v == scope then
            return true
        end
    end
    return false
end

function switchScope(self, scope)
    self:_switchScope(scope)
end

function switchScopeAsync(self, scope)
    self:startCoroutine(_switchScope, scope, true)
    return self._switchScopeOperation
end

local function addSystem(systemName, systemClass, scope)
    scope = scope or SystemScope.Global

    if allSystemClasses[systemName] ~= nil then
        -- ignored can redefine this system
    end

    local scopeClasses = systemScopeClasses[scope]
    if nil == scopeClasses then
        scopeClasses = {}
        systemScopeClasses[scope] = scopeClasses
    end

    Console.info("addSystem:" .. systemName .. "|scope:" .. scope)
    table.insert(scopeClasses, systemClass)
    allSystemClasses[systemName] = systemClass
end

function __getSystemsByScope(self, scope)
    return systemScopeClasses[scope]
end

function __getAllSystems(self)
    return allSystemClasses
end

local fileNameDic = {}

function getSystemByFileName(self, fileName)
    for _, system in pairs(allSystemClasses) do
        if nil ~= system.panelDefine then
            if nil == fileNameDic[system.panelDefine.file] then
                local panelFileNameStrs = string.split(system.panelDefine.file, "/")
                fileNameDic[system.panelDefine.file] = panelFileNameStrs[#panelFileNameStrs]
            end

            local realFileName = fileNameDic[system.panelDefine.file]
            if realFileName == fileName then
                return system
            end
        end
    end
end

function getSystemByName(self, name)
    return allSystemClasses[name]
end

function getSystemAndWindowIdByUIPath(self, panelFileName, relPath)
    if nil == panelFileName then
        return
    end

    local system = self:getSystemByFileName(panelFileName)
    if nil == system then
        return
    end

    if system.instance == nil or system.instance.Panel == nil then
        return system
    end

    local windowId = CS.UIWindowExport.GetWindowIdByAbsPath(system.instance.panel.id, relPath)

    return system, windowId
end

function system(name, scope, baseClass)
    local fenv = getfenv(2)

    local systemMeta
    if scope == classPartial then
        systemMeta = ClassFactory.define(fenv, name, classPartial)
    else
        if baseClass == nil then
            systemMeta = ClassFactory.define(fenv, name, SystemComponent)
        else
            systemMeta = ClassFactory.define(fenv, name, baseClass)
        end
    end

    if nil == systemMeta then
        return
    end

    if scope ~= classPartial then
        addSystem(name, systemMeta, scope)
    end

    setfenv(2, systemMeta)
end

_G.system = system