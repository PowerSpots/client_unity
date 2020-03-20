module("Gankx.UI", package.seeall)

service("PanelUnloadService")

local CacheCount = Gankx.Config.UI.panelUnloadCacheCount

function startup(self)
    self:listenEvent("OnSystemPanelHide", onSystemPanelHide)
    self:listenEvent("OnSystemPanelShow", onSystemPanelShow)

    self._latestSystems = {}
end

function shutdown(self)
    self:unlistenAll()
    self._latestSystems = nil
end

function onSystemPanelHide(self, systemClass)
    if nil == systemClass.instance then
        return
    end

    local panelDefine = systemClass.panelDefine
    if panelDefine == nil then
        return
    end

    local unloadOnHide = table.tryGetValue(panelDefine.tags, "unloadOnHide", true) and table.tryGetValue(panelDefine.tags, "lazyLoad", false)
    if not unloadOnHide then
        return
    end

    local noCache = table.tryGetValue(panelDefine.tags, "noCache", false)
    if noCache or CacheCount <= 0 then
        if nil ~= systemClass.instance then
            if not systemClass.instance.panel.visible then
                systemClass.instance:unloadPanel()
            end
        end
    else
        self:_addToLatest(systemClass)
    end
end

function onSystemPanelShow(self, systemClass)
    Array.remove(self._latestSystems, systemClass)
end

local function isSystemInstanceNull(systemClass)
    return systemClass.instance == nil
end

function _addToLatest(self, systemClass)
    Array.removeAll(self._latestSystems, isSystemInstanceNull)

    local index = Array.indexOf(self._latestSystems, systemClass)
    if index ~= 0 then
        Array.removeAt(self._latestSystems, index)
    elseif #self._latestSystems >= CacheCount then
        local oldestSystemClass = self._latestSystems[1]
        Array.removeAt(self._latestSystems, 1)
        if not oldestSystemClass.instance.panel.visible then
            oldestSystemClass.instance:unloadPanel()
        end
    end

    Array.add(self._latestSystems, systemClass)
end

function clearAll(self)
    for _, systemClass in ipairs(self._latestSystems) do
        if nil ~= systemClass.instance and not systemClass.instance.panel.visible then
            systemClass.instance:unloadPanel()
        end
    end

    Array.removeAll(self._latestSystems)
end

function printCache(self)
    local result = "Cache-----"
    for _, systemClass in ipairs(self._latestSystems) do
        result = result .. systemClass.__classname .. ", "
    end

    print(result)
end