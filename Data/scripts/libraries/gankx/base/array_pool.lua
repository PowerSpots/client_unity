module("ArrayPool", package.seeall)

local cachedTables = {}

local initExpandCount = 50
local expandStep = 10
local maxCachedCount = 100

function put(t)
    if nil == t then
        return
    end

    if #cachedTables >= maxCachedCount then
        return
    end

    Array.add(cachedTables, t)

    for k, v in ipairs(t) do
        t[k] = nil
    end
end

function expand(count)
    for index = 1, count do
        local t = {}
        Array.add(cachedTables, t)
    end
end

function get()
    if #cachedTables <= 0 then
        expand(expandStep)
    end

    local lastTable = cachedTables[#cachedTables]
    Array.removeAt(cachedTables, #cachedTables)

    return lastTable
end

function getCachedCount()
    return #cachedTables
end

expand(initExpandCount)