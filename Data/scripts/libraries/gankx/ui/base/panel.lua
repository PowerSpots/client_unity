local UIPanelServiceExport = CS.UIPanelServiceExport
local UIWindowExport = CS.UIWindowExport
local PanelCreateLoadRequest = Gankx.UI.PanelCreateLoadRequest
local Widget = Gankx.UI.Widget
local MessageRouter = Gankx.UI.MessageRouter
local Coroutine = Gankx.Coroutine
local LoadStatus = Gankx.UI.LoadStatus
local LoadOperation = Gankx.UI.LoadOperation

module("Gankx.UI.Panel", package.seeall)

local PanelService = nil
function setService(panelService)
    PanelService = panelService
end

local InvalidId = UIPanelServiceExport.INVALID_ID

local PanelBaseMeta = {}
PanelBaseMeta.__index = PanelBaseMeta

function PanelBaseMeta:__parseHandle(handleName)
    return Widget.createHandler(self.__context, self:__parseContextHandle(handleName))
end

function PanelBaseMeta:__parseContextHandle(handleName)
    local panelContext = self.__context
    if nil == handleName or nil == panelContext then
        return
    end

    handleName = panelContext[handleName]

    if type(handleName) == "function" then
        return handleName
    end
end

function PanelBaseMeta:__parseFullPath()
    return self.name
end

function PanelBaseMeta:__error(errorTitle, errorMsg)
    Console.error("panel(" .. (self.name or "") .. ") " .. errorTitle .. " occurred error: " .. errorMsg)
end

local function __debugTraceback(msg)
    return debug.traceback(msg)
end

local __callInterface
local __createWidgets
local __bindWidgets
local __bindHandles
local __load, __onLoad, __isLoaded
local __unload
local __init, __release

__callInterface = function(self, handleName, ...)
    local handle = self:__parseHandle(handleName)

    if nil == handle then
        return
    end

    local status, result = xpcall(handle, __debugTraceback, ...)

    if not status then
        local errorMsg = "call panel interface '" .. tostring(handleName) .. "]'"
        self:__error(errorMsg, result)
    end
end

__createWidgets = function(self)
    local panelContext = self.__context
    local settings = self.__define.widgets

    if nil == settings then
        return
    end

    Widget.createVarTable(self, settings, panelContext)
end

__bindWidgets = function(self)
    local panelContext = self.__context
    local settings = self.__define.widgets

    if nil == settings then
        return
    end

    Widget.bindVarTable(self, settings, panelContext)
end

__unbindWindows = function(self)
    local panelContext = self.__context
    local settings = self.__define.widgets

    if nil == settings then
        return
    end

    Widget.ubindVarTable(self, settings, panelContext)
end

__bindHandles = function(self)
    self.__handles = {}

    local handles = self.__define.handles
    if handles ~= nil then
        for eventName, codes in pairs(handles) do
            self:attachHandle(eventName, self:__parseContextHandle(codes))
            WidgetEventService.instance:bindEvent(self.id, eventName)
        end
    end
end

__onLoad = function(self, id)
    self.__loaded = LoadStatus.Loaded

    self.id = id or InvalidId

    if self.id == InvalidId then
        self:__error("load", "load panel '" .. self.file .. "' failed, please confirm the file path")
        return
    end

    UIWindowExport.SetVisible(self.id, self.visible)

    PanelService:bindPanel(self)

    __bindWidgets(self)

    __bindHandles(self)

    self.__context.panel = self

    __callInterface(self, "onPanelLoad", self)

    if self.visible then
        __callInterface(self, "_onShow")
    end
end

__loadCoroutine = function(self, async)
    if nil == self.name or nil == self.file then
        return
    end

    self.__loaded = LoadStatus.Loading

    PanelService:startAsyncLoad(self)

    local loadRequest = PanelCreateLoadRequest:new(self.name, self.file)

    if nil ~= async then
        async:yield(loadRequest, table.tryGetValue(self.__define.tags, "loadWeight", 1))
    else
        Coroutine.yieldAsyncOperation(loadRequest)
    end

    print("loadRequest id:" .. loadRequest.id)
    __onLoad(self, loadRequest.id)

    PanelService:stopAsyncLoad(self)
end

__load = function(self, isAsync, async)
    if self.__loaded ~= LoadStatus.None then
        return
    end

    if isAsync == true then
        if async ~= nil then
            __loadCoroutine(self, async)
        else
            Coroutine.start(self, __loadCoroutine)
        end
    else
        local id = UIPanelServiceExport.LoadPanel(self.name, self.file)
        __onLoad(self, id)
    end
