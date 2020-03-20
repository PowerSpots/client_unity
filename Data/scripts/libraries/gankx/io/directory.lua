local Path = Gankx.Path

module("Gankx.Directory", package.seeall)

local _getsysfiles = CS.Gankx.LuaExport.GetSysFiles
_G.getsysfiles = nil

local _createsysdirectory = CS.Gankx.LuaExport.CreateSysDirectory
_G.createsysdirectory = nil

function getFiles(filePath, ext, recursive, withpath)
    if nil == filePath then
        return
    end

    filePath = Path.normalize(filePath)

    if nil == recursive then
        recursive = false
    end

    if nil == withpath then
        withpath = true
    end

    local searchPattern = ext or ""
    searchPattern = "*." .. string.lower(ext)

    local files = _getsysfiles(filePath, searchPattern, recursive) or {}
    if withpath then
        for i, v in ipairs(files) do
            files[i] = filePath .. v
        end
    end

    return files
end

function createDirectory(filePath)
    if nil == filePath then
        return
    end

    filePath = Path.normalize(filePath)
    _createsysdirectory(filePath)
end
