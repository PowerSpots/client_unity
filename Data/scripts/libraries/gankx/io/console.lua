local DebugExport = CS.DebugExport
local Path = Gankx.Path

module("Console", package.seeall)

local enabled = true
local tempPrint = print

function gPrint(...)
  tempPrint(..., format(2))
end

_G.print = gPrint

function format(depth) 
  return  "\n\n <filePath>" .. debug.getinfo(depth+1).short_src .. "</filePath><line>" .. debug.getinfo(depth+1).currentline .. "</line>"
end

function print(message, depth)
    if not enabled then
        return
    end
    
    if depth == nil then
      depth = 3
    else
      depth = depth + 1
    end
    
    message = tostring(message)
    message = "lua: " .. message

    tempPrint(message, format(depth))
end

function error(message, depth)
    if depth == nil then
      depth = 2
    else
      depth = depth + 1
    end

    message = tostring(message)
    message = "lua: " .. message .. format(depth)

    if DebugExport ~= nil then
        DebugExport.LogError(message)
    else
        gPrint(message)
    end
end

function warning(message, depth)
    if not enabled then
        return
    end
    if depth == nil then
      depth = 2
    else
      depth = depth + 1
    end
    
    message = tostring(message)
    message = "lua: " .. message .. format(depth)

    if DebugExport ~= nil then
        DebugExport.LogWarning(message)
    else
        gPrint(message)
    end
end

function info(message, depth)
    if not enabled then
        return
    end
    
    if depth == nil then
      depth = 2
    else
      depth = depth + 1
    end

    message = string.format("Lua[%.3f]%s%s", Application.totalTime, tostring(message), format(depth))

    if DebugExport ~= nil then
        DebugExport.Log(message)
    else
        gPrint(message)
    end
end

function infoFormat(...)
    info(string.format(...))
end

function setEnable(enabled)
    enabled = enabled or false
    enabled = enabled
end

function clear()
    if DebugExport ~= nil then
        DebugExport.ClearLog()
    end
end

local function startup()
    file_path = Gankx.Config.Debug.configpath, "console.lua"
    Booter.dofile(file_path, false)
end

startup()
