local UIPanelServiceExport = CS.UIPanelServiceExport
local AsyncOperation = Gankx.AsyncOperation
local InvalidId = UIPanelServiceExport.INVALID_ID

module("Gankx.UI", package.seeall)

class("PanelCreateLoadRequest", AsyncOperation)

function constructor(self, panelName, panelFile)
    __super.constructor(self)

    self.name = panelName
    self.file = panelFile
    self.id = InvalidId

    Event.listen("OnLoadPanelAsync", self, onLoaded)
    local succeed = UIPanelServiceExport.LoadPanelAsync(panelName, panelFile)
    if not succeed then
        self._isDone = true
    end
end

function onLoaded(self, panelId, panelName)
    print("onloaded panel" .. tostring(panelName) .. "" .. tostring(panelId))
    if panelName ~= self.name then
        return
    end

    self.id = panelId
    self._isDone = true
    self._progress = 1
end

function onStop(self)
    UIPanelServiceExport.CancelLoadPanelAsync(self.name)
end

function onResume(self)
    Event.unlistenAll(self)
end
