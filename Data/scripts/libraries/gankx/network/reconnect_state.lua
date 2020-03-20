module("ReconnectState", package.seeall)

Running = 1
Waiting = 2
Connecting = 3
Failed = 4

local reconnectorMap =
{
    [Running] = { [Running] = true, [Connecting] = true, },
    [Waiting] = { [Connecting] = true,[Running] = true, },
    [Connecting] = { [Running] = true, [Waiting] = true, [Failed] = true, },
    [Failed] = { [Waiting] = true, },
}


local stateDesc =
{
    [Running] = "Running",
    [Waiting] = "Waiting",
    [Connecting] = "Connecting",
    [Failed] = "Failed",
}

function canEnter(from, to)
    if nil == reconnectorMap[from] then
        return false
    end

    return reconnectorMap[from][to] or false
end

function toString(state)
    return stateDesc[state] or "Invalid"
end

