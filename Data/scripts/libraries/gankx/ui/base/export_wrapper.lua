local LoadStatus = Gankx.UI.LoadStatus

module("Gankx.UI", package.seeall)

local ExportWrapperMeta = {}
ExportWrapperMeta.__index = ExportWrapperMeta

function ExportWrapper(exportInterface)
    local object = {}
    object.interface = exportInterface
    setmetatable(object, ExportWrapperMeta)
    return object
end

local function __setProperty(widget, key, value)
    local properties = widget.context._exportSetProperties
    if nil == properties then
        properties = {}
        widget.context._exportSetProperties = properties
    end

    properties[key] = value
end

local function __getProperty(widget, key)
    local properties = widget.context._exportSetProperties
    if nil == properties then
        return nil
    end

    return properties[key]
end

local function __addProperty(widget, key, value)
    if nil == value then
        return
    end

    local properties = widget.context._exportAddProperties
    if nil == properties then
        properties = {}
        widget.context._exportAddProperties = properties
    end

    local addProperty = properties[key]
    if nil == addProperty then
        addProperty = {}
        properties[key] = addProperty
    end

    table.insert(addProperty, value)
end

local function __clearProperty(widget, key)
    local properties = widget.context._exportAddProperties
    if nil == properties then
        return
    end

    properties[key] = nil
end

local function __autoValue(value)
    if type(value) == "table" then
        return unpack(value)
    else
       return value
    end
end

ExportWrapperMeta.setProperty = __setProperty
ExportWrapperMeta.getProperty = __getProperty
ExportWrapperMeta.addProperty = __addProperty
ExportWrapperMeta.clearProperty = __clearProperty

function ExportBind(widget)
    local setProperties = widget.context._exportSetProperties
    if setProperties ~= nil then
        for k, setValue in pairs(setProperties) do
            widget[k] = __autoValue(setValue)
        end

        widget.context._exportSetProperties = nil
    end

    local addProperties = widget.context._exportAddProperties
    if addProperties ~= nil then
        for k, addList in pairs(addProperties) do
            for i, addValue in ipairs(addList) do
                widget["add" .. k](widget, addValue)
            end
        end

        widget.context._exportAddOperations = nil
    end
end

function ExportWrapperMeta:setValue(widget, key, value)
    local loaded = widget.__loaded == LoadStatus.Loaded
    if loaded then
        local func = self.interface["Set"..key]
        if nil == func then
            Console.error("ExportWrapper.setValue occured error, no setter is found in export, setter name: " .. tostring("Set"..key))
        else
            func(widget.id, value)
        end
    else
        __setProperty(widget, key, value)
    end
end

function ExportWrapperMeta:getValue(widget, key, defaultValue)
    local loaded = widget.__loaded == LoadStatus.Loaded
    if loaded then
        local func = self.interface["Get"..key]
        return func(widget.id)
    else
        local value = __getProperty(widget, key)
        if nil == value then
            return defaultValue
        end
        return value
    end
end

function ExportWrapperMeta:setVector(widget, key, ...)
    local loaded = widget.__loaded == LoadStatus.Loaded
    if loaded then
        local func = self.interface["Set"..key]
        if nil == func then
            Console.error("ExportWrapper.setVector occured error, no setter is found in export, setter name: " .. tostring("Set"..key))
        else
            func(widget.id, ...)
        end
    else
        __setProperty(widget, key, { ...})
    end
end

function ExportWrapperMeta:getVector(widget, key, ...)
    local loaded = widget.__loaded == LoadStatus.Loaded
    if loaded then
        local func = self.interface["Get"..key]
        return func(widget.id)
    else
        local value = __getProperty(widget, key)
        if nil == value then
            return ...
        end
        return unpack(value)
    end
end

function ExportWrapperMeta:addValue(widget, key, value)
    local loaded = widget.__loaded == LoadStatus.Loaded
    if loaded then
        local func = self.interface["Add"..key]
        if nil == func then
            Console.error("ExportWrapper.addValue occured error, no adder is found in export, adder name: " .. tostring("Add"..key))
        else
            func(widget.id, value)
        end
    else
        __addProperty(widget, key, value)
    end
end

function ExportWrapperMeta:clearValue(widget, key)
    local loaded = widget.__loaded == LoadStatus.Loaded
    if loaded then
        local func = self.interface["Clear"..key]
        if nil == func then
            Console.error("ExportWrapper.clearValue occured error, no clear func is found in export, func name: " .. tostring("Clear"..key))
        else
            func(widget.id)
        end
    else
        __clearProperty(widget, key)
    end
end

function ExportWrapperMeta:setBool(widget, key, value)
    return self:setValue(widget, key, value ~= nil and value ~= false)
end

function ExportWrapperMeta:getBool(widget, key)
    return self:getValue(widget, key, false)
end

function ExportWrapperMeta:setNumber(widget, key, value)
    return self:setValue(widget, key, value or 0)
end

function ExportWrapperMeta:getNumber(widget, key)
    return self:getValue(widget, key, 0)
end

function ExportWrapperMeta:setString(widget, key, value)
    return self:setValue(widget, key, value or "")
end

function ExportWrapperMeta:getString(widget, key)
    return self:getValue(widget, key, "")
end
