local UIPanelServiceExport = CS.UIPanelServiceExport
local UIWindowExport = CS.UIWindowExport
local LoadStatus = Gankx.UI.LoadStatus
local LoadOperation = Gankx.UI.LoadOperation
local ExportBind = Gankx.UI.ExportBind
local InvalidId = UIPanelServiceExport.INVALID_ID

module("Gankx.UI.Widget", package.seeall)

local WidgetBaseMeta = {}
local WidgetRootMeta = { __index = _G }
local WidgetMetas = {}

function _G.widget(widgetName, baseName)
    local newWidgetMeta = WidgetMetas[widgetName]

    if nil == newWidgetMeta then
        newWidgetMeta = {}
        WidgetMetas[widgetName] = newWidgetMeta
    end

    if baseName ~= classPartial then
        newWidgetMeta.__base = baseName or ""
        newWidgetMeta.__name = widgetName
        newWidgetMeta.__index = newWidgetMeta

        setmetatable(newWidgetMeta, WidgetRootMeta)
    end

    setfenv(2, newWidgetMeta)
end

function buildWidgetMetas()
    for k, v in pairs(WidgetMetas) do
        local baseMeta = WidgetMetas[v.__base]
        v.__super = nil
        if baseMeta ~= nil then
            setmetatable(v, baseMeta)
            v.__super = baseMeta
        end
    end
end

local function xpcallErrorHandle(msg)
    return debug.traceback(msg)
end

local __isWidget
local __parseChildId

local __create, __createVar, __createVarTable, __createChildren
local __init, __onInit
local __bind, __bindVar, __bindChild, __bindVarTable, __bindChildren, __isBound
local __bindHandles
local __unbind, __unbindVarTable, __unbindChildren
local __unbindHandles
local __load, __onLoad, __isLoaded
local __unload, __onUnload

local keyEncodeMap = {
    path = 1,
    var = 2,
    __setting = 3,
    __parent = 4,
    context = 5,
    children = 6,
    __messageRouter = 7,
    __handles = 8,
    __meta = 9,
    __define = 10,
    type = 11,
    id = 12,
    __loaded = 13,
    __bound = 14,
}

__getAllMetas = function()
    return WidgetMetas
end

__isWidget = function(t)
    if nil == t then
        return false
    end

    if type(t) ~= "table" then
        return false
    end

    return getmetatable(t) == WidgetBaseMeta
end

__parseChildId = function(parent, childName)
    if nil == childName then
        return InvalidId
    end

    local windowId = InvalidId

    if type(childName) == "string" then
        windowId = UIWindowExport.GetChildByName(parent.id, childName)
    elseif type(childName) == "number" then
        windowId = childName
    end

    return windowId
end

__create = function(path, var, parent, setting)
    local newWidget = {}
    setmetatable(newWidget, WidgetBaseMeta)

    if not __init(newWidget, path, var, parent, setting) then
        return
    end

    return newWidget
end

__createVar = function(parent, setting)
    if nil == parent or nil == setting then
        return
    end

    local path = tostring(setting.path or "")
    local var = tostring(setting.var or "")
    local count = tonumber(setting.count or 1)

    if count <= 1 then
        return __create(path, var, parent, setting)
    else
        local widgetArray = {}
        for i = 1, count do
            local newWidget = __create(path .. i, "", parent, setting)

            if newWidget then
                table.insert(widgetArray, newWidget)
            end
        end
        return widgetArray
    end
end

__createVarTable = function(parent, settings, varTable)
    for i, setting in ipairs(settings) do
        if not setting.lazyCreate then
            local widgetVar = __createVar(parent, setting)
            local var = setting.var or ""
            local anonymous = (var == "")

            if anonymous then
                var = "__var" .. i
            end

            varTable[var] = widgetVar
        end
    end
end

__createChildren = function(self)
    local widgetDefine = self.__define
    if nil == widgetDefine then
        return
    end

    local settings = widgetDefine.widgets
    if nil == settings then
        return
    end

    local widgetChildren = {}

    self.children = widgetChildren

    __createVarTable(self, settings, widgetChildren)
