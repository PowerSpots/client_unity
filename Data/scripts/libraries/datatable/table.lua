local RowMeta = DataTable.RowMeta

module("DataTable", package.seeall)

class("Table")

function constructor(self, name, dataMeta, data)
    self.name = name
    self._rowMeta = RowMeta.create(name, dataMeta)
    self._allData = data
end

function _error(self, title, msg)
    Console.error("Table." .. title .. " on [" .. self.name .. "]" .. " occurred error: " .. msg)
end

function tryGetRow(self, key, key2, key3)
    local tableMeta = getmetatable(self._allData)

    local rowData
    if nil ~= tableMeta and nil ~= tableMeta.___get then
        rowData = tableMeta.___get(self._allData, key, key2, key3)
    else
        if nil == key then
            self:_error("tryGetRow", "key is nil." .. debug.traceback())
            return nil
        end

        rowData = self._allData[key]
    end

    if nil == rowData then
        return nil
    end

    setmetatable(rowData, self._rowMeta)

    return rowData
end

function getRows(self, ...)
    local tableMeta = getmetatable(self._allData)

    local rowDatas
    if nil ~= tableMeta and nil ~= tableMeta.___get then
        rowDatas = tableMeta.___get(self._allData, ...)
    end

    if nil == rowDatas then
        self:_error("getRows", "cannot find data. keys: " .. tostring(...) .. debug.traceback())
        return
    end

    local ret = Util.clone(rowDatas)

    for i, v in ipairs(ret) do
        setmetatable(v, self._rowMeta)
    end

    return ret
end

function tryGetRows(self, ...)
    local tableMeta = getmetatable(self._allData)

    local rowDatas
    if nil ~= tableMeta and nil ~= tableMeta.___get then
        rowDatas = tableMeta.___get(self._allData, ...)
    end

    if nil == rowDatas then
        return nil
    end

    local ret = Util.clone(rowDatas)

    for i, v in ipairs(ret) do
        setmetatable(v, self._rowMeta)
    end

    return ret
end

function getRow(self, key, key2, key3)
    local tableMeta = getmetatable(self._allData)

    if tableMeta and tableMeta.__rows then
        return self:getRows(key, key2, key3)
    end

    local rowData
    if nil ~= tableMeta and nil ~= tableMeta.___get then
        rowData = tableMeta.___get(self._allData, key, key2, key3)
    else
        if nil == key then
            self:_error("getRow", "key is nil." .. debug.traceback())
            return nil
        end

        rowData = self._allData[key]
    end

    if nil == rowData then
        local keysStr = string.substitute("{1},{2},{3}", tostring(key) , tostring(key2) , tostring(key3))

        self:_error("getRow", "cannot find data. keys: " .. keysStr .. debug.traceback())
        return nil
    end

    setmetatable(rowData, self._rowMeta)

    return rowData
end

function getAllRow(self, match, ...)
    local allRow = {}

    local allData = self._allData
    local rowMeta = self._rowMeta
    for k, rowData in pairs(allData) do
        setmetatable(rowData, rowMeta)
        if nil == match or match(rowData, ...) then
            table.insert(allRow, rowData)
        end
    end

    return allRow
end

function getLength(self)
    return #self._allData
end
