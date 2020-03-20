local UIWindowExport = CS.UIWindowExport
local Widget = Gankx.UI.Widget

widget("WindowCachePool", "Window")

local function find(list, window)
    for i, v in ipairs(list) do
        if v == window then
            return i
        end
    end

    return nil
end

function pop(self)
    local cacheTemplate = self.context.cacheTemplate
    local cachedList = self.context.cachedList
    local usedList = self.context.usedList
    local scaleVisible = self.context.scaleVisible

    local usedWindow

    local cachedCount = #cachedList
    if cachedCount > 0 then
        local cachedWindow = cachedList[1]
        table.remove(cachedList, 1)
        if scaleVisible then
            cachedWindow.scaleVisible = true
        else
            cachedWindow.Visible = true
        end
        usedWindow = cachedWindow
    else
        local allCount = cachedCount + #usedList
        if self.maxCount == nil or self.maxCount > allCount then
            if cacheTemplate ~= nil then
                local newWindow = cacheTemplate:clone()
                if scaleVisible then
                    newWindow.scaleVisible = true
                else
                    newWindow.Visible = true
                end
                usedWindow = newWindow
            end
        end
    end

    if usedWindow ~= nil then
        table.insert(usedList, usedWindow)
    end

    return usedWindow
end

function push(self, window)
    if nil == window then
        return
    end

    local cachedList = self.context.cachedList
    local usedList = self.context.usedList

    local found = find(cachedList, window)
    if found ~= nil then
        return
    end

    found = find(usedList, window)
    if found ~= nil then
        table.remove(usedList, found)
    end

    local scaleVisible = self.context.scaleVisible
    if scaleVisible then
        window.scaleVisible = false
    else
        window.Visible = false
    end

    table.insert(cachedList, window)
end

local DefaultSetting = {}

function onInit(self, setting)
    self.context.cacheTemplate = nil
    self.context.cachedList = {}
    self.context.usedList = {}
    self.context.maxCount = nil
    self.context.scaleVisible = setting.scaleVisible

    local cacheSetting = setting.cacheSetting or DefaultSetting
    self.context.cacheTemplate = Widget.createBySetting("WindowCacheTemplate",
            self.__parent, cacheSetting)
end

function onLoad(self)
    local setting = self.__setting
    local cacheTemplate = self.context.cacheTemplate
    local cachedList = self.context.cachedList
    local usedList = self.context.usedList
    local scaleVisible = self.context.scaleVisible

    local childList = {}
    local childCount = UIWindowExport.GetChildCount(self.id)
    for i = 1, childCount do
        local childId = UIWindowExport.GetChildAt(self.id, i - 1)
        if childId > 0 then
            if not cacheTemplate:isBound() then
                if scaleVisible then
                    cacheTemplate.Visible = true
                    cacheTemplate.ScaleVisible = false
                else
                    cacheTemplate.Visible = false
                end

                cacheTemplate:bind(childId)
            else
                table.insert(childList, childId)
            end
        end
    end

    local cacheSetting = setting.cacheSetting or DefaultSetting
    if not cacheTemplate:isBound() then
        self:__error("onLoad", "can not bind cache type '" .. tostring(cacheSetting.type or "Window") .. "' maybe there is not template child")
        return
    end

    local leftCount = #cachedList + #usedList - #childList
    if leftCount > 0 then
        for i = 1, leftCount do
            table.insert(childList, UIWindowExport.clone(cacheTemplate.id))
        end
    end

    local index = 1
    for i, childWindow in ipairs(cachedList) do
        childWindow:bind(childList[index])
        if scaleVisible then
            childWindow.Visible = true
            childWindow.ScaleVisible = false
        else
            childWindow.Visible = false
        end
        index = index + 1
    end

    for i, childWindow in ipairs(usedList) do
        childWindow:bind(childList[index])
        if scaleVisible then
            childWindow.Visible = true
            childWindow.ScaleVisible = true
        else
            childWindow.Visible = true
        end

        index = index + 1
    end

    for i = index, #childList do
        local childWindow = Widget.createBySetting("WindowCacheTemplate",
                self.__parent, cacheSetting)
        childWindow:bind(childList[index])
        self:push(childWindow)
        index = index + 1
    end
end

function onUnload(self)
    self.context.cacheTemplate:unbind()

    for _, window in ipairs(self.context.usedList) do
        window:unbind()
    end

    for _, window in ipairs(self.context.cachedList) do
        window:unbind()
    end
end

function reserve(self, count)
    local cacheTemplate = self.context.cacheTemplate
    local cachedList = self.context.cachedList

    local reserveCount = math.min(count, self.maxCount)
    for i = 1, reserveCount do
        if cacheTemplate ~= nil then
            local newWindow = cacheTemplate:clone()

            table.insert(cachedList, newWindow)
        end
    end
end

function setMaxCount(self, count)
    self.context.maxCount = count or 10
end

function getMaxCount(self)
    return self.context.maxCount or 10
end

function getUsedList(self)
    return self.context.usedList
end

function setTemplate(self, setting)
    self.context.cacheTemplate = Widget.createBySetting("WindowCacheTemplate",
            self.__parent, setting)
end