end

__bind = function(self, id)
    if nil == self then
        Console.error("Widget.__bind occured error, invalid parameter!" .. debug.traceback())
        return
    end

    local bound = self.__bound
    if bound then
        return true
    end

    local parent = self.__parent
    if nil == parent then
        return false
    end

    local setting = self.__setting

    id = id or __parseChildId(parent, self.path)
    local required = true
    if setting.required ~= nil then
        required = setting.required
    end

    if not required and id == InvalidId then
        return false
    end

    if id == InvalidId then
        self:__error("bind", "widget window can not find")
        return false
    end

    self.id = id
    self.__bound = true

    UIWindowExport.SetSlotPath(self.id, setting.slot or "")

    if setting.lazyLoad ~= true then
        __load(self)
        return true
    else
        local loaded = UIWindowExport.GetSlotLoadStatus(self.id)
        if loaded == LoadStatus.Loaded then
            __onLoad(self)
        end
    end

    local isVisible = rawget(self, "__loadOnVisible")
    if isVisible ~= nil then
        if self.Visible == true then
            __load(self, LoadOperation.toBool(isVisible))
            rawset(self, "__loadOnVisible", nil)
            return true
        end
    end

    local loadOnBound = rawget(self, "__loadOnBound")
    if loadOnBound ~= nil then
        __load(self, LoadOperation.toBool(loadOnBound))
        rawset(self, "__loadOnBound", nil)
        return true
    end

    return true
end

__bindVar = function(parent, setting, varTable, var)
    var = var or tostring(setting.var or "")
    local count = tonumber(setting.count or 1)

    local widgetVar = varTable[var]
    if widgetVar ~= nil then
        if count <= 1 then
            if not __bind(widgetVar) then
                varTable[var] = nil
            end
        else
            local rmCount = 0
            local widgetArray = widgetVar
            for i = 1, count do
                local widget = widgetArray[i - rmCount]
                if nil == widget or not __bind(widget) then
                    table.remove(widgetArray, i - rmCount)
                    rmCount = rmCount + 1
                end
            end
        end
    end
end

__bindChild = function(self, setting)
    __bindVar(self, setting, self.children)
end

__bindVarTable = function(parent, settings, varTable)
    for i, setting in ipairs(settings) do
        local var = tostring(setting.var or "")
        local anonymous = (var == "")
        if anonymous then
            var = "__var" .. i
        end

        __bindVar(parent, setting, varTable, var)
    end
end

__bindChildren = function(self)
    local controlDefine = self.__define
    if nil == controlDefine then
        return
    end

    local settings = controlDefine.widgets
    if nil == settings then
        return
    end

    __bindVarTable(self, settings, self.children)
end

__isBound = function(self)
    return self.__bound
end

__bindHandles = function(self)
    local widgetDefine = self.__define
    if widgetDefine ~= nil then
        local handles = widgetDefine.handles
        if handles ~= nil then
            for eventName, codes in pairs(handles) do
                self:attachHandle(eventName, self:__parseMetaHandle(codes))
                WidgetEventService.instance:bindEvent(self.id, eventName)
            end
        end
    end

    local widgetSetting = self.__setting
    local widgetParent = self.__parent
    local handles = widgetSetting.handles
    if handles ~= nil then
        for eventName, codes in pairs(handles) do
            self:attachHandle(eventName, widgetParent:__parseHandle(codes))
            WidgetEventService.instance:bindEvent(self.id, eventName)
        end
    end
end

__unbind = function(self)
    if not __isWidget(self) then
        return
    end

    __unload(self)

    self.id = InvalidId
    self.__bound = false
end

