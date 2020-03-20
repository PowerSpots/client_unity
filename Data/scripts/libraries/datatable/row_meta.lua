module("DataTable", package.seeall)

RowMeta = {}

local function contain(self, key)
    local index = getmetatable(self).__dataMeta[key]
    if nil == index then
        return false
    end
    return true
end

local function clone(rowData)
    local newRowData = {}

    local tableMeta = getmetatable(rowData).__dataMeta
    for fieldName, _ in pairs(tableMeta) do
        if fieldName ~= "defaultMeta" then
            newRowData[fieldName] = rowData[fieldName]
        end
    end

    return newRowData
end

local function bitCount(n)
    n = ((n & 0xAAAAAAAAAAAAAAAA) >> 1) + (n & 0x5555555555555555)
    n = ((n & 0xCCCCCCCCCCCCCCCC) >> 2) + (n & 0x3333333333333333)
    n = ((n & 0xF0F0F0F0F0F0F0F0) >> 4) + (n & 0x0F0F0F0F0F0F0F0F)
    n = ((n & 0xFF00FF00FF00FF00) >> 8) + (n & 0x00FF00FF00FF00FF)
    n = ((n & 0xFFFF0000FFFF0000) >> 16) + (n & 0x0000FFFF0000FFFF)
    n = ((n & 0xFFFFFFFF00000000) >> 32) + (n & 0x00000000FFFFFFFF)
    return n
end

function RowMeta.create(tablename, dataMeta)
    local newRowMeta = {}
    newRowMeta.__tableName = tablename
    newRowMeta.__dataMeta = dataMeta
    newRowMeta.contain = contain
    newRowMeta.clone = clone

    newRowMeta.__index = RowMeta.__index
    newRowMeta.__newindex = RowMeta.__newindex
    newRowMeta.__tostring = RowMeta.__tostring
    return newRowMeta
end

function RowMeta.__error(rowData, title, msg)
    local rowMeta = getmetatable(rowData)
    Console.error("Row." .. title .. " on [" .. rowMeta.__tableName .. "]" .. " occurred error: " .. msg)
end

function getFromCompressedRow(rowData, originalIndex, defaultMeta)
    local len = #rowData
    local flag_1_64 = rowData[len]
    local flag_64_128 = rowData[len - 1]
    local actualIndex

    if originalIndex < 65 then
        if flag_1_64 & (1 << (originalIndex - 1)) == 0 then
            return rawget(defaultMeta, originalIndex)
        end

        local bitMask = 0xffffffffffffffff & (0xffffffffffffffff >> (64 - originalIndex))
        flag_1_64 = flag_1_64 & bitMask
        actualIndex = bitCount(flag_1_64)
    elseif originalIndex < 129 then
        if flag_64_128 & (1 << (originalIndex - 65)) == 0 then
            return rawget(defaultMeta, originalIndex)
        end

        local bitMask = 0xffffffffffffffff & (0xffffffffffffffff >> (128 - originalIndex))
        flag_64_128 = flag_64_128 & bitMask
        actualIndex = bitCount(flag_1_64) + bitCount(flag_64_128)
    else
        assert(false, "error, originalIndex must not be more than 128!")
    end
    return rawget(rowData, actualIndex)
end

function RowMeta.__index(rowData, key)
    if nil == key then
        RowMeta.__error(rowData, "get", debug.traceback("key is nil"))
        return nil
    end

    local dataMeta = getmetatable(rowData).__dataMeta
    local defaultMeta = dataMeta.defaultMeta
    local index = dataMeta[key]

    if nil == index then
        if getmetatable(rowData)[key] ~= nil then
            return getmetatable(rowData)[key]
        end
        RowMeta.__error(rowData, "get", debug.traceback("key " .. tostring(key) .. " is not existed!"))
        return nil
    end

    if defaultMeta then
        return getFromCompressedRow(rowData, index, defaultMeta)
    else
        return rawget(rowData, index)
    end
end

function RowMeta.__newindex(rowData, key, value)
    RowMeta.__error(rowData, "set", "can not set the row data!")
end

function RowMeta.__tostring(rowData)
    local result = "{\n"

    local tableMeta = getmetatable(rowData).__dataMeta
    for fieldName, _ in pairs(tableMeta) do
        local fieldValue = rowData[fieldName]
        if type(fieldValue) == 'string' then
            result = result .. fieldName .. " = \"" .. tostring(fieldValue) .. "\",\n"
        else
            result = result .. fieldName .. " = " .. tostring(fieldValue) .. ",\n"
        end
    end

    result = result .. "}"

    return result
end
