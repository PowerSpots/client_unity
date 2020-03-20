module("Gankx.File", package.seeall)

local _readsysfile = CS.Gankx.LuaExport.ReadSysFile
_G.readsysfile = nil

local _writesysfile = CS.Gankx.LuaExport.WriteSysFile
_G.writesysfile = nil

local _fileexists = CS.Gankx.LuaExport.FileExists

local _writeAppFile = CS.Gankx.LuaExport.WriteAppFile

function readAllBytes(fileName)
    return _readsysfile(fileName)
end

function writeAllBytes(fileName, bytes)
    return _writesysfile(fileName, bytes)
end

function exists(fileName)
    return _fileexists(fileName)
end

function writeAppFile(fileName, content)
    _writeAppFile(fileName, content)
end