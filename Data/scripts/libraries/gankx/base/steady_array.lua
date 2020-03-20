module("SteadyArray", package.seeall)

local EmptyItem = {}

function add(array, item)
    if nil == array or nil == item then
        return 0
    end

    local arrayCount = 1
    for i, v in ipairs(array) do
        if v == EmptyItem then
            array[i] = item
            return i
        end
        arrayCount = arrayCount + 1
    end

    array[arrayCount] = item
    return arrayCount
end

function contains(array, item)
    if nil == array or nil == item then
        return false
    end

    for i, v in ipairs(array) do
        if item == v then
            return true
        end
    end

    return false
end

function clear(array, action, ...)
    if nil == array then
        return
    end

    for i, v in ipairs(array) do
        if action ~= nil and v ~= EmptyItem then
            action(v, ...)
        end

        array[i] = nil
    end
end

function indexOf(array, item)
    if nil == array or nil == item then
        return 0
    end

    for i, v in ipairs(array) do
        if item == v then
            return i
        end
    end

    return 0
end

function remove(array, item)
    if nil == array or nil == item then
        return
    end

    for i, v in ipairs(array) do
        if v == item then
            array[i] = EmptyItem
            return
        end
    end
end

function removeAt(array, index)
    if nil == array or nil == index then
        return
    end

    local arrayCount = #array
    if index < 1 or index > arrayCount then
        return
    end

    array[index] = EmptyItem
end

function removeAll(array, match, ...)
    if nil == array then
        return
    end

    for i, v in ipairs(array) do
        if nil == match or match(v, ...) then
            array[i] = EmptyItem
        end
    end
end

function foreach(array, action, ...)
    if nil == array or nil == action then
        return
    end

    for i, v in ipairs(array) do
        if v ~= EmptyItem then
            if action(v, ...) == true then
                array[i] = EmptyItem
            end
        end
    end
end

function foreachAction(array, action, ...)
    if nil == array or nil == action then
        return
    end

    for i, v in ipairs(array) do
        if v ~= EmptyItem then
            action(v, ...)
        end
    end
end