module("ComponentManager", package.seeall)

local updateComponents = {}
local lateUpdateComponents = {}

function _doUpdate(component, deltaTime)
    local ret = component:update(deltaTime)
    return ret
end

function update(deltaTime)
    return SteadyArray.foreach(updateComponents, _doUpdate, deltaTime)
end

function _doLateUpdate(component, deltaTime)
    local ret = component:lateUpdate(deltaTime)
    return ret
end

function lateUpdate(deltaTime)
    return SteadyArray.foreach(lateUpdateComponents, _doLateUpdate, deltaTime)
end

function addUpdate(component)
    if component.update ~= nil then
        SteadyArray.add(updateComponents, component)
    end

    if component.lateUpdate ~= nil then
        SteadyArray.add(lateUpdateComponents, component)
    end
end

function removeUpdate(component)
    if component.update ~= nil then
        SteadyArray.remove(updateComponents, component)
    end

    if component.lateUpdate ~= nil then
        SteadyArray.remove(lateUpdateComponents, component)
    end
end

function _doPrintUpdate(component, count, sb, prefix)
    count.Value = count.Value + 1
    sb:appendLine(prefix .. "- " .. component:toString())
end

function printUpdate()
    local sb = StringBuilder:new()
    local prefix = "    "
    local count = {}

    sb:appendLine("")

    count.Value = 0
    sb:appendLine("- Component.Update:")
    SteadyArray.foreach(updateComponents, _doPrintUpdate, count, sb, prefix)
    if count.Value == 0 then
        sb:appendLine(prefix .. "- Empty")
    end

    count.Value = 0
    sb:appendLine("- Component.LateUpdate:")
    SteadyArray.foreach(lateUpdateComponents, _doPrintUpdate, count, sb, prefix)
    if count.Value == 0 then
        sb:appendLine(prefix .. "- Empty")
    end

    Console.info(sb:toString())
end
