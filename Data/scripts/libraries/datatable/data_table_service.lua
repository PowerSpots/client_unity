local Directory = Gankx.Directory
local Table = DataTable.Table

module("DataTable", package.seeall)

_G.TableDataSet = {}
_G.TableDefine = nil

local dataTablePath = Config.DataTablePath
local dataConfigPath = Config.DataConfigPath
local dataTableDic

local errfunc = function(err)
    return debug.traceback(err)
end

local function error(title, msg)
    Console.error("DataTable." .. title .. " occurred error: " .. tostring(msg))
end

local function loadEnumDefine()
    local sourceFile = dataTablePath .. "GAME_ENUM_DEFINE.lua"
    bOk, errMsg = Booter.dofile(sourceFile)
    if not bOk then
        error("loadEnumDefine", "load GAME_ENUM_DEFINE.lua error: " .. errMsg)
    end
end

loadEnumDefine()

local function loadDataConfig()
    local files = Directory.getFiles(dataConfigPath, "lua", true)
    for _, v in ipairs(files) do
        if v ~= sharedConfigPath then
            local bOk, errMsg = Booter.dofile(v)
            if not bOk then
                error("loadDataConfig", "load config '" .. v .. "' error: " .. errMsg)
            end
        end
    end
end

loadDataConfig()

local function loadTableDefine()
    local sourceFile = dataTablePath .. "TABLE_FIELD_DEFINE.lua"
    local bOk, errMsg = Booter.dofile(sourceFile)
    if not bOk then
        error("loadTableDefine", "load TABLE_FIELD_DEFINE.lua error: " .. errMsg)
    end
end

loadTableDefine()

local function createTableDic(dataTableName, dataTableMeta, dataTableFile)
    local bOk, errMsg = Booter.dofile(dataTableFile)

    if not bOk then
        error("createTableDic", "load table '" .. dataTableFile .. "' error: " .. errMsg)
    end

    if nil == TableDataSet[dataTableName] then
        error("createTableDic", "table '" .. dataTableName .. "' is not defined in TableDataSet")
        return
    end

    if nil ~= dataTableDic[dataTableName] then
        error("createTableDic", "table '" .. dataTableName "' already loaded")
        return
    end

    local tableConvertFunc = TableConvertConfig[dataTableName]
    if nil == tableConvertFunc then
        tableConvertFunc = TableConvertConfig["__default"]
    end

    if nil == tableConvertFunc then
        error("createTableDic", "cannot find index register function, table name: " .. dataTableName)
        return
    end

    local bOK, data = xpcall(tableConvertFunc, errfunc, _G.TableDataSet[dataTableName], dataTableName, dataTableMeta)
    if not bOK then
        error("CreateTableDic", "table convert func occured error, table name: " .. dataTableName .. "\n" .. data)
    else
        local dataTable = Table:new(dataTableName, dataTableMeta, data)
        dataTableDic[dataTableName] = dataTable
    end
end

local paramPrefixList = {
    "L", "R", "N", "S", "D", "F", "G", "X"
}

local function addParamPrefixToEnv()
    for index = 0, 32 do
        for _, paramName in ipairs(paramPrefixList) do
            _G[paramName .. index] = 10000
        end
    end

    _G.RATE_MAX = 10000
end

local function removeParamPrefixFromEnv()
    for index = 0, 32 do
        for _, paramName in ipairs(paramPrefixList) do
            _G[paramName .. index] = nil
        end
    end

    _G.RATE_MAX = nil
end

local function loadTableData()
    if nil == TableDefine then
        error("loadTableData", "TableDefine is nil")
        return
    end

    addParamPrefixToEnv()

    dataTableDic = {}

    for key, value in pairs(TableDefine) do
        local tableDataFile = dataTablePath .. value.file
        value.meta.defaultMeta = value.defaultMeta
        createTableDic(key, value.meta, tableDataFile)
    end

    removeParamPrefixFromEnv()

    _G.TableDataSet = nil
end

loadTableData()

function get(tableName)
    if nil == tableName then
        error("get", debug.traceback("tableName is nil"))
        return
    end

    if dataTableDic[tableName] == nil then
        error("get", "table '" .. tableName .. "' is not defined")
        return
    end

    return dataTableDic[tableName]
end
