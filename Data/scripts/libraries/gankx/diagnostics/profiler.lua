local LuaProfilerExport =  CS.LuaProfilerExport

module("Profiler", package.seeall)

local enabled = false

function setEnable(enable)
    enabled = enable
end

function beginSample(sampleName)
    if not enabled then return end
    local stringId = StringService.instance:getStringId(sampleName)
    LuaProfilerExport.BeginSample(stringId)
end

function endSample()
    if not enabled then return end
    LuaProfilerExport.EndSample(stringId)
end

stack = {}

function beginSampleCheck(sampleName)
    if not enabled then return end
    local stringId = StringService.instance:getStringId(sampleName)
    LuaProfilerExport.BeginSample(stringId)
    table.insert(stack,sampleName)
end

function endSampleCheck(sampleName)
    if not enabled then return end
    if stack[#stack] ~= sampleName then
        Console.error(debug.traceback(string.format("No Match sampleName %s %s",sampleName, stack[#stack])))
    end
    table.remove(stack)
    LuaProfilerExport.EndSample()
end

testflag = false

Tags = {
    NetworkTag = 1,
    UpdateTag = 2,
}

local alloweTags = {}

function setAllowTags(...)
    local targetTags = {...}
    alloweTags = {}
    for targetTag in pairs(targetTags) do
        local find = false
        for k,v in pairs(Tags) do
            if v == targetTag then
                find = true
            end
        end
        if find == true then
            Array.add(alloweTags,targetTag)
        end
    end
end

function beginTagSample(sampleName, tag)
    if not Array.contains(alloweTags,tag) then return end
    beginSample(sampleName)
end

function endTagSample(sampleName, tag)
    if not Array.contains(alloweTags,tag) then return end
    endSample(sampleName)
end