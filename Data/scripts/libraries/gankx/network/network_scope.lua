module("NetworkScope", package.seeall)

Dir = 0
Game = 1
Battle = 2
Count = 3

local scopeDesc =
{
    [Dir] = "Dir",
    [Game] = "Game",
    [Battle] = "Battle",
}

function toString(scope)
    if not isValid(scope) then
        return "invalid"
    end

    return scopeDesc[scope]
end

function isValid(scope)
    if nil == scope or scope < 0 or scope >= Count then
        return false
    end

    return true
end
