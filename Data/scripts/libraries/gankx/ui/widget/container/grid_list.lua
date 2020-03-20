local ContentSizeFitterExport = CS.ContentSizeFitterExport

widget("GridList", "WindowCachePool")

function onInit(self, setting)
    __super.onInit(self, setting)

    self:setMaxCount(500)

    self.context.cellList = {}
    self.context.listData = {}
end

function onLoad(self)
    __super.onLoad(self)

    for index, cell in ipairs(self.context.cellList) do
        cell:setName("Item" .. index)
    end

    local setting = self.__setting
    if false == setting.refreshOnLoad then
        return
    end

    self:refreshListData(self.context.listData)
end

function getCellList(self)
    return self.context.cellList
end

function setSelected(self, cellWindow)
    self:clearSelected()
    cellWindow:setSelected(true)
end

function setSelectedByIndex(self, index)
    if index >= 1 and index <= #self.context.cellList then
        self:clearSelected()
        self.context.cellList[index]:setSelected(true)
    end
end

function clearSelected(self)
    local cellList = self.context.cellList
    for _, cell in pairs(cellList) do
        cell:setSelected(false)
    end
end

function setListData(self, listData)
    listData = listData or {}

    if #listData == self.Count then
        self:refreshListData(listData)
    else
        self:removeAll()
        for _, data in ipairs(listData) do
            self:add(data)
        end
    end
    
    ContentSizeFitterExport.SetLayoutImmediate(self.id)
end

function refreshListData(self, listData)
    for i = 1, #listData do
        self:setCellData(listData[i], i)
    end
end

function add(self, cellData)
    return self:insert(cellData, self.Count + 1)
end

function insert(self, cellData, index)
    if index < 1 or index > self.Count + 1 then
        self:__error("insert", "invalid index")
        return nil
    end

    local newWindow = self:pop()
    if nil == newWindow then
        self:__error("insert", "can not create new")
        return nil
    end

    newWindow:setName("Item" .. index)
    newWindow:setCellData(cellData, index)
    if index == self.Count + 1 then
        self:readdChild(newWindow)
    else
        self:insertChild(newWindow, self.context.cellList[index])
    end

    table.insert(self.context.listData, index, cellData)
    table.insert(self.context.cellList, index, newWindow)
    return index
end

function remove(self, window)
    local index = self:_findWindowIndex(window)
    if nil ~= index then
        table.remove(self.context.cellList, index)
        table.remove(self.context.listData, index)
        window:setName("Item")
        self:push(window)
    else
        return
    end
end

function removeAt(self, index)
    if index < 1 or index > self.Count then
        self:__error("removeAt", "invalid index")
        return
    end

    local cellList = self.context.cellList
    cellList[index]:setName("Item")
    self:push(cellList[index])
    table.remove(cellList, index)
    table.remove(self.context.listData, index)
end

function removeAll(self)
    local cellList = self.context.cellList

    for index = 1, #cellList do
        cellList[index]:setName("Item")
        self:Push(cellList[index])
    end

    self.context.cellList = {}
    self.context.listData = {}
end

function getCount(self)
    return #self.context.listData
end

function getCell(self, index)
    if nil == index or index < 1 or index > self.Count then
        self:__error("getCell", debug.traceback("invalid index: " .. tostring(index)))
        return nil
    end

    return self.context.cellList[index]
end

function setCellData(self, cellData, index)
    if index < 1 or index > self.Count then
        self:__error("setCellData", "invalid index: " .. index)
        return nil
    end

    self.context.cellList[index]:setCellData(cellData, index)
    self.context.listData[index] = cellData
end

function getCellData(self, index)
    return self.context.listData[index]
end

function _findWindowIndex(self, window)
    for i, v in ipairs(self.context.cellList) do
        if v.id == window.id then
            return i
        end
    end
    return nil
end

function move(self, oldIndex, newIndex)
    local cellData = self:getCellData(oldIndex)
    self:removeAt(oldIndex)

    if nil ~= cellData then
        self:insert(cellData, newIndex)
        return self:getCell(newIndex)
    end
end

function callFunction(self, funcName, ...)
    local cellList = self.context.cellList
    for i = 1, #cellList do
        cellList[i]:__callInterface(funcName, ...)
    end
end