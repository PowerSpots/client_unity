class("GameObject")

function constructor(self, name)
    self.name = name or "no name"

    self._children = nil
    self._components = {}
    self._componentLookup = {}

    self._active = true
    self._activeInHierarchy = true
    self._started = false
end

function toString(self)
    return "GameObject(" .. self.name .. ")"
end

function getSummaryInfo(self, sb, prefix)
    prefix = prefix or ""
    local subPrefix = prefix .. "    "
    sb:appendLine(prefix .. "- GameObject:" .. self.name)
    local components = self._components
    for i, component in ipairs(components) do
        component:getSummaryInfo(sb, subPrefix)
    end
end

function _error(self, title, msg)
    local errorMsg = "GameObject." .. title .. " on [" .. self:toString() .. "]" .. " occurred error: " .. msg
    Console.error(errorMsg)
end

function start(self)
    if self._started then
        return
    end

    self._started = true
    self:_sendMessage("start")
end

function _destroyComponents(self)
    local components = self._components
    for i, component in ipairs(components) do
        component:_destroy()
    end
    self._components = nil
    self._componentLookup = nil
end

function _destroyChildren(self)
    local children = self._children
    if nil == children then
        return
    end

    self._children = nil

    for i, child in ipairs(children) do
        children:destroy()
    end
end

function destroy(self)
    self:_destroyChildren()
    self:_destroyComponents()
    self:setParent(nil)
    self._started = false

    setmetatable(self, nil)
end

function getActiveSelf(self)
    return self._active
end

function getActiveInHierarchy(self)
    return self._activeInHierarchy
end

function _updateActiveInHierarchy(self, parentActiveInHierarchy)
    if nil == parentActiveInHierarchy then
        parentActiveInHierarchy = true
        if self._parent ~= nil then
            parentActiveInHierarchy = self._parent:getActiveInHierarchy()
        end
    end

    local activeInHierarchy = parentActiveInHierarchy and self._active

    if self._activeInHierarchy == activeInHierarchy then
        return
    end

    self._activeInHierarchy = activeInHierarchy

    local children = self._children
    if children ~= nil then
        for i, child in ipairs(children) do
            child:_updateActiveInHierarchy(activeInHierarchy)
        end
    end

    local components = self._components
    for i, component in ipairs(components) do
        component:_updateActiveInHierarchy(activeInHierarchy)
    end
end

function setActive(self, value)
    if value == self._active then
        return
    end

    self._active = value
    self:_updateActiveInHierarchy()
end

function addComponent(self, componentType)
    local newComponent
    if type(componentType) == "table" then
        newComponent = componentType:new()
    elseif type(componentType) == "string" then
        newComponent = ClassFactory.create(componentType)
    else
        self:_error("addComponent", debug.traceback())
        return nil
    end

    if nil == newComponent then
        self:_error("addComponent", " Component(" .. className .. ") do not existed!")
        return nil
    end

    self._componentLookup[newComponent.__classname] = newComponent
    table.insert(self._components, newComponent)

    newComponent:_awake(self)

    return newComponent
end

function getComponent(self, componentType)
    local className
    if type(componentType) == "table" then
        className = componentType.__classname
    elseif type(componentType) == "string" then
        className = componentType
    end

    if nil == className then
        self:_error("getComponent", debug.traceback())
        return nil
    end

    return self._componentLookup[className]
end

function getComponentInChildren(self, componentType)
    local className
    if type(componentType) == "table" then
        className = componentType.__classname
    elseif type(componentType) == "string" then
        className = componentType
    end

    if nil == className then
        self:_error("getComponentInChildren", debug.traceback())
        return nil
    end

    return self:_getComponentInChildren(className);
end

function _getComponentInChildren(self, className)
    local children = self._children
    if children ~= nil then
        for i, child in ipairs(children) do
            local component = child:_getComponentInChildren(className)
            if component ~= nil then
                return component
            end
        end
    end

    return self._componentLookup[className]
end

function getComponentInParent(self, componentType)
    local className
    if type(componentType) == "table" then
        className = componentType.__classname
    elseif type(componentType) == "string" then
        className = componentType
    end

    if nil == className then
        self:_error("getComponentInParent", debug.traceback())
        return nil
    end

    local current = self

    while current ~= nil do
        local component = current._componentLookup[className]
        if component ~= nil then
            return component
        end
        current = current._parent
    end

    return nil
end

function getComponents(self, componentType)
    local className
    if type(componentType) == "table" then
        className = componentType.__classname
    elseif type(componentType) == "string" then
        className = componentType
    end

    if nil == className then
        self:_error("getComponents", debug.traceback())
        return nil
    end

    local result = {}
    self:_getComponents(className, result)
    return result
end

function _getComponents(self, className, result)
    local components = self._components
    for i, component in ipairs(components) do
        if component.__classname == className then
            table.insert(result, component)
        end
    end
end