__unbindVarTable = function(parent, settings, varTable)
    for i, setting in ipairs(settings) do
        local var = tostring(setting.var or "")
        local anonymous = (var == "")
        local count = tonumber(setting.count or 1)

        if anonymous then
            var = "__var" .. i
        end

        local widgetVar = varTable[var]
        if nil ~= widgetVar then
            if count <= 1 then
                __unbind(widgetVar)
            else
                local rmCount = 0
                local widgetArray = widgetVar
                for j = 1, count do
                    local control = widgetArray[j - rmCount]
                    if nil ~= control then
                        __unbind(control)
                    end
                end
            end
        end
    end
end

__unbindChildren = function(self)
    local widgetDefine = self.__define
    if nil == widgetDefine then
        return
    end

    local settings = widgetDefine.widgets
    if nil == settings then
        return
    end

    __unbindVarTable(self, settings, self.children)
end

__unbindHandles = function(self)
    local widgetDefine = self.__define
    if widgetDefine ~= nil then
        local handles = widgetDefine.handles
        if handles ~= nil then
            for eventName, codes in pairs(handles) do
                self:detachHandle(eventName)
            end
        end
    end

    local widgetSetting = self.__setting
    local handles = widgetSetting.handles
    if handles ~= nil then
        for eventName, codes in pairs(handles) do
            self:detachHandle(eventName)
        end
    end
end

__load = function(self, isAsync)
    local widgetSetting = self.__setting
    if nil == isAsync then
        isAsync = widgetSetting.asyncLoad or false
    end

    local loaded = self.__loaded
    if loaded == LoadStatus.Loaded then
        return
    end

    if isAsync then
        self:attachHandle("OnLoadAsync", __onLoad)
        UIWindowExport.LoadSlotAsync(self.id)
    else
        UIWindowExport.LoadSlot(self.id)
        __onLoad(self)
    end
end

__onLoad = function(self)
    self.__loaded = LoadStatus.Loaded

    __bindChildren(self)

    __bindHandles(self)

    ExportBind(self)

    self:__callInterface("onLoad")
end

__unload = function(self)
    __onUnload(self)
end

__onUnload = function(self)
    self:__callInterface("onUnload")

    self.__loaded = LoadStatus.None

    __unbindChildren(self)

    __unbindHandles(self)
end

__isLoaded = function(self)
    return self.__loaded == LoadStatus.Loaded
end

__onInit = function(self)
    rawset(self, keyEncodeMap["id"], InvalidId)

    local parent = self.__parent
    local setting = self.__setting

    local router = parent.__messageRouter
    if nil == router then
        self:__error("init", "message router is nil")
        return false
    end

    local type = tostring(setting.type or "Window")
    local widgetMeta = WidgetMetas[type]
    if nil == widgetMeta then
        self:__error("init", "widget type(" .. tostring(type) .. ") is not defined")
        return false
    end

    self.type = type
    self.__messageRouter = router
    self.__meta = widgetMeta
    self.__define = widgetMeta.WidgetDefine
    self.__loaded = LoadStatus.None
    self.__bound = false

    __createChildren(self)

    self:__callInterface("onInit", setting)

    return true
end

__init = function(self, path, var, parent, setting)
    rawset(self, keyEncodeMap["path"], path or "")
    rawset(self, keyEncodeMap["var"], var or "")

    rawset(self, keyEncodeMap["__setting"], setting)
    rawset(self, keyEncodeMap["__parent"], parent)

    if false == setting.required then
        return true
    end

    return __onInit(self)
end

function WidgetBaseMeta:__parseHandle(handleName)
    return createHandler(self, self:__parseMetaHandle(handleName))
end

function WidgetBaseMeta:__parseMetaHandle(handleName)
    local widgetMeta = self.__meta

    if nil == handleName or nil == widgetMeta then
        return
    end

    handleName = widgetMeta[handleName]

    if type(handleName) == "function" then
        return handleName
    end
end

function WidgetBaseMeta:__parseFullPath()
    local fullPath = self:__parseNameSpace() .. "/" .. self.path
    if string.len(self.var) > 0 then
        fullPath = fullPath .. "<" .. self.var .. ">"
    end

    return fullPath
end

