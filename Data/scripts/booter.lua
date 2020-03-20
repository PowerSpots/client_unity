local __loadfile = _G.loadfile
_G.loadfile = nil
local __fileexists = CS.Gankx.LuaExport.FileExists
_G.fileexists = nil
local __booterror = CS.Gankx.LuaExport.BootError
_G.booterror = nil
local __getsysfiles = CS.Gankx.LuaExport.GetSysFiles

local isDebug = CS.Gankx.LuaExport.IsDebug
local setBackgroundLoadingPriority = CS.Gankx.LuaExport.SetBackgroundLoadingPriority

module("Booter", package.seeall)

paths = {
    base = "scripts/",
    config = "scripts/config/",
    libraries = "scripts/libraries/",
    source = "scripts/source/",
}

function loadfile(file)
    if nil == file then
        return nil, "file path is nil"
    end

    return __loadfile(file)
end

currentFilePath = ""

function dofile(fileName, required)
    if nil == fileName then
        return false, "file path is nil"
    end

    if false == required then
        local exists = __fileexists(fileName)
        if not exists then
            return false, "cannot find file '" .. fileName .. "'"
        end
    end

    local func, errorMsg = loadfile(fileName)
    local status = false

    if func ~= nil then
        currentFilePath = fileName
        status, errorMsg = pcall(func)
        currentFilePath = ""
    end

    if not status then
        local file = fileName or ""
        __booterror("load file '" .. file .. "' occurred error: " .. errorMsg)
    end

    return status, errorMsg
end

local function strTrimAndLower(s)
    s = string.lower(s)
    return s:gsub("^%s*(.-)%s*$", "%1", 1)
end

local function strSplit(s, sep)
    sep, fields = sep or ":", {}
    local pattern = string.format("([^%s]+)", sep)
    s:gsub(pattern, function(c)
        fields[#fields + 1] = c
    end)
    return fields
end

local function pathNormalize(path)
    path = string.lower(path)
    path = table.concat(strSplit(path, "\\/"), "/")
    if string.len(path) > 0 then
        return path .. "/"
    else
        return ""
    end
end

local pathSperator = "/"

local slicedFiles = {}
local slicedIndex = 0
local loadedFiles = {}
local doPackageFile

local function checkRelease(v)
    if v.debug ~= true then
        return true
    end

    return isDebug() or (not RELEASE)
end

local function doSlicedFiles()
    loadedFiles = {}

    setBackgroundLoadingPriority(4)

    for i = 1, #slicedFiles do
        doPackageFile(slicedFiles[i])
        local res = math.fmod(i, 200)
        if res == 0 then
            Gankx.Coroutine.yieldNull()
        end
        slicedIndex = i
    end

    setBackgroundLoadingPriority(2)
end

local function getSlicedProgress()
    if slicedFiles == nil or #slicedFiles == 0 then
        return 0
    end

    local p = slicedIndex / #slicedFiles
    Console.info("getSlicedProgress " .. p)
    return p
end

doPackageFile = function(filePath, required, sliced)
    if nil == filePath then
        return
    end

    if loadedFiles[filePath] ~= nil then
        return
    end

    if sliced == true then
        table.insert(slicedFiles, filePath)
        return
    end

    local status, errorMsg = dofile(filePath, required)
    loadedFiles[filePath] = status
end

local function doPackageAllFiles(path, sliced)
    local files = __getsysfiles(path, "*.lua", true)
    if nil == files or #files == 0 then
        return
    end

    for i, file in ipairs(files) do
        doPackageFile(path .. file, true, sliced)
    end
end

local function doPackageRecursively(packageDefine, path, sliced)
    if nil == packageDefine then
        doPackageAllFiles(path, sliced)
        return
    end

    for i, v in ipairs(packageDefine) do
        if type(v) == "string" then
            local fp = path .. strTrimAndLower(v)
            -- print("doPackageFile:" .. fp)
            doPackageFile(fp, true, sliced)
        elseif type(v) == "table" and v.name ~= nil then
            local dirPath = path .. strTrimAndLower(v.name) .. pathSperator
            if checkRelease(v) then
                if v.package == true then
                    doPackage(dirPath, sliced)
                else
                    doPackageRecursively(v, dirPath, sliced)
                end
            end
        end
    end
end

packageDefine = nil

function doPackage(path, sliced)
    if nil == path then
        return
    end

    path = pathNormalize(path)

    packageDefine = nil

    doPackageFile(path .. "package.lua", false)
    doPackageRecursively(packageDefine, path, sliced)
end

function reboot()
    loadedFiles = {}

    setBackgroundLoadingPriority(4)
    doPackage(paths.config)
    doPackage(paths.libraries)
    setBackgroundLoadingPriority(2)

    slicedFiles = {}
    doPackage(paths.source, true)

    local littleBooter = {}
    littleBooter["doSlicedFiles"] = doSlicedFiles
    Gankx.Coroutine.start(littleBooter, "doSlicedFiles")
end

_G.BootComplete = getSlicedProgress

reboot()
