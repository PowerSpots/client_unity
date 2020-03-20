module("SystemScope", package.seeall)

local SystemScopeDefine = Gankx.Config.System.define
local SystemScope = SystemScope

local scopeCount = 0
local scopeDesc = { }
local scopeTree = { }

local function loadScope(tree, define)
    local defineName = define.name
    if nil == defineName or string.len(defineName) == 0 then
        return
    end

    tree.value = scopeCount
    scopeCount = scopeCount + 1
    SystemScope[defineName] = tree.value
    print("Dump SystemScope Name:" .. defineName .. " Value:" .. tree.value)
    scopeDesc[tree.value] = defineName

    local subDefines = define.subDefines
    if nil == subDefines then
        return
    end

    print("loadScope name:" .. defineName)
    local subTrees = {}

    for i, subDefine in ipairs(subDefines) do
        local subTree = {}
        loadScope(subTree, subDefine)
        table.insert(subTrees, subTree)
    end

    tree.subScopes = subTrees
end

local function loadScopeByDefine()
    if nil == SystemScopeDefine then
        return
    end

    loadScope(scopeTree, SystemScopeDefine)
end

loadScopeByDefine()

function toString(scope)
    if not isValid(scope) then
        return "invalid"
    end

    return scopeDesc[scope]
end

function isValid(scope)
    if nil == scope or scope < 0 or scope >= scopeCount then
        return false
    end

    return true
end

FindResult = {
    NONE = -1,
    FALSE = 0,
    TRUE = 1,
}

local function findScope(tree, scope, path)
    if nil == tree then
        return FindResult.NONE
    end

    if tree.value ~= scope then
        local subTrees = tree.subScopes
        if nil == subTrees then
            return FindResult.NONE
        end

        for i, subTree in ipairs(subTrees) do
            local result = findScope(subTree, scope, path)
            if result == FindResult.TRUE then
                Array.add(path, tree.value)
                return FindResult.TRUE
            elseif result == FindResult.FALSE then
                Array.add(path, tree.value)
                return FindResult.FALSE
            end
        end

        return FindResult.NONE
    else
        Array.add(path, tree.value)

        local subTrees = tree.subScopes
        if nil ~= subTrees then
            return FindResult.FALSE
        end

        return FindResult.TRUE
    end
end

function findScopeInTree(scope, path)
    local result = findScope(scopeTree, scope, path)
    Array.reverse(path)

    return result
end

local function getSubScopeTree(tree, scope)
    if nil == tree then
        return nil
    end

    if tree.value ~= scope then
        local subTrees = tree.subScopes
        if nil == subTrees then
            return nil
        end

        for i, subTree in ipairs(subTrees) do
            local result = getSubScopeTree(subTree, scope)
            if result ~= nil then
                return result
            end
        end

        return nil
    else
        return tree
    end
end

local function getChildren(tree, children)
    if nil == tree then
        return
    end

    Array.add(children, tree.value)

    if tree.value ~= scope then
        local subTrees = tree.subScopes
        if nil == subTrees then
            return
        end

        for i, subTree in ipairs(subTrees) do
            getChildren(subTree, children)
        end
    end
end

function getChildScope(scope, children)
    local subtree = getSubScopeTree(scopeTree, scope)
    getChildren(subtree, children)
end