function WidgetBaseMeta:__parseNameSpace()
    local parent = self.__parent
    local namespace = ""
    if nil ~= parent then
        namespace = parent:__parseFullPath()
    end

    return namespace
end

function WidgetBaseMeta:__error(title, msg)
    local namespaceFull = self:__parseFullPath() or "unknown path"
    Console.error("widget(" .. tostring(namespaceFull) .. ") " .. tostring(title) .. " occurred error: " .. tostring(msg) .. debug.traceback(""))
end

function WidgetBaseMeta:__callInterface(handleName, ...)
    local handle = self:__parseMetaHandle(handleName)

    if nil == handle then
        return
    end

    local status, result = pcall(handle, self, ...)

    if not status then
        local errorMsg = "call interface " .. tostring(handleName)
        self:__error(errorMsg, result)
    end
end

WidgetBaseMeta.hasFunc = function(self, key)
    local value = rawget(self, key) or rawget(WidgetBaseMeta, key)
    if nil ~= value then
        return true
    end

    local realKey = "get" .. key
    local metaTable = self.__meta

    if metaTable ~= nil then
        local getFunc = metaTable[realKey]
        if nil ~= getFunc then
            return true
        end

        local opeFunc = metaTable[key]
        if nil ~= opeFunc then
            return true
        end
    end

    return false
end

WidgetBaseMeta.__call = function(self, windowId, msgName, ...)
    local handles = self.__handles
    if nil == handles then
        return
    end

    local handle = handles[msgName]
    if nil == handle then
        return
    end

    local status, result = xpcall(handle, xpcallErrorHandle, self, ...)
    if not status then
        local windowName = UIWindowExport.GetObjectName(windowId)
        local errorMsg = "handle message '" .. tostring(msgName) .. "' on window " .. tostring(windowName)
        self:__error(errorMsg, result)
    end
end

WidgetBaseMeta.__index = function(self, key)
    if nil == rawget(self, keyEncodeMap["id"]) then
        if not __onInit(self) then
            return nil
        end
    end

    local encodedKey = keyEncodeMap[key]
    if nil ~= encodedKey then
        local value = rawget(self, encodedKey)
        if nil == value then
            if key == "context" then
                value = {}
                rawset(self, encodedKey, value)
            else
                value = rawget(WidgetBaseMeta, encodedKey)
            end
        end

        return value
    end

    local value = rawget(self, key) or rawget(WidgetBaseMeta, key)
    if nil ~= value then
        return value
    end

    local realKey = "get" .. key
    local metaTable = rawget(self, keyEncodeMap["__meta"])

    if metaTable ~= nil then
        local getFunc = metaTable[realKey]
        if nil ~= getFunc then
            return getFunc(self)
        end

        local opeFunc = metaTable[key]
        if nil ~= opeFunc then
            return opeFunc
        end
    end

    WidgetBaseMeta.__error(self,
            "get property or method '" .. tostring(key) .. "'",
            "undefined")
end

WidgetBaseMeta.__newindex = function(self, key, value)
    if nil == rawget(self, keyEncodeMap["id"]) then
        if not __onInit(self) then
            return
        end
    end

    local encodedKey = keyEncodeMap[key]
    if nil ~= encodedKey then
        rawset(self, encodedKey, value)
        return
    end

    local realKey = "set" .. key
    local metaTable = rawget(self, keyEncodeMap["__meta"])

    if metaTable ~= nil then
        local setFunc = metaTable[realKey]
        if nil ~= setFunc then
            return setFunc(self, value)
        end
    end

    WidgetBaseMeta.__error(self,
            "set property '" .. tostring(key) .. "' occurred error",
            "undefined " .. debug.traceback())
end

function WidgetBaseMeta:attachHandle(msgName, codes)
    if nil == msgName then
        self:__error("attach handle", "message name is nil")
        return false
    end

    if nil == codes then
        self:__error("attach '" .. msgName .. "' handle", "handle is nil")
        return false
    end

    self.__messageRouter:attachHandle(self.id, msgName, self)
    local handles = self.__handles
    if nil == handles then
        handles = {}
        self.__handles = handles
    end

    handles[msgName] = codes
    return true
