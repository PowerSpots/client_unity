module("Gankx.UI.LoadOperation")

Sync = 0
Async = 1

function fromBool(async)
    if async == true then
        return Async
    end

    return Sync
end

function toBool(operation)
    return operation == Async
end