function getComponentsInChildren(self, componentType, includeInactive)
    local className
    if type(componentType) == "table" then
        className = componentType.__classname
    elseif type(componentType) == "string" then
        className = componentType
    end

    if nil == className then
        self:_error("getComponentsInChildren", debug.traceback())
        return nil
    end

    local result = {}
    self:_getComponentsInChildren(className, includeInactive, result)
    return result
end

function _getComponentsInChildren(self, className, includeInactive, result)
    local children = self._children
    if children ~= nil then
        for i, child in ipairs(children) do
            child:_getComponentsInChildren(className, includeInactive, result)
        end
    end

    if (includeInactive == true) or self._active then
        self:_getComponents(className, result)
    end
end

function getComponentsInParent(self, componentType, includeInactive)
    local className
    if type(componentType) == "table" then
        className = componentType.__classname
    elseif type(componentType) == "string" then
        className = componentType
    end

    if nil == className then
        self:_error("getComponentsInParent", debug.traceback())
        return nil
    end

    local result = {}

    local current = self
    while current ~= nil do
        if (includeInactive == true) or current._active then
            current:_getComponents(className, result)
        end

        current = current._parent
    end

    return result
end

function _sendMessage(self, methodName, ...)
    if nil == methodName or string.len(methodName) == 0 then
        self:_error("_SendMessageToAll", debug.traceback())
        return
    end

    local components = self._components
    for i, component in ipairs(components) do
        if component.gameObject == nil then
            break
        end
        if component._receiveMessage then
            component:_receiveMessage(methodName, ...)
        else
            self:_error("_sendMessage",tostring(component.__classname)  .. "._ReceiveMessage method is nil")
        end
    end
end

function sendMessage(self, methodName, ...)
    if not self._active then
        return
    end

    return self:_sendMessage(methodName, ...)
end

function sendMessageUpwards(self, methodName, ...)
    local current = self
    while current ~= nil do
        current:sendMessage(methodName, ...)
        current = current._parent
    end
end

function broadcastMessage(self, methodName, ...)
    if not self._active then
        return
    end

    self:sendMessage(methodName, ...)

    local children = self._children
    if children ~= nil then
        for i, child in ipairs(children) do
            child:sendMessage(methodName, ...)
        end
    end
end

function getParent(self)
    return self._parent
end

function setParent(self, value)
    if value == self._parent then
        return
    end

    if self._parent ~= nil then
        self._parent:_detachChild(self)
    end

    self._parent = value

    if self._parent ~= nil then
        self._parent:_attachChild(self)
    end

    self:_updateActiveInHierarchy()
end

function _detachChild(self, child)
    local children = self._children
    if nil == children then
        return
    end

    for i, _child in ipairs(children) do
        if _child == child then
            table.remove(i)
        end
    end
end

function _attachChild(self, child)
    local children = self._children
    if nil == children then
        self._children = {}
    end

    table.insert(children, child)
end

function detachChildren(self)
    local children = self._children
    if nil == children then
        return
    end

    self._children = nil

    for i, child in ipairs(children) do
        child:setParent(nil)
    end
end

function find(self, name)
    if nil == name then
        return nil
    end

    local paths = string.split(name, "/")

    local current = self
    for i, path in ipairs(paths) do
        current = current:_find(path)
        if nil == current then
            return nil
        end
    end

    if current ~= self then
        return current
    end

    return nil
end

function _find(self, name)
    local children = self._children
    if nil == children then
        return
    end

    for i, child in ipairs(children) do
        if child.name == name then
            return child
        end
    end

    return nil
end

function getChild(self, index)
    if nil == index then
        return nil
    end

    return self._children[index]
end

function getChildCount(self)
    local children = self._children
    if nil == children then
        return 0
    end

    return #children
end

function isChildOf(self, parent)
    local current = self
    while current ~= nil do
        if current == parent then
            return true
        end
        current = current._parent
    end
    return false
end

function getSiblingIndex(self)
    local parent = self._parent
    if nil == parent then
        return 0
    end

    local siblings = parent._children
    local siblingCount = #siblings

    for i, sibling in ipairs(siblings) do
        if sibling == self then
            return i
        end
    end

    return 0
end

function setSiblingIndex(self, index)
    local parent = self._parent
    if nil == parent then
        return
    end

    local siblings = parent._children
    local siblingCount = #siblings
    if index <= 0 or index > siblingCount then
        return
    end

    for i, sibling in ipairs(siblings) do
        if sibling == self then
            table.remove(i)
        end
    end

    table.insert(siblings, index, self)
end

function setAsFirstSibling(self)
    self:setSiblingIndex(1)
end

function setAsLastSibling(self)
    local parent = self._parent
    if nil == parent then
        return
    end

    local siblings = parent._children
    local siblingCount = #siblings
    self:setSiblingIndex(siblingCount)
end

function destroyComponent(self, targetComponent)
    local components = self._components
    for i, component in ipairs(components) do
        if targetComponent == component then
            table.removeItem(self._components, component)
            self._componentLookup[component.__classname] = nil
            component:_destroy()
        end
    end
end