end

function WidgetBaseMeta:detachHandle(msgName)
    if nil == msgName then
        self:__error("detach handle", "message name is nil")
        return false
    end

    self.__messageRouter:detachHandle(self.id, msgName)
    local handles = self.__handles
    if nil == handles then
        return true
    end

    handles[msgName] = nil
    return true
end

function WidgetBaseMeta:postMessage(msgName, ...)
    if nil == msgName then
        self:__error("post message", "message name is nil")
        return
    end

    self.__call(self, self.id, msgName, ...)
end

function WidgetBaseMeta:messageBox(...)
    return self.__parent:messageBox(...)
end

function WidgetBaseMeta:clone()
    local newWidget = {}
    setmetatable(newWidget, WidgetBaseMeta)

    if not __init(newWidget, self.path,
            "", self.__parent, self.__setting) then
        return
    end

    if self.__bound then
        local newCloneId = UIWindowExport.clone(self.id)
        if not __bind(newWidget, newCloneId) then
            return
        end
    end

    return newWidget
end

WidgetBaseMeta.createChild = function(self, var)
    if self.children[var] ~= nil then
        return self.children[var]
    end

    if nil == var then
        self:__error("create child", "var name is nil")
        return nil
    end

    local widgetDefine = self.__define
    if nil == widgetDefine then
        return nil
    end

    local settings = widgetDefine.widgets
    if nil == settings then
        return nil
    end

    local varTable = self.children

    for i, setting in ipairs(settings) do
        if setting.lazyCreate and var == setting.var then
            local widgetVar = __createVar(self, setting)
            varTable[var] = widgetVar

            local loaded = self.__loaded
            if loaded == LoadStatus.Loaded then
                __bindChild(self, setting)
            end

            return widgetVar
        end
    end
end

WidgetBaseMeta.load = function(self, isAsync)
    local bound = self.__bound
    if not bound then
        rawset(self, "__loadOnBound", LoadOperation.fromBool(isAsync))
        return
    end

    __load(self, isAsync)
end

WidgetBaseMeta.loadOnVisible = function(self, isAsync)
    local loaded = self.__loaded
    if loaded ~= LoadStatus.None then
        return
    end

    local bound = self.__bound
    if not bound then
        rawset(self, "__loadOnVisible", LoadOperation.fromBool(isAsync))
        return
    end

    local visible = self.visible
    if not visible then
        rawset(self, "__loadOnVisible", LoadOperation.fromBool(isAsync))
        return
    end

    __load(self, isAsync)
end

WidgetBaseMeta.checkOnVisible = function(self)
    local isVisible = rawget(self, "__loadOnVisible")
    if isVisible ~= nil then
        local bound = self.__bound
        if not bound then
            return
        end

        __load(self, LoadOperation.toBool(isVisible))
    end
end

WidgetBaseMeta.isLoaded = __isLoaded
WidgetBaseMeta.bind = __bind
WidgetBaseMeta.unbind = __unbind
WidgetBaseMeta.isBound = __isBound

createVar = __createVar
createVarTable = __createVarTable
bindVarTable = __bindVarTable
ubindVarTable = __unbindVarTable

createBySetting = function(path, parent, setting)
    local newWidget = {}
    setmetatable(newWidget, WidgetBaseMeta)

    if not __init(newWidget, path, "", parent, setting) then
        return
    end

    return newWidget
end

createByType = function(path, parent, type)
    local setting = {
        type = type,
    }

    return createBySetting(path, parent, setting)
end

function list(type)
    local widgetMeta = WidgetMetas[type]
    local tabString = "    "
    local tabTotal = ""
    while widgetMeta and widgetMeta ~= WidgetRootMeta do
        widgetMeta = getmetatable(widgetMeta)
        tabTotal = tabTotal .. tabString
    end
end
