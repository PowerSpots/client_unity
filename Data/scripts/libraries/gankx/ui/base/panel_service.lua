local Panel = Gankx.UI.Panel
local Widget = Gankx.UI.Widget

module("Gankx.UI", package.seeall)

service("PanelService")

function startup(self)
    self._allPanels = {}
    self._panelMap = {}

    self:listenEvent("OnPanelMessage", onPanelMessage)

    Widget.buildWidgetMetas()

    Panel.setService(self)
end

function shutdown(self)
    self._allPanels = nil
    self._panelMap = nil

    Panel.setService(nil)
end

function setAsyncLoadCallback(self, callback)
    self._asyncLoadCallback = callback
end

function startAsyncLoad(self, panel)
    local asyncLoadCallback = self._asyncLoadCallback
    if nil == asyncLoadCallback then
        return
    end

    local onStartCallback = asyncLoadCallback.onStart
    if nil == onStartCallback then
        return
    end

    local status, result = pcall(onStartCallback, asyncLoadCallback, panel)
    if not status then
        self:error("startAsyncLoad", result)
    end
end

function stopAsyncLoad(self, panel)
    local asyncLoadCallback = self._asyncLoadCallback
    if nil == asyncLoadCallback then
        return
    end

    local onStopCallback = asyncLoadCallback.onStop
    if nil == onStopCallback then
        return
    end

    local status, result = pcall(onStopCallback, asyncLoadCallback, panel)
    if not status then
        self:error("stopAsyncLoad", result)
    end
end

function createPanel(self, panelName, panelDefine, panelContext)
    if nil == panelName or
            nil == panelDefine or
            nil == panelContext then
        self:error("createPanel", debug.traceback("invalid parameters"))
        return
    end

    local allPanels = self._allPanels
    local panel = allPanels[panelName]
    if panel ~= nil then
        return panel
    end

    panel = Panel.create(panelName, panelDefine, panelContext)
    if nil == panel then
        self:error("createPanel", "create panel(" .. tostring(panelName) .. ") failed")
        return
    end

    allPanels[panelName] = panel

    return panel
end

function bindPanel(self, panel)
    if nil == panel or nil == panel.id then
        self:error("bindPanel", debug.traceback("invalid parameters"))
        return
    end

    self._panelMap[panel.id] = panel
end

function unbindPanel(self, panel)
    if nil == panel or nil == panel.id then
        self:error("unbindPanel", debug.traceback("invalid parameters"))
        return
    end

    self._panelMap[panel.id] = nil
end

function destroyPanel(self, panel)
    if nil == panel or nil == panel.name then
        self:error("destroyPanel", debug.traceback("invalid parameters"))
        return
    end

    self._allPanels[panel.name] = nil

    panel:destroy()
end

function onPanelMessage(self, panelId, windowId, msgName, ...)
    local panelMap = self._panelMap
    local panel = panelMap[panelId]
    if nil == panel then
        return
    end

    fireEvent("OnEarlyPanelMessage", panelId, windowId, msgName, ...)

    panel:_onMessage(windowId, msgName, ...)
end

function getPanel(self, panelId)
    local panelMap = self._panelMap
    local panel = panelMap[panelId]
    return panel
end
