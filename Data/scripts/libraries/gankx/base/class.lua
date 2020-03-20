module("ClassFactory", package.seeall)

local function error(title, msg)
    Console.error("ClassFactory." .. title .. " occurred error: " .. msg)
end

local function classnew(class, ...)
    local object = {}
    setmetatable(object, class)
    if object.constructor then
        object:constructor(...)
    end
    return object
end

local function findTable(env, name)
    local fields = string.split(name, ".")
    local curTable = env
    for i, field in ipairs(fields) do
        local newTable = curTable[field]
        if nil == newTable then
            newTable = {}
            curTable[field] = newTable
        end
        curTable = newTable
    end
    return curTable
end

local function getTable(env, name)
    local fields = string.split(name, ".")
    local curTable = env
    for i, field in ipairs(fields) do
        local newTable = curTable[field]
        if nil == newTable then
            return
        end
        curTable = newTable
    end
    return curTable
end

local classPartial = {}

local function defineClass(fenv, classMeta, className, baseClass)
    local baseClassMeta
    if nil == baseClass or "" == baseClass then
        baseClassMeta = { __index = fenv }
        classMeta.__super = nil
        classMeta.__basename = ""
    else
        local baseClassPresent = type(baseClass)
        local baseClassName
        if baseClassPresent == "string" then
            baseClassName = baseClass
            baseClassMeta = findTable(fenv, baseClassName)
        elseif baseClassPresent == "table" and baseClass.__classname ~= nil then
            baseClassName = baseClass.__classname
            baseClassMeta = baseClass
        end

        if nil == baseClassMeta then
            error("define(_G.class)", debug.traceback("Invalid BaseName Type:", 2))
            return nil
        end

        classMeta.__super = baseClassMeta
        classMeta.__basename = baseClassName
    end

    classMeta.__classname = className
    classMeta.__index = classMeta
    classMeta.new = classnew

    setmetatable(classMeta, baseClassMeta)
end

local function defineClassPartial(fenv, classMeta)
    local baseClassMeta = getmetatable(classMeta)
    if nil == baseClassMeta then
        setmetatable(classMeta, { __index = fenv })
    end
end

function define(fenv, className, baseClass)
    if nil == className or "" == className or
            type(className) ~= "string" then
        error("define(_G.class)", debug.traceback("Invalid ClassName Type:", 2))
        return nil
    end

    local classMeta = findTable(fenv, className)

    if baseClass == classPartial then
        defineClassPartial(fenv, classMeta)
    else
        defineClass(fenv, classMeta, className, baseClass)
    end

    return classMeta
end

function create(className, ...)
    if nil == className or "" == className or
            type(className) ~= "string" then
        error("create", debug.traceback("Invalid ClassName Type:", 2))
        return
    end

    local fenv = getfenv(2)
    local classMeta = getTable(fenv, className)
    if nil == classMeta then
        error("create", debug.traceback("Class [" .. className .. "] is not existed!", 2))
        return
    end

    return classMeta:new(...)
end

function _G.class(className, baseName)
    local fenv = getfenv(2)
    local classMeta = define(fenv, className, baseName)
    if classMeta ~= nil then
        setfenv(2, classMeta)
    end

    return classMeta
end

_G.classPartial = classPartial