end

__unload = function(self)
    if self.__loaded == LoadStatus.None then
        return
    end

    __callInterface(self, "onPanelUnload", self)

    __unbindWindows(self)

    Coroutine.stopAll(self)

    if self.__loaded == LoadStatus.Loading then
        PanelService:stopAsyncLoad(self)
    end

    if self:isLoaded() then
        local bOk = UIPanelServiceExport.UnloadPanel(self.name)
        if not bOk then
            self:__error("unload", "unload panel '" .. self.file .. "' failed, please confirm the file path")
        end
    end

    PanelService:unbindPanel(self)

    self.__loaded = LoadStatus.None

    self.id = nil
end

__isLoaded = function(self)
    return self.__loaded == LoadStatus.Loaded
end

__init = function(self, panelName, panelDefine, panelContext)
    if nil == panelName then
        self:__error("init", "panel name is nil")
        return false
    end

    self.name = tostring(panelName)

    if nil == panelDefine then
        self:__error("init", "panel define is nil")
        return false
    end

    self.__define = panelDefine

    if nil == panelDefine.file then
        self:__error("init", "pane file is nil")
        return false
    end

    self.file = tostring(panelDefine.file)

    self.__context = panelContext
    if nil == self.__context then
        self:__error("init", "panel context '" .. self.name .. "' is nil")
        return false
    end

    self.id = InvalidId

    self.visible = false

    self.__messageRouter = MessageRouter.create()
    self.__messageRouter:attachGlobeHandle(self)

    __createWidgets(self)

    self.__loaded = LoadStatus.None
    return true
end

__release = function(self)
    __unload(self)

    self.name = nil
    self.__define = nil
    self.__context = nil

    setmetatable(self, nil)
end

function PanelBaseMeta:__call(windowId, msgName, ...)
    local handle = self.__handles[msgName]
    if nil == handle then
        return false
    end

    local status, result = pcall(handle, self.__context, ...)
    if not status then
        local errorMsg = "handle message '" .. tostring(msgName) .. "'"
        self:__error(errorMsg, result)
    end

    return true
end

function PanelBaseMeta:_onMessage(windowId, msgName, ...)
    return self.__messageRouter:routeMessage(windowId, msgName, ...)
end

function PanelBaseMeta:destroy()
    __release(self)
end

function PanelBaseMeta:attachHandle(msgName, codes)
    if nil == msgName then
        self:__error("attach handle", "message name is nil")
        return false
    end

    if nil == codes then
        self:__error("attach '" .. msgName .. "' handle", "message handle is nil")
        return false
    end

    self.__handles[msgName] = codes
    return true
end

function PanelBaseMeta:detachHandle(msgName)
    if nil == msgName then
        self:__error("detach handle", "message name is nil")
        return false
    end

    self.__handles[msgName] = nil
    return true
end

function PanelBaseMeta:show()
    if self.visible then
        return
    end

    self.visible = true

    if __isLoaded(self) then
        UIWindowExport.SetVisible(self.id, self.visible)
        __callInterface(self, "_onShow")
    else
        local isVisible = self.__loadOnVisible
        if isVisible ~= nil then
            self.__loadOnVisible = nil
            __load(self, LoadOperation.toBool(isVisible))
        end
    end
end

function PanelBaseMeta:hide()
    if not self.visible then
        return
    end

    self.visible = false

    if __isLoaded(self) then
        UIWindowExport.SetVisible(self.id, self.visible)
    end

    __callInterface(self, "_onHide")
end

PanelBaseMeta.isLoaded = __isLoaded
PanelBaseMeta.load = __load
PanelBaseMeta.unload = __unload
PanelBaseMeta.loadOnVisible = function(self, isAsync)
    if self.__loaded ~= LoadStatus.None then
        return
    end

    if self.visible then
        return __load(self, isAsync)
    end

    self.__loadOnVisible = LoadOperation.fromBool(isAsync)
end

PanelBaseMeta.readd = function(self, groupType, sortOrder, depth)
    if nil == groupType then
        groupType = UIPanelServiceExport.GetPanelGroup(self.id)
    end

    sortOrder = sortOrder or UIPanelServiceExport.GetPanelSortOrder(self.id)

    depth = depth or UIPanelServiceExport.GetPanelDepth(self.id)

    UIPanelServiceExport.ReaddPanel(self.id, groupType, sortOrder, depth)
end

function create(panelName, panelDefine, panelContext)
    local object = {}
    setmetatable(object, PanelBaseMeta)
    if __init(object, panelName, panelDefine, panelContext) then
        return object
    end
end
