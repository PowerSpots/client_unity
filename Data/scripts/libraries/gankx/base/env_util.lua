module("EnvUtil", package.seeall)

local function searchUpvalue(func, search_name)
    if nil == func then
        print(debug.traceback("Error: SearchUpvalue param(func) is nil"))
        return
    end

    if nil == search_name or "" == search_name then
        print(debug.traceback("Error: SearchUpvalue param(search_name) is nil or empty"))
        return
    end

    local index = 1
    while true do
        local name, val = debug.getupvalue(func, index)
        if nil == name then
            break
        end
        if name == search_name then
            return index, val
        end
        index = index + 1
    end

    return nil
end

function getFuncEnv(func)
    if func == nil or type(func) ~= "function" then
        print(debug.traceback("Error: GetFuncEnv param(func) is nil"))
        return
    end

    local index, val = searchUpvalue(func, "_ENV")
    return val
end


local function setFuncEnv(func, env)
    if func == nil or type(func) ~= "function" then
        print(debug.traceback("Error: SetFuncEnv param(func) is nil"))
        return
    end

    if env == nil then
        print(debug.traceback("Error: SetFuncEnv param(env) is nil"))
        return
    end

    local index = searchUpvalue(func, "_ENV")
    if index == nil then
        print(debug.traceback("Error: SetFuncEnv _Env is miss"))
        return
    end

    debug.upvaluejoin(func, index, (function() return env end), 1)
    return func
end

function getChunkEnv(level)
    if level == nil or type(level) ~= "number"  then
        print(debug.traceback("Error: GetChunkEnv level is nil"))
        return
    end

    local info = debug.getinfo(level + 1, "f")
    if (info.func == nil) then
        print(debug.traceback("Error:GetChunkEnv info.func miss"))
        return
    end

    return info.func
end

function getEnv(level)
    if level == nil or type(level) ~= "number" then
        print(debug.traceback("Error: GetEnv param(level) is nil"))
        return
    end

    local func = getChunkEnv(level + 1)
    if nil == func then
        return
    end

    return getFuncEnv(func)
end

function setEnv(level, env)
    if level == nil or type(level) ~= "number" then
        print(debug.traceback("Error: SetEnv param(level) is nil"))
        return
    end

    if env == nil then
        print(debug.traceback("Error: SetEnv param(env) is nil"))
        return
    end

    local func = getChunkEnv(level + 1)
    if nil == func then
        print("______________error")
        return
    end

    return setFuncEnv(func, env)
end



