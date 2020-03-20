local ServiceComponent = Component

module("Application.Services", package.seeall)

local serviceClasses = {}
local serviceClassesMap = {}
local servicesGameObject

function startup()
    Console.info("Application.Services startup " .. #serviceClasses)
    servicesGameObject = GameObject:new("Services")
    for i, serviceClass in ipairs(serviceClasses) do
        serviceClass.instance = servicesGameObject:addComponent(serviceClass)
    end
    servicesGameObject:sendMessage("startup")
end

function shutdown()
    Console.info("Application.Services shutdown")
    servicesGameObject:sendMessage("shutdown")
    for i, serviceClass in ipairs(serviceClasses) do
        serviceClass.instance = nil
    end
    servicesGameObject:destroy()
    servicesGameObject = nil
end

-- function print()
--     local sb = StringBuilder:new()
--     sb:appendLine("")
--     servicesGameObject:getSummaryInfo(sb)
--     Console.info(sb:toString())
-- end

function service(name, partial)
    local fenv = getfenv(2)

    local serviceMeta
    if partial == classPartial then
        serviceMeta = ClassFactory.define(fenv, name, classPartial)
    else
        serviceMeta = ClassFactory.define(fenv, name, ServiceComponent)
    end

    if nil == serviceMeta then
        return
    end

    if serviceClassesMap[name] == nil then
        table.insert(serviceClasses, serviceMeta)
        serviceClassesMap[name] = true
    end

    setfenv(2, serviceMeta)
end

_G.